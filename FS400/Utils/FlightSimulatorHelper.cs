using System;
using System.ComponentModel;
using System.Windows.Threading;
using Microsoft.FlightSimulator.SimConnect;

namespace FS400.Utils
{
    public class FlightSimulatorHelper : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;

        private IntPtr m_hWnd;
        private const int WM_USER_SIMCONNECT = 0x0402;
        private readonly DispatcherTimer _timer = new DispatcherTimer();

        public SimConnect SimConnect { get; set; }

        public bool IsConnected { get; set; }


        public FlightSimulatorHelper()
        {
            _timer.Interval = new TimeSpan(0, 0, 0, 0, 250);
            _timer.Tick += Timer_Tick;
        }

        /// <summary>
        /// Tick qui déclenche une récupération de données dans FS
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Timer_Tick(object sender, EventArgs e)
        {
            try
            {
                //for (int i = 0; i < 2; i++)
                //{
                //    SimConnect?.RequestDataOnSimObjectType(DATA_REQUESTS.DR400Request, (DEFINITIONS)i, 0, SIMCONNECT_SIMOBJECT_TYPE.USER);
                //}

                SimConnect?.RequestDataOnSimObjectType(DATA_REQUESTS.DR400Request, DEFINITIONS.DR400Struct, 0, SIMCONNECT_SIMOBJECT_TYPE.USER);
            }
            catch (Exception ex)
            {
                ConsoleHelper.Instance.Write(ex.Message);
            }
        }

        /// <summary>
        /// Méthode de connexion au simulateur
        /// </summary>
        public void Connect()
        {
            try
            {
                // Le constructeur fait la connexion.
                SimConnect = new SimConnect("FS400", m_hWnd, WM_USER_SIMCONNECT, null, 0);

                // On attache différents évènements (Open/Quit/Exception)
                SimConnect.OnRecvOpen += SimConnectOnOnRecvOpen;
                SimConnect.OnRecvQuit += SimConnectOnOnRecvQuit;

                SimConnect.OnRecvException += SimConnectOnOnRecvException;

                // On fait l'assignation des commandes et des variables
                AssignCommandsAndValues();
            }
            catch (Exception e)
            {
                ConsoleHelper.Instance.Write(e.Message);
            }
        }

        private void SimConnectOnOnRecvException(SimConnect sender, SIMCONNECT_RECV_EXCEPTION data)
        {
            var simException = (SIMCONNECT_EXCEPTION)data.dwException;
            ConsoleHelper.Instance.Write($"SimConnect_OnRecvException : {simException}");
        }

        private void SimConnectOnOnRecvQuit(SimConnect sender, SIMCONNECT_RECV data)
        {
            Disconnect();
        }

        private void SimConnectOnOnRecvOpen(SimConnect sender, SIMCONNECT_RECV_OPEN data)
        {
            IsConnected = true;
            _timer.Start();
        }

        public void Disconnect()
        {
            if (SimConnect != null)
            {
                SimConnect.Dispose();
                SimConnect = null;
                _timer.Stop();
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
                ConsoleHelper.Instance.Write(e.Message);
            }
        }
        #endregion

        /// <summary>
        /// Méthode d'assignation des commandes et des variables 
        /// </summary>
        public void AssignCommandsAndValues()
        {
            // On assigne les différentes commandes qu'on veut envoyer au simulateur
            foreach (SimEvents simEvent in Enum.GetValues(typeof(SimEvents)))
            {
                SimConnect.MapClientEventToSimEvent(simEvent, simEvent.ToString());
            }

            // Il reste à assigner les différentes variables à récupérer au niveau du simulateur
            foreach (var name in SimVariables.Names)
            {
                SimConnect.AddToDataDefinition(DEFINITIONS.DR400Struct, name, "", SIMCONNECT_DATATYPE.FLOAT64, 0.0f, SimConnect.SIMCONNECT_UNUSED);
            }

            SimConnect.RegisterDataDefineStruct<DR400Struc>(DEFINITIONS.DR400Struct);
        }


        private void SetMagneto1(uint value)
        {
            // MAGNETO1_SET 
            if (IsConnected)
                SimConnect.TransmitClientEvent(0, SimEvents.MAGNETO1_SET, value, GROUP.GROUP1, SIMCONNECT_EVENT_FLAG.GROUPID_IS_PRIORITY);
        }

        public void ChangeBatteryState()
        {
            if (IsConnected)
                SimConnect.TransmitClientEvent(0, SimEvents.TOGGLE_MASTER_BATTERY, 0, GROUP.GROUP1, SIMCONNECT_EVENT_FLAG.GROUPID_IS_PRIORITY);
        }

        public void ChangeAlternatorState()
        {
            if (IsConnected)
                SimConnect.TransmitClientEvent(0, SimEvents.TOGGLE_MASTER_ALTERNATOR, 0, GROUP.GROUP1, SIMCONNECT_EVENT_FLAG.GROUPID_IS_PRIORITY);
        }

        public void SetMagnetoOff()
        {
            SetMagneto1(0);
        }

        public void SetMagnetoRight()
        {
            SetMagneto1(1);
        }

        public void SetMagnetoLeft()
        {
            SetMagneto1(2);
        }

        public void SetMagnetoBoth()
        {
            SetMagneto1(3);
        }
    }
}
