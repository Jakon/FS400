using System;
using System.ComponentModel;
using System.Diagnostics;
using System.IO;
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
        public SimWinOCore SimWinOCore { get; } = new SimWinOCore();

        public Map BingMap { get; set; }

        public Command CheckForUpdateCommand { get; set; }
        public Command ConnectArduinoCommand { get; set; }
        public Command DisconnectArduinoCommand { get; set; }
        public Command ReloadPortsListCommand { get; set; }
        public Command ConnectFSCommand { get; set; }
        public Command DisconnectFSCommand { get; set; }
        public Command SendCommandToArduinoCommand { get; set; }

        public Location PlaneLocation => SimWinOCore.PlaneLocation;
        public double ZoomLevel => SimWinOCore.ZoomLevel;

        public bool UpdateCheckError { get; set; }
        public string ArduinoCommand { get; set; }

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
            SimWinOCore.PropertyChanged += SimWinOCoreOnPropertyChanged;
        }

        private void SimWinOCoreOnPropertyChanged(object? sender, PropertyChangedEventArgs e)
        {
            if (e.PropertyName == nameof(SimWinOCore.PlaneLocation))
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
            SimWinOCore.ConnectToArduino();
        }

        public void DisconnectArduino()
        {
            SimWinOCore.DisconnectArduino();
        }

        private void LoadAvailablePorts()
        {
            SimWinOCore.RefreshAvailablePorts();
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
            if (!string.IsNullOrWhiteSpace(ArduinoCommand))
            {
                SimWinOCore.SendCommandToArduino(ArduinoCommand);
                ArduinoCommand = string.Empty;
            }
        }
    }
}
