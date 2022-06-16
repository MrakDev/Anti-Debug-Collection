using System;
using System.Threading;

namespace Anti_Debug_Collection.Misc;

public class Timer
{
    public static bool TimerCheck() /* If you want to detect debugger attach, loop it in new thread */
    {
        return Time();
    }
    private static bool Time() /* 300 IQ check, abuse the loading instructions */
    {
        long tickCount = Environment.TickCount;
        Thread.Sleep(500);
        long tickCount2 = Environment.TickCount;
        return tickCount2 - tickCount < 500L;
    }
}