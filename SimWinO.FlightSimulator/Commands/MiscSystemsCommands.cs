using Microsoft.FlightSimulator.SimConnect;
using SimWinO.FlightSimulator.Enums;

namespace SimWinO.FlightSimulator.Commands
{
    public class MiscSystemsCommands : BaseCommands
    {
        public MiscSystemsCommands(SimConnect simConnect) : base(simConnect)
        {
        }

        public void ChangeBatteryState()
        {
            SimConnect?.TransmitClientEvent(0, SimEvents.TOGGLE_MASTER_BATTERY, 0, GROUP.GROUP1, SIMCONNECT_EVENT_FLAG.GROUPID_IS_PRIORITY);
        }

        public void ChangeAlternatorState()
        {
            SimConnect?.TransmitClientEvent(0, SimEvents.TOGGLE_MASTER_ALTERNATOR, 0, GROUP.GROUP1, SIMCONNECT_EVENT_FLAG.GROUPID_IS_PRIORITY);
        }

        public void ChangePitotHeatState()
        {
            SimConnect?.TransmitClientEvent(0, SimEvents.PITOT_HEAT_TOGGLE, 0, GROUP.GROUP1, SIMCONNECT_EVENT_FLAG.GROUPID_IS_PRIORITY);
        }
    }
}
