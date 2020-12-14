using Microsoft.FlightSimulator.SimConnect;
using SimWinO.FlightSimulator.Enums;

namespace SimWinO.FlightSimulator.Commands
{
    public class EngineCommands : BaseCommands
    {
        public EngineCommands(SimConnect simConnect) : base(simConnect)
        {
        }

        private void SetMagneto1(uint value)
        {
            // MAGNETO1_SET 
            SimConnect?.TransmitClientEvent(0, SimEvents.MAGNETO1_SET, value, GROUP.GROUP1, SIMCONNECT_EVENT_FLAG.GROUPID_IS_PRIORITY);
        }

        public void SetMagnetoOff()
        {
            SetMagneto1(0);
        }

        public void SetMagnetoRight()
        {
            SetMagneto1(1);
        }

        public void SetMagnetoLeft()
        {
            SetMagneto1(2);
        }

        public void SetMagnetoBoth()
        {
            SetMagneto1(3);
        }

        public void ChangeFuelPumpState()
        {
            SimConnect?.TransmitClientEvent(0, SimEvents.TOGGLE_ELECT_FUEL_PUMP1, 0, GROUP.GROUP1, SIMCONNECT_EVENT_FLAG.GROUPID_IS_PRIORITY);
        }

        public void SetMixture1(uint value)
        {
            SimConnect?.TransmitClientEvent(0, SimEvents.MIXTURE1_SET, value, GROUP.GROUP1, SIMCONNECT_EVENT_FLAG.GROUPID_IS_PRIORITY);
        }

        public void SetAntiIce1(uint value)
        {
            SimConnect?.TransmitClientEvent(0, SimEvents.ANTI_ICE_SET_ENG1, value, GROUP.GROUP1, SIMCONNECT_EVENT_FLAG.GROUPID_IS_PRIORITY);
        }

        public void SetStarter1()
        {
            SimConnect?.TransmitClientEvent(0, SimEvents.TOGGLE_STARTER1, 1, GROUP.GROUP1, SIMCONNECT_EVENT_FLAG.GROUPID_IS_PRIORITY);
        }

        public void SetAutoStart()
        {
            SimConnect?.TransmitClientEvent(0, SimEvents.ENGINE_AUTO_START, 1, GROUP.GROUP1, SIMCONNECT_EVENT_FLAG.GROUPID_IS_PRIORITY);
        }

    }
}
