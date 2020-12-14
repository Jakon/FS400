using Microsoft.FlightSimulator.SimConnect;
using SimWinO.FlightSimulator.Enums;

namespace SimWinO.FlightSimulator.Commands
{
    public class LightsCommands : BaseCommands
    {
        public LightsCommands(SimConnect simConnect) : base(simConnect)
        {
        }

        public void ChangeTaxiLightState()
        {
            SimConnect?.TransmitClientEvent(0, SimEvents.TOGGLE_TAXI_LIGHTS, 0, GROUP.GROUP1, SIMCONNECT_EVENT_FLAG.GROUPID_IS_PRIORITY);
        }

        public void ChangeLandingLightState()
        {
            SimConnect?.TransmitClientEvent(0, SimEvents.LANDING_LIGHTS_TOGGLE, 0, GROUP.GROUP1, SIMCONNECT_EVENT_FLAG.GROUPID_IS_PRIORITY);
        }

        public void ChangeStrobesLightState()
        {
            SimConnect?.TransmitClientEvent(0, SimEvents.STROBES_TOGGLE, 0, GROUP.GROUP1, SIMCONNECT_EVENT_FLAG.GROUPID_IS_PRIORITY);
        }

        public void ChangeNavLightState()
        {
            SimConnect?.TransmitClientEvent(0, SimEvents.TOGGLE_NAV_LIGHTS, 0, GROUP.GROUP1, SIMCONNECT_EVENT_FLAG.GROUPID_IS_PRIORITY);
        }
    }
}
