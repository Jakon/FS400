using System;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using Microsoft.Maps.MapControl.WPF;
using SimWinO.Core;
using SimWinO.WPF.Properties;
using SimWinO.WPF.Utils;

namespace SimWinO.WPF.ViewModels
{
    public class MainViewModel : BaseViewModel
    {
        public SimWinOCore SimWinOCore { get; } = new();

        public Map BingMap { get; set; }

        public Command CheckForUpdateCommand { get; }
        public Command ConnectArduinoCommand { get; }
        public Command DisconnectArduinoCommand { get; }
        public Command ReloadPortsListCommand { get; }
        public Command ConnectFSCommand { get; }
        public Command DisconnectFSCommand { get; }
        public Command SendCommandToArduinoCommand { get; }

        public Location PlaneLocation { get; set; } = new(0, 0);
        public double ZoomLevel { get; set; } = 20;

        public bool UpdateCheckError { get; set; }
        public string ArduinoCommand { get; set; }
        public int BaudRate { get; set; } = 115200;
        public string PortName { get; set; }
        public int ReadTimeout { get; set; } = 250;
        public int WriteTimeout { get; set; } = 250;

        public bool IsArduinoConnected { get; set; }
        public bool IsFlightSimulatorConnected { get; set; }
        
        public MainViewModel()
        {
            CheckForUpdateCommand = new Command(CheckForUpdate);
            ConnectArduinoCommand = new Command(ConnectArduino);
            DisconnectArduinoCommand = new Command(DisconnectArduino);
            ReloadPortsListCommand = new Command(LoadAvailablePorts);
            ConnectFSCommand = new Command(ConnectFlightSimulator);
            DisconnectFSCommand = new Command(DisconnectFlightSimulator);
            SendCommandToArduinoCommand = new Command(SendCommandToArduino);

            SimWinOCore.Config = "DR400";
            PortName = SimWinOCore.AvailablePorts.FirstOrDefault();
            
            SimWinOCore.ArduinoStateChanged += SimWinOCoreOnArduinoStateChanged;
            SimWinOCore.FlightSimulatorStateChanged += SimWinOCoreOnFlightSimulatorStateChanged;
        }

        private void SimWinOCoreOnArduinoStateChanged(object sender, ArduinoStateEventArgs e)
        {
            IsArduinoConnected = e.CurrentState.IsArduinoConnected;
        }
        
        private void SimWinOCoreOnFlightSimulatorStateChanged(object sender, FlightSimulatorStateEventArgs e)
        {
            // Flight Simulator
            IsFlightSimulatorConnected = e.CurrentState.IsFlightSimulatorConnected;
            
            // Bing Maps
            PlaneLocation = new Location(e.CurrentState.SimState.PlaneLatitude, e.CurrentState.SimState.PlaneLongitude);
            ZoomLevel = Math.Max((300 - e.CurrentState.SimState.GroundVelocity) / 20 + 7, 7);
            BingMap.SetView(PlaneLocation, ZoomLevel);
        }
        
        public void CheckForUpdate()
        {
            try
            {
                using var client = new WebClient();

                var remoteVersion = client.DownloadString(Settings.Default.VersionURL);
                var localVersion = File.ReadAllText(Path.Join(AppDomain.CurrentDomain.BaseDirectory, Settings.Default.VersionFile));

                var shouldUpdate = remoteVersion != localVersion;

                if (!shouldUpdate)
                    return;

                var updateDialog = new UpdateDialog();
                var result = updateDialog.ShowDialog();

                if (result != true)
                    return;

                var downloadDialog = new DownloadingDialog();

                Task.Run(async () =>
                {
                    using var downloadClient = new WebClient();

                    var setupFile = Path.Join(Path.GetTempPath(), "SimWinO_update.exe");
                    Debug.WriteLine(setupFile);
                    await downloadClient.DownloadFileTaskAsync(Settings.Default.UpdateURL, setupFile);

                    var process = new Process
                    {
                        StartInfo =
                        {
                            FileName = setupFile,
                            Arguments = "/silent /nocancel /norestart /closeapplications"
                        }
                    };

                    process.Start();
                    Environment.Exit(0);
                });

                downloadDialog.ShowDialog();
            }
            catch (Exception e)
            {
                Debug.WriteLine(e.Message);
                UpdateCheckError = false;
            }
        }

        private void ConnectArduino()
        {
            SimWinOCore.ConnectToArduino(PortName, BaudRate, ReadTimeout, WriteTimeout);
        }

        public void DisconnectArduino()
        {
            SimWinOCore.DisconnectArduino();
        }

        private void LoadAvailablePorts()
        {
            SimWinOCore.RefreshAvailablePorts();
            
            if (!SimWinOCore.AvailablePorts.Contains(PortName))
                PortName = SimWinOCore.AvailablePorts.FirstOrDefault();
        }

        private void ConnectFlightSimulator()
        {
            SimWinOCore.ConnectToFlightSimulator();
        }

        public void DisconnectFlightSimulator()
        {
            SimWinOCore.DisconnectFromFlightSimulator();
        }

        public void SendCommandToArduino()
        {
            if (string.IsNullOrWhiteSpace(ArduinoCommand))
                return;
            
            SimWinOCore.SendCommandToArduino(ArduinoCommand);
            ArduinoCommand = string.Empty;
        }
    }
}
