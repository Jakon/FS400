﻿using System;
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