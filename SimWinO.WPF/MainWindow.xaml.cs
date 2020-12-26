using System;
using System.ComponentModel;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Interop;
using Microsoft.Maps.MapControl.WPF;
using SimWinO.WPF.Properties;
using SimWinO.WPF.ViewModels;

namespace SimWinO.WPF
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
            ViewModel.SimWinOCore.OnSourceInitialized(GetHWinSource().Handle);
            base.OnSourceInitialized(e);
        }

        // Réception de message inter-processus
        private IntPtr WndProc(IntPtr hWnd, int iMsg, IntPtr hWParam, IntPtr hLParam, ref bool bHandled)
        {
            return ViewModel.SimWinOCore.WndProc(hWnd, iMsg, hWParam, hLParam, ref bHandled);
        }

        #endregion

        public MainViewModel ViewModel { get; }

        public MainWindow()
        {
            ViewModel = new MainViewModel();

            InitializeComponent();

            ViewModel.BingMap = BingMap;
        }

        private void MainWindow_OnClosing(object sender, CancelEventArgs e)
        {
            ViewModel.DisconnectFlightSimulator();
            ViewModel.DisconnectArduino();
        }

        private void ArduinoCommandTextBox_OnKeyUp(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter)
            {
                ViewModel.SendCommandToArduino();
            }
        }

        private void MainWindow_OnLoaded(object sender, RoutedEventArgs e)
        {
            ViewModel.CheckForUpdate();

            if (Settings.Default.FollowPlane)
                BingMap.MouseMove += ViewModel.DisableMouseEvent;
            
            if (Settings.Default.AutoZoom)
                BingMap.MouseWheel += ViewModel.DisableMouseWheelEvent;

            BingMap.Mode = Settings.Default.MapSatelliteMode ? new AerialMode(true) : new RoadMode();
        }

        private void Tabs_OnSelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (BingMap == null)
                return;
            
            BingMap.CredentialsProvider = MapsTab?.IsSelected switch
            {
                true => new ApplicationIdCredentialsProvider("Ajd2i1Te41xx2-us7UBRKPy-C-MYW0oeISO9I1dD0mYMYbvaXCSW8Eas6Jz9yRSx"),
                false => null,
                _ => BingMap.CredentialsProvider
            };
        }
    }
}
