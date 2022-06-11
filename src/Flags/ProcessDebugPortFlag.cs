using System;
using System.Diagnostics;
using System.Runtime.InteropServices;

namespace Anti_Debug_Collection.Flags;

internal static class ProcessDebugPortFlag
{
    [DllImport("ntdll.dll", SetLastError = true)]
    private static extern int NtQueryInformationProcess(IntPtr processHandle, int processInformationClass,
        out IntPtr processInformation, int processInformationLength, IntPtr returnLength);

    public static bool CheckProcessDebugPort()
    {
        var status = NtQueryInformationProcess
        (
            Process.GetCurrentProcess().Handle,
            7 /* 7 = ProcessDebugPort */,
            out var flProcessDebugPort,
            IntPtr.Size,
            IntPtr.Zero
        );
        return status == 0 && (IntPtr) (-1) == flProcessDebugPort;
    }
}