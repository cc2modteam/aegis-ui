using System;
using System.Threading;

namespace CC2AirController
{
    public class Utils
    {
        public static bool ActionMaxSleep(Action a, double max_msec)
        {
            var started = DateTime.UtcNow;
            a();
            var ended = DateTime.UtcNow;
            var elapsed = ended - started;
            var msec = elapsed.TotalMilliseconds;
            var remain = max_msec - msec;
            if (remain > 0)
            {
                Thread.Sleep((int)remain);
            }

            return remain > 0;
        }
    }
}