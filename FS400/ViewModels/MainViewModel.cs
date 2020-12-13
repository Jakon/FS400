using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.IO.Ports;
using System.Linq;
using FS400.Utils;
using Microsoft.FlightSimulator.SimConnect;

namespace FS400.ViewModels
{
    public class MainViewModel : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;

        public ArduinoHelper ArduinoHelper { get; set; }

        public FlightSimulatorHelper FlightSimulatorHelper { get; set; }

        public ObservableCollection<string> AvailablePorts { get; set; }

        public DR400Struc DR400Variables { get; set; }

        public MainViewModel()
        {
            ArduinoHelper = new ArduinoHelper();
            FlightSimulatorHelper = new FlightSimulatorHelper();

            LoadAvailablePorts();
        }

        /// <summary>
        /// Fonction appelée quand SimConnect récupère des données depuis FS
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="data"></param>
        private void SimConnectOnOnRecvSimobjectDataBytype(SimConnect sender, SIMCONNECT_RECV_SIMOBJECT_DATA_BYTYPE data)
        {
            if (data.dwRequestID == 0) // Notre Struct
            {
                DR400Variables = (DR400Struc)data.dwData[0];
            }
        }

        public void ConnectArduino()
        {
            ArduinoHelper.Connect();

            if (ArduinoHelper.IsConnected)
            {
                ArduinoHelper.Port.DataReceived += PortOnDataReceived;
            }
        }

        public void DisconnectArduino()
        {
            ArduinoHelper.Disconnect();
        }

        public void LoadAvailablePorts()
        {
            AvailablePorts = new ObservableCollection<string>(SerialPort.GetPortNames());
            if (AvailablePorts.Count > 0)
            {
                ArduinoHelper.PortName = AvailablePorts.First();
            }
        }

        public void ConnectFlightSimulator()
        {
            FlightSimulatorHelper.Connect();

            FlightSimulatorHelper.SimConnect.OnRecvSimobjectDataBytype += SimConnectOnOnRecvSimobjectDataBytype;
        }

        public void DisconnectFlightSimulator()
        {
            FlightSimulatorHelper.Disconnect();
        }

        /// <summary>
        /// Fonction appelée dès que l'Arduino transmet des informations vers l'ordinateur
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void PortOnDataReceived(object sender, SerialDataReceivedEventArgs e)
        {
            try
            {
                string message = (sender as SerialPort).ReadLine();
                if (!string.IsNullOrWhiteSpace(message))
                {
                    var type = message.Substring(0, 1); // On a une lettre
                    var variable = message.Substring(1, 3);
                    var value = message.Substring(4).Replace("\r","");

                    switch (variable)
                    {
                        // Magneto Off
                        case "000":
                            FlightSimulatorHelper.SetMagnetoOff();
                            break;
                        case "001":
                            break;
                        // Battery Switch
                        case "002":
                            switch (value)
                            {
                                case "0" when DR400Variables.IsElectricalMasterBatteryOn:
                                case "1" when !DR400Variables.IsElectricalMasterBatteryOn:
                                    FlightSimulatorHelper.ChangeBatteryState();
                                    break;
                            }
                            break;
                        // Alternator Switch
                        case "003":
                            switch (value)
                            {
                                case "0" when DR400Variables.IsAlternatorOn:
                                case "1" when !DR400Variables.IsAlternatorOn:
                                    FlightSimulatorHelper.ChangeAlternatorState();
                                    break;
                            }
                            break;
                        case "004":
                            FlightSimulatorHelper.SetMagnetoBoth();
                            break;
                        case "005":
                            break;
                        case "006":
                            break;
                        case "007":
                            break;
                        case "008":
                            break;
                        case "009":
                            break;
                        case "010":
                            break;
                        case "011":
                            break;
                        case "012":
                            break;
                        case "013":
                            break;
                        case "014":
                            FlightSimulatorHelper.SetMagnetoRight();
                            break;
                        case "015":
                            FlightSimulatorHelper.SetMagnetoLeft();
                            break;
                    }
                }
            }
            catch (Exception exception)
            {

            }
        }
    }
}
