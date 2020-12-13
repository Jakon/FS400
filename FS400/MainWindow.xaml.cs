using System;
using System.ComponentModel;
using System.Windows;
using System.Windows.Interop;
using FS400.Utils;
using FS400.ViewModels;

namespace FS400
{
    /// <summary>
    /// Logique d'interaction pour MainWindow.xaml
    /// </summary>
    public partial class MainWindow
    {
        #region Flight Sim Code pour se connecter
        /* Du code qui sert à interconnecter Flight Sim avec l'appli. J'ai pas tout compris comment ça marche */
        protected HwndSource GetHWinSource()
        {
            return PresentationSource.FromVisual(this) as HwndSource;
        }

        protected override void OnSourceInitialized(EventArgs e)
        {
            GetHWinSource().AddHook(WndProc);
            ViewModel.FlightSimulatorHelper.SetWindowHandle(GetHWinSource().Handle);
            base.OnSourceInitialized(e);
        }

        private IntPtr WndProc(IntPtr hWnd, int iMsg, IntPtr hWParam, IntPtr hLParam, ref bool bHandled)
        {
            try
            {
                if (iMsg == ViewModel.FlightSimulatorHelper.GetUserSimConnectWinEvent())
                {
                    ViewModel.FlightSimulatorHelper.ReceiveSimConnectMessage();
                }
            }
            catch
            {
                ViewModel.FlightSimulatorHelper.Disconnect();
            }

            return IntPtr.Zero;
        }
        #endregion

        public MainViewModel ViewModel { get; set; }

        public MainWindow()
        {
            ViewModel = new MainViewModel();

            InitializeComponent();

            ConsoleHelper.Instance.SetConsole(DebugBox);
        }

        private void ConnectArduinoButton_OnClick(object sender, RoutedEventArgs e)
        {
            ViewModel.ConnectArduino();
        }

        private void DisconnectArduinoButton_OnClick(object sender, RoutedEventArgs e)
        {
            ViewModel.DisconnectArduino();
        }

        private void ReloadPortsListButton_OnClick(object sender, RoutedEventArgs e)
        {
            ViewModel.LoadAvailablePorts();
        }

        private void ConnectFSButton_OnClick(object sender, RoutedEventArgs e)
        {
            ViewModel.ConnectFlightSimulator();
        }

        private void DisconnectFSButton_OnClick(object sender, RoutedEventArgs e)
        {
            ViewModel.DisconnectFlightSimulator();
        }

        private void TestButton_OnClick(object sender, RoutedEventArgs e)
        {
            //ViewModel.FlightSimulatorHelper.ChangeBatteryState();
            //Thread.Sleep(2000);
            //ViewModel.FlightSimulatorHelper.ChangeBatteryState();
            //Thread.Sleep(2000);
            //ViewModel.FlightSimulatorHelper.ChangeBatteryState();
            //ViewModel.FlightSimulatorHelper.MagnetoOff(1);
            //Thread.Sleep(2000);
            //ViewModel.FlightSimulatorHelper.MagnetoOff(2);
            //Thread.Sleep(2000);
            //ViewModel.FlightSimulatorHelper.MagnetoOff(3);
        }

        private void MainWindow_OnClosing(object sender, CancelEventArgs e)
        {
            ViewModel.DisconnectFlightSimulator();
            ViewModel.DisconnectArduino();
        }
    }
}
