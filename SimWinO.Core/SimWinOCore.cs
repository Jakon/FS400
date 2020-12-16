using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Diagnostics;
using System.IO.Ports;
using System.Linq;
using System.Runtime.CompilerServices;
using SimWinO.Arduino;
using SimWinO.Core.Configs;
using SimWinO.FlightSimulator;

namespace SimWinO.Core
{
    public class SimWinOCore : INotifyCollectionChanged, INotifyPropertyChanged
    {
        public event NotifyCollectionChangedEventHandler CollectionChanged;

        public event PropertyChangedEventHandler PropertyChanged;

        private ISimStruct CurrentState { get; set; }
        private Type CurrentConfigType { get; set; }
        private string CurrentConfig { get; set; }

        private IArduinoParser ArduinoParser { get; set; }

        private FlightSimulatorHelper FSHelper { get; set; } = new FlightSimulatorHelper();
        private ArduinoHelper ArduinoHelper { get; set; } = new ArduinoHelper();
        
        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        public SimWinOCore()
        {
            FSHelper.PropertyChanged += FSHelperOnPropertyChanged;
            ArduinoHelper.PropertyChanged += ArduinoHelperOnPropertyChanged;
            RefreshAvailablePorts();
        }

        private void ArduinoHelperOnPropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            OnPropertyChanged(nameof(IsArduinoConnected));
            OnPropertyChanged(nameof(PortName));
            OnPropertyChanged(nameof(BaudRate));
            OnPropertyChanged(nameof(WriteTimeout));
            OnPropertyChanged(nameof(ReadTimeout));
        }

        private void FSHelperOnPropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            OnPropertyChanged(nameof(IsFlightSimulatorConnected));
        }

        #region Flight Simulator internal logic

        private void OnReceiveFromFlightSimulator(object sender, FlightSimulatorHelper.OnReceiveSimObjectEventArgs data)
        {
            // TODO: vérifier si cette ligne cast correctement
            CurrentState = (ISimStruct)Convert.ChangeType(data.Data, CurrentConfigType);
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

        public bool IsFlightSimulatorConnected => FSHelper.IsConnected;

        public Dictionary<string, (Type, IArduinoParser)> AvailableConfigs { get; } = new Dictionary<string, (Type, IArduinoParser)>
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

            //FSHelper = new FlightSimulatorHelper();
            var connect = typeof(FlightSimulatorHelper).GetMethod(nameof(FSHelper.Connect));
            var method = connect?.MakeGenericMethod(CurrentConfigType);
            method?.Invoke(FSHelper, null);

            FSHelper.OnReceiveSimObjectData += OnReceiveFromFlightSimulator;
        }

        public void DisconnectFromFlightSimulator()
        {
            if (FSHelper.IsConnected)
            {
                FSHelper.Disconnect();
            }
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

        public ObservableCollection<string> AvailablePorts { get; set; }

        public bool IsArduinoConnected => ArduinoHelper.IsConnected;

        public string PortName
        {
            get => ArduinoHelper.PortName;
            set => ArduinoHelper.PortName = value;
        }

        public int BaudRate
        {
            get => ArduinoHelper.BaudRate;
            set => ArduinoHelper.BaudRate = value;
        }

        public int ReadTimeout
        {
            get => ArduinoHelper.ReadTimeout;
            set => ArduinoHelper.ReadTimeout = value;
        }

        public int WriteTimeout
        {
            get => ArduinoHelper.WriteTimeout;
            set => ArduinoHelper.WriteTimeout = value;
        }

        public void ConnectToArduino()
        {
            // TODO: disconnect if already connected

            //ArduinoHelper = new ArduinoHelper();
            ArduinoHelper.Connect();

            if (ArduinoHelper.IsConnected)
            {
                ArduinoHelper.Port.DataReceived += OnArduinoDataReceived;
            }
        }

        public void DisconnectArduino()
        {
            if (ArduinoHelper.IsConnected)
            {
                ArduinoHelper.Disconnect();
            }
        }

        public void SendCommandToArduino(string command)
        {
            if (ArduinoHelper.IsConnected)
                ArduinoHelper.SendCommandToArduino(command);
        }

        public void RefreshAvailablePorts()
        {
            AvailablePorts = new ObservableCollection<string>(SerialPort.GetPortNames());

            if (!AvailablePorts.Contains(ArduinoHelper.PortName))
                ArduinoHelper.PortName = AvailablePorts.FirstOrDefault();
        }

        #endregion
    }
}
