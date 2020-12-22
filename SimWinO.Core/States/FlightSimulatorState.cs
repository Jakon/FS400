using SimWinO.Core.Configs;

namespace SimWinO.Core.States
{
    public record FlightSimulatorState(
        bool IsFlightSimulatorConnected,
        ISimStruct SimState
    );
}
