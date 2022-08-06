using System;
using System.Threading;

namespace Anti_Debug_Collection.Misc;

public static class Timer
{
    public static bool TimerCheck()
    {
        return Time();
    }
    private static bool Time()
    {
        var tickCount = Environment.TickCount;
        Thread.Sleep(500);
        var tickCount2 = Environment.TickCount;
        return tickCount2 - tickCount < 500L;
    }
}