using System;
using SimWinO.Core.States;

namespace SimWinO.Core
{
    public class FlightSimulatorStateEventArgs : EventArgs
    {
        public FlightSimulatorState CurrentState { get; }

        public FlightSimulatorStateEventArgs(FlightSimulatorState state)
        {
            CurrentState = state;
        }
    }
}
