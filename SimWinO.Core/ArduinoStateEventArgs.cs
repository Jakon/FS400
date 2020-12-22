using System;
using SimWinO.Core.States;

namespace SimWinO.Core
{
    public class ArduinoStateEventArgs : EventArgs
    {
        public ArduinoState CurrentState { get; }

        public ArduinoStateEventArgs(ArduinoState state)
        {
            CurrentState = state;
        }
    }
}
