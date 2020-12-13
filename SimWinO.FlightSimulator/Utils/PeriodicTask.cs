using System;
using System.Threading;
using System.Threading.Tasks;

namespace SimWinO.FlightSimulator.Utils
{
    public static class PeriodicTask
    {
        public static async Task Run(Action action, TimeSpan period, CancellationToken token)
        {
            while (!token.IsCancellationRequested)
            {
                await Task.Delay(period, token);

                if (!token.IsCancellationRequested)
                    action();
            }
        }

        public static Task Run(Action action, TimeSpan period)
        {
            return Run(action, period, CancellationToken.None);
        }
    }
}
