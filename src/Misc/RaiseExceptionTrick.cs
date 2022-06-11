using System;
using System.Runtime.InteropServices;

namespace Anti_Debug_Collection.Misc;

internal static class RaiseExceptionTrick
{
    [DllImport("kernel32.dll")]
    private static extern bool RaiseException(uint dwExceptionCode, uint dwExceptionFlags, uint nNumberOfArguments,
        IntPtr lpArguments);

    [DllImport("kernel32.dll")]
    private static extern uint GetLastError();

    public static bool IsDebugged()
    {
        try
        {
            RaiseException(0x40010006, 0, 0, IntPtr.Zero);
        }
        catch
        {
            return GetLastError() == 0x40010006;
        }

        return true;
    }
}