using System;
using System.Diagnostics;
using System.IO;
using System.Net;
using System.Threading.Tasks;
using System.Windows;
using MahApps.Metro.Controls.Dialogs;
using SimWinO.Core;
using SimWinO.WPF.Properties;
using SimWinO.WPF.Utils;

namespace SimWinO.WPF.ViewModels
{
    public class MainViewModel : BaseViewModel
    {
        public SimWinOCore SimWinOCore { get; } = new SimWinOCore();

        public Command CheckForUpdateCommand { get; set; }
        public Command ConnectArduinoCommand { get; set; }
        public Command DisconnectArduinoCommand { get; set; }
        public Command ReloadPortsListCommand { get; set; }
        public Command ConnectFSCommand { get; set; }
        public Command DisconnectFSCommand { get; set; }
        public Command SendCommandToArduinoCommand { get; set; }

        public bool UpdateCheckError { get; set; }
        public string ArduinoCommand { get; set; }

        public MainViewModel()
        {
            CheckForUpdateCommand = new Command(async () => await CheckForUpdate());
            ConnectArduinoCommand = new Command(ConnectArduino);
            DisconnectArduinoCommand = new Command(DisconnectArduino);
            ReloadPortsListCommand = new Command(LoadAvailablePorts);
            ConnectFSCommand = new Command(ConnectFlightSimulator);
            DisconnectFSCommand = new Command(DisconnectFlightSimulator);
            SendCommandToArduinoCommand = new Command(SendCommandToArduino);

            CheckForUpdateCommand.Execute();
            
            SimWinOCore.Config = "DR400";
        }

        private async Task CheckForUpdate()
        {
            try
            {
                using var client = new WebClient();

                var remoteVersion = await client.DownloadStringTaskAsync(Settings.Default.VersionURL);
                var localVersion = await File.ReadAllTextAsync(Path.Join(AppDomain.CurrentDomain.BaseDirectory, Settings.Default.VersionFile));

                var shouldUpdate = remoteVersion != localVersion;

                if (shouldUpdate)
                {
                    var updateDialog = new UpdateDialog();
                    var result = updateDialog.ShowDialog();

                    if (result == true)
                    {
                        var setupFile = Path.Join(Path.GetTempPath(), "SimWinO_update.exe");
                        Debug.WriteLine(setupFile);
                        await client.DownloadFileTaskAsync(Settings.Default.UpdateURL, setupFile);

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
                    }
                }
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
