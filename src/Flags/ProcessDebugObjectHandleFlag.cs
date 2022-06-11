using System;
using System.Diagnostics;
using System.Runtime.InteropServices;

namespace Anti_Debug_Collection.Flags;

internal static class ProcessDebugObjectHandleFlag
{
    [DllImport("ntdll.dll", SetLastError = true)]
    private static extern int NtQueryInformationProcess(IntPtr processHandle, int processInformationClass,
        out IntPtr processInformation, int processInformationLength, IntPtr returnLength);

    public static bool CheckProcessDebugObjectHandle()
    {
        var status = NtQueryInformationProcess
        (
            Process.GetCurrentProcess().Handle,
            0x1e /* ProcessDebugObjectHandle */,
            out var flProcessDebugObject,
            IntPtr.Size,
            IntPtr.Zero
        );
        return status == 0 && (IntPtr) 0 != flProcessDebugObject;
    }
}