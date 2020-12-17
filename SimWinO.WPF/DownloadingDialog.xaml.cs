using System;
using System.Windows;

namespace SimWinO.WPF
{
    /// <summary>
    /// Logique d'interaction pour DownloadingDialog.xaml
    /// </summary>
    public partial class DownloadingDialog
    {
        public DownloadingDialog()
        {
            InitializeComponent();
        }

        private void DownloadingDialog_OnContentRendered(object sender, EventArgs e)
        {
            Focus();
        }

        private void DownloadingDialog_OnClick(object sender, RoutedEventArgs e)
        {
            DialogResult = true;
        }
    }
}
