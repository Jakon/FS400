using System;
using System.ComponentModel;
using System.Windows;
using System.Windows.Interop;
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
            GetHWinSource().AddHook(ViewModel.SimWinOCore.WndProc);
            ViewModel.SimWinOCore.OnSourceInitialized(GetHWinSource().Handle);
            base.OnSourceInitialized(e);
        }

        #endregion

        public MainViewModel ViewModel { get; set; }

        public MainWindow()
        {
            ViewModel = new MainViewModel();

            InitializeComponent();
        }

        private void MainWindow_OnClosing(object sender, CancelEventArgs e)
        {
            ViewModel.DisconnectFlightSimulator();
            ViewModel.DisconnectArduino();
        }
    }
}
