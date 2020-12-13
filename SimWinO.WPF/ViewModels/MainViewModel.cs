using SimWinO.FlightSimulator;
using SimWinO.Core;

namespace SimWinO.WPF.ViewModels
{
    public class MainViewModel : BaseViewModel
    {
        public SimWinOCore SimWinOCore { get; } = new SimWinOCore();

        public Command ConnectArduinoCommand { get; set; }
        public Command DisconnectArduinoCommand { get; set; }
        public Command ReloadPortsListCommand { get; set; }
        public Command ConnectFSCommand { get; set; }
        public Command DisconnectFSCommand { get; set; }

        public MainViewModel()
        {
            ConnectArduinoCommand = new Command(ConnectArduino);
            DisconnectArduinoCommand = new Command(DisconnectArduino);
            ReloadPortsListCommand = new Command(LoadAvailablePorts);
            ConnectFSCommand = new Command(ConnectFlightSimulator);
            DisconnectFSCommand = new Command(DisconnectFlightSimulator);
        }

        public void ConnectArduino()
        {
            SimWinOCore.ConnectToArduino();
        }

        public void DisconnectArduino()
        {
            SimWinOCore.DisconnectArduino();
        }

        public void LoadAvailablePorts()
        {
            SimWinOCore.RefreshAvailablePorts();
        }

        public void ConnectFlightSimulator()
        {
            SimWinOCore.ConnectToFlightSimulator();
        }

        public void DisconnectFlightSimulator()
        {
            SimWinOCore.DisconnectFromFlightSimulator();
        }
    }
}
