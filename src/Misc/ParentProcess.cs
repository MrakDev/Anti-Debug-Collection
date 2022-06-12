using System;
using System.Diagnostics;
using System.Linq;
using System.Runtime.InteropServices;
using Anti_Debug_Collection.Structs;

namespace Anti_Debug_Collection.Misc;

internal static class ParentProcess
{
    [DllImport("ntdll.dll")]
    private static extern int NtQueryInformationProcess(IntPtr processHandle, int processInformationClass,
        ref ParentProcessUtilities processInformation, int processInformationLength, out int returnLength);

    [DllImport("wintrust.dll", ExactSpelling = true, SetLastError = false, CharSet = CharSet.Unicode)]
    private static extern uint WinVerifyTrust(IntPtr hWnd, [MarshalAs(UnmanagedType.LPStruct)] Guid pgActionId,
        WinTrustData pWvtData);

    private static readonly Guid WintrustActionGenericVerifyV2 = new("{00AAC56B-CD44-11d0-8CC2-00C04FC295EE}");

    public static bool IsExplorerParentProcess()
    {
        var actualProcess = Process.GetCurrentProcess();
        var parentProcess = ParentProcesses(actualProcess.Handle);
        if (parentProcess.Id < 1)
            return false;

        if (parentProcess.MainModule is {FileName: { }})
        {
            var file = new WinTrustFileInfo(parentProcess.MainModule.FileName);
            var trustData = new WinTrustData(file);
            var result = WinVerifyTrust(parentProcess.Handle, WintrustActionGenericVerifyV2, trustData);
            try
            {
                if (result != 0)
                    return true;
            }
            finally
            {
                trustData.Dispose();
                file.Dispose();
            }

            var fullFileName = parentProcess.MainModule.FileName;
            var fileNameWithoutPath =
                fullFileName[(fullFileName.LastIndexOf("\\", StringComparison.Ordinal) + 1)..];
            var fileNameWithoutExtension =
                fileNameWithoutPath[..fileNameWithoutPath.LastIndexOf(".", StringComparison.Ordinal)];
            
            if (parentProcess.ProcessName.ToLower() != fileNameWithoutExtension.ToLower())
                return true;
        }
        
        return parentProcess.ProcessName.ToLower() is not ("explorer" or "cmd" or "powershell");
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