using System;
using System.Windows;

namespace SimWinO.WPF
{
    /// <summary>
    /// Logique d'interaction pour UpdateDialog.xaml
    /// </summary>
    public partial class UpdateDialog
    {
        public UpdateDialog()
        {
            InitializeComponent();
        }

        private void UpdateDialog_OnContentRendered(object sender, EventArgs e)
        {
            Focus();
        }

        private void Update_OnClick(object sender, RoutedEventArgs e)
        {
            DialogResult = true;
        }
    }
}
