﻿using System;
using System.ComponentModel;
using System.Diagnostics;
using System.Threading;
using Microsoft.FlightSimulator.SimConnect;
using SimWinO.FlightSimulator.Commands;
using SimWinO.FlightSimulator.Enums;
using SimWinO.FlightSimulator.Utils;

namespace SimWinO.FlightSimulator
{
    public class FlightSimulatorHelper : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;

        private IntPtr m_hWnd;
        private const int WM_USER_SIMCONNECT = 0x0402;

        private CancellationTokenSource tickCancellationToken { get; set; }

        public SimConnect SimConnect { get; set; }

        public bool IsConnected { get; set; }

        public EngineCommands EngineCommands { get; set; }
        public MiscSystemsCommands MiscSystemsCommands { get; set; }
        public LightsCommands LightsCommands { get; set; }
        public FuelSystemsCommands FuelSystemsCommands { get; set; }

        private void StartTimer()
        {
            tickCancellationToken = new CancellationTokenSource();
            _ = PeriodicTask.Run(TimerTick, TimeSpan.FromMilliseconds(20), tickCancellationToken.Token);
        }

        private void StopTimer()
        {
            tickCancellationToken.Cancel();
        }

        /// <summary>
        /// Tick qui déclenche une récupération de données dans FS
        /// </summary>
        private void TimerTick()
        {
            if (!IsConnected)
                return;

            try
            {
                SimConnect?.RequestDataOnSimObjectType(DATA_REQUESTS.Dummy, DEFINITIONS.Dummy, 0, SIMCONNECT_SIMOBJECT_TYPE.USER);
            }
            catch (Exception ex)
            {
                Debug.Write(ex.Message);
            }
        }

        /// <summary>
        /// Méthode de connexion au simulateur
        /// </summary>
        public void Connect<T>()
        {
            try
            {
                // Le constructeur fait la connexion.
                SimConnect = new SimConnect("SimWinO", m_hWnd, WM_USER_SIMCONNECT, null, 0);

                // On attache différents évènements (Open/Quit/Exception)
                SimConnect.OnRecvOpen += SimConnectOnOnRecvOpen;
                SimConnect.OnRecvQuit += SimConnectOnOnRecvQuit;

                SimConnect.OnRecvException += SimConnectOnOnRecvException;

                // On relie la réception d'informations à l'évènement OnReceiveSimObjectData
                SimConnect.OnRecvSimobjectDataBytype += (sender, data) =>
                {
                    if (data.dwRequestID != 0)
                        return;

                    var handler = OnReceiveSimObjectData;

                    var eventArgs = new OnReceiveSimObjectEventArgs
                    {
                        Data = data.dwData[0]
                    };

                    handler?.Invoke(this, eventArgs);
                };

                // On fait l'assignation des commandes et des variables
                AssignCommandsAndValues<T>();
            }
            catch (Exception e)
            {
                Debug.Write(e.Message);
            }
        }

        public event EventHandler<OnReceiveSimObjectEventArgs> OnReceiveSimObjectData;

        public class OnReceiveSimObjectEventArgs : EventArgs
        {
            public object Data { get; set; }
        }

        private void SimConnectOnOnRecvException(SimConnect sender, SIMCONNECT_RECV_EXCEPTION data)
        {
            var simException = (SIMCONNECT_EXCEPTION)data.dwException;
            Debug.Write($"SimConnect_OnRecvException : {simException}");
        }

        private void SimConnectOnOnRecvQuit(SimConnect sender, SIMCONNECT_RECV data)
        {
            Disconnect();
        }

        private void SimConnectOnOnRecvOpen(SimConnect sender, SIMCONNECT_RECV_OPEN data)
        {
            IsConnected = true;
            StartTimer();
        }

        public void Disconnect()
        {
            if (SimConnect != null)
            {
                SimConnect.Dispose();
                SimConnect = null;
                StopTimer();
                IsConnected = false;
            }
        }

        #region Des trucs de Windows pour se connecter au Sim, j'ai pas tout compris

        public void SetWindowHandle(IntPtr hWnd)
        {
            m_hWnd = hWnd;
        }

        public int GetUserSimConnectWinEvent()
        {
            return WM_USER_SIMCONNECT;
        }

        public void ReceiveSimConnectMessage()
        {
            try
            {
                SimConnect?.ReceiveMessage();
            }
            catch (Exception e)
            {
                Debug.Write(e.Message);
            }
        }

        #endregion

        /// <summary>
        /// Méthode d'assignation des commandes et des variables 
        /// </summary>
        public void AssignCommandsAndValues<T>()
        {
            EngineCommands = new EngineCommands(SimConnect);
            MiscSystemsCommands = new MiscSystemsCommands(SimConnect);
            LightsCommands = new LightsCommands(SimConnect);
            FuelSystemsCommands = new FuelSystemsCommands(SimConnect);

            // On assigne les différentes commandes qu'on veut envoyer au simulateur
            foreach (SimEvents simEvent in Enum.GetValues(typeof(SimEvents)))
            {
                SimConnect.MapClientEventToSimEvent(simEvent, simEvent.ToString());
            }

            // Il reste à assigner les différentes variables à récupérer au niveau du simulateur
            foreach (var variable in SimVariables.Variables)
            {
                SimConnect.AddToDataDefinition(DEFINITIONS.Dummy, variable.PropertyName, variable.PropertyType, SIMCONNECT_DATATYPE.FLOAT64, 0.0f, SimConnect.SIMCONNECT_UNUSED);
            }

            SimConnect.RegisterDataDefineStruct<T>(DEFINITIONS.Dummy);
        }
    }
}
