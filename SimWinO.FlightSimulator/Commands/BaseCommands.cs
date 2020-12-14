using Microsoft.FlightSimulator.SimConnect;

namespace SimWinO.FlightSimulator.Commands
{
    public abstract class BaseCommands
    {
        protected readonly SimConnect SimConnect;

        protected BaseCommands(SimConnect simConnect)
        {
            SimConnect = simConnect;
        }
    }
}
