using System.Windows;
using SimWinO.WPF.Properties;

namespace SimWinO.WPF
{
    /// <summary>
    /// Logique d'interaction pour App.xaml
    /// </summary>
    public partial class App : Application
    {
        public App()
        {
            // Si l'application a été mise à jour, migrer les préférences utilisateurs de l'ancienne version
            if (!Settings.Default.UpgradeRequired)
                return;

            Settings.Default.Upgrade();
            Settings.Default.UpgradeRequired = false;
            Settings.Default.Save();
        }
    }
}
