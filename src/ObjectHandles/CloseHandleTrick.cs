using System;
using System.Runtime.InteropServices;

namespace Anti_Debug_Collection.ObjectHandles;

internal static class CloseHandleTrick
{
    [DllImport("kernel32.dll", SetLastError = true)]
    private static extern bool CloseHandle(IntPtr hObject);
    
    public static bool IsDebugged()
    {
        try
        {
            CloseHandle((IntPtr)0xDEADBEEF);
            return false;
        }
        catch
        {
            return true;
        }
    }
}