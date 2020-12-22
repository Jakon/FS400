using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Diagnostics;
using System.IO.Ports;
using SimWinO.Arduino;
using SimWinO.Core.Configs;
using SimWinO.Core.States;
using SimWinO.FlightSimulator;

namespace SimWinO.Core
{
    public class SimWinOCore
    {
        private ISimStruct CurrentState { get; set; }
        private Type CurrentConfigType { get; set; }
        private string CurrentConfig { get; set; }

        private IArduinoParser ArduinoParser { get; set; }

        private FlightSimulatorHelper FSHelper { get; } = new();
        private ArduinoHelper ArduinoHelper { get; } = new();

        public SimWinOCore()
        {
            FSHelper.PropertyChanged += FSHelperOnPropertyChanged;
            ArduinoHelper.PropertyChanged += ArduinoHelperOnPropertyChanged;
            RefreshAvailablePorts();
        }

        private void ArduinoHelperOnPropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            ArduinoStateChanged?.Invoke(this,
                new ArduinoStateEventArgs(
                    new ArduinoState(ArduinoHelper.IsConnected)
                )
            );
        }

        private void FSHelperOnPropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            FlightSimulatorStateChanged?.Invoke(this, 
                new FlightSimulatorStateEventArgs(
                    new FlightSimulatorState(FSHelper.IsConnected, CurrentState)
                )
            );
        }

        #region Flight Simulator internal logic

        private void OnReceiveFromFlightSimulator(object sender, FlightSimulatorHelper.OnReceiveSimObjectEventArgs data)
        {
            CurrentState = (ISimStruct)Convert.ChangeType(data.Data, CurrentConfigType);

            FlightSimulatorStateChanged?.Invoke(this, 
                new FlightSimulatorStateEventArgs(
                    new FlightSimulatorState(FSHelper.IsConnected, CurrentState)
                )
            );
        }

        #endregion

        #region Arduino internal logic

        private void OnArduinoDataReceived(object sender, SerialDataReceivedEventArgs e)
        {
            try
            {
                var message = (sender as SerialPort)?.ReadLine();

                if (string.IsNullOrWhiteSpace(message))
                    return;

                var action = ArduinoParser.Parse(message);
                action?.Invoke(CurrentState, FSHelper);
            }
            catch (Exception exception)
            {
                Debug.WriteLine($"Unable to read from serial port : {exception.Message}");
            }
        }

        #endregion

        #region Flight Simulator API

        public event EventHandler<FlightSimulatorStateEventArgs> FlightSimulatorStateChanged; 
        
        public bool IsFlightSimulatorConnected => FSHelper.IsConnected;

        public Dictionary<string, (Type, IArduinoParser)> AvailableConfigs { get; } = new()
        {
            { "DR400", (typeof(DR400Struct), new DR400Parser()) },
        };

        public string Config
        {
            get => CurrentConfig;
            set
            {
                var (structType, parser) = AvailableConfigs[value];

                CurrentConfig = value;
                CurrentConfigType = structType;
                CurrentState = (ISimStruct)Activator.CreateInstance(CurrentConfigType);
                ArduinoParser = parser;
            }
        }

        public void ConnectToFlightSimulator()
        {
            // TODO: disconnect if already connected
            
            var connect = typeof(FlightSimulatorHelper).GetMethod(nameof(FSHelper.Connect));
            var method = connect?.MakeGenericMethod(CurrentConfigType);
            method?.Invoke(FSHelper, null);

            FSHelper.OnReceiveSimObjectData += OnReceiveFromFlightSimulator;
        }

        public void DisconnectFromFlightSimulator()
        {
            if (!FSHelper.IsConnected)
                return;
            
            FSHelper.Disconnect();
            FSHelper.OnReceiveSimObjectData -= OnReceiveFromFlightSimulator;
        }

        public void OnSourceInitialized(IntPtr hWnd)
        {
            FSHelper.SetWindowHandle(hWnd);
        }

        // Réception de message inter-processus
        public IntPtr WndProc(IntPtr hWnd, int iMsg, IntPtr hWParam, IntPtr hLParam, ref bool bHandled)
        {
            try
            {
                if (iMsg == FSHelper.GetUserSimConnectWinEvent())
                {
                    FSHelper.ReceiveSimConnectMessage();
                }
            }
            catch
            {
                FSHelper.Disconnect();
            }

            return IntPtr.Zero;
        }

        #endregion

        #region Arduino connector API

        public event EventHandler<ArduinoStateEventArgs> ArduinoStateChanged;

        public ObservableCollection<string> AvailablePorts { get; } = new();

        public void ConnectToArduino(string portName, int baudRate, int readTimeout, int writeTimeout)
        {
            // TODO: disconnect if already connected

            ArduinoHelper.PortName = portName;
            ArduinoHelper.BaudRate = baudRate;
            ArduinoHelper.ReadTimeout = readTimeout;
            ArduinoHelper.WriteTimeout = writeTimeout;
            
            ArduinoHelper.Connect();

            if (ArduinoHelper.IsConnected)
                ArduinoHelper.Port.DataReceived += OnArduinoDataReceived;
        }

        public void DisconnectArduino()
        {
            if (!ArduinoHelper.IsConnected)
                return;
            
            ArduinoHelper.Disconnect();
            ArduinoHelper.Port.DataReceived -= OnArduinoDataReceived;
        }

        public void SendCommandToArduino(string command)
        {
            if (ArduinoHelper.IsConnected)
                ArduinoHelper.SendCommandToArduino(command);
        }

        public void RefreshAvailablePorts()
        {
            AvailablePorts.Clear();

            foreach (var port in SerialPort.GetPortNames())
            {
                AvailablePorts.Add(port);
            }
        }

        #endregion
    }
}
