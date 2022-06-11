using System;
using System.Diagnostics;
using System.Runtime.InteropServices;
using Anti_Debug_Collection.Structs;

namespace Anti_Debug_Collection.Misc;

internal static class ParentProcess
{
    [DllImport("ntdll.dll")]
    private static extern int NtQueryInformationProcess(IntPtr processHandle, int processInformationClass,
        ref ParentProcessUtilities processInformation, int processInformationLength, out int returnLength);

    public static bool IsExplorerParentProcess()
    {
        var actualProcess = Process.GetCurrentProcess();
        var parentProcess = ParentProcesses(actualProcess.Handle);
        return !(parentProcess.Id > 0 && parentProcess.ProcessName.ToLower() is "explorer" or "cmd" or "powershell");
    }

    private static Process ParentProcesses(IntPtr processHandle)
    {
        var pbi = new ParentProcessUtilities();
        var status = NtQueryInformationProcess(processHandle, 0, ref pbi, Marshal.SizeOf(pbi), out var returnLength);
        if (status != 0)
            return default;
        try
        {
            return Process.GetProcessById(pbi.InheritedFromUniqueProcessId.ToInt32());
        }
        catch (ArgumentException)
        {
            return default;
        }
    }
}