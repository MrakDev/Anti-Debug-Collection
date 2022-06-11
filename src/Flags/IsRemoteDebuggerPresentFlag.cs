using System;
using System.Diagnostics;
using System.Runtime.InteropServices;

namespace Anti_Debug_Collection.Flags;

internal static class IsRemoteDebuggerPresentFlag
{
    [DllImport("kernel32.dll", CharSet = CharSet.Auto, ExactSpelling = true)]
    private static extern bool CheckRemoteDebuggerPresent(IntPtr handle, ref bool isDebuggerPresent);

    public static bool CheckRemoteDebuggerPresentFlag()
    {
        var debugStat = false;
        CheckRemoteDebuggerPresent(Process.GetCurrentProcess().Handle, ref debugStat);
        return debugStat;
    }
}