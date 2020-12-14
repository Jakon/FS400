using Microsoft.FlightSimulator.SimConnect;
using SimWinO.FlightSimulator.Enums;

namespace SimWinO.FlightSimulator.Commands
{
    public class FuelSystemsCommands : BaseCommands
    {
        public FuelSystemsCommands(SimConnect simConnect) : base(simConnect)
        {
        }

        public void SetFuel1SelectorOff()
        {
            SimConnect?.TransmitClientEvent(0, SimEvents.FUEL_SELECTOR_OFF, 0, GROUP.GROUP1, SIMCONNECT_EVENT_FLAG.GROUPID_IS_PRIORITY);
        }

        public void SetFuel1SelectorAll()
        {
            SimConnect?.TransmitClientEvent(0, SimEvents.FUEL_SELECTOR_ALL, 0, GROUP.GROUP1, SIMCONNECT_EVENT_FLAG.GROUPID_IS_PRIORITY);
        }
    }
}