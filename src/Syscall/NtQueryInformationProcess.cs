using Anti_Debug_Collection.Structs;
using System;
using System.Diagnostics;
using System.Runtime.InteropServices;
using System.Security;

namespace Anti_Debug_Collection.Syscall;

public static class NtQueryInformationProcess
{
    [DllImport("kernel32.dll")]
    private static extern bool VirtualProtect(IntPtr lpAddress, uint dwSize, uint flNewProtect, out uint lpflOldProtect);

    private static byte[] _sysCall64 =
    {
        0xB8, 0x0, 0x0, 0x0, 0x0, // mov eax,syscallNumber
        0x4C, 0x8B, 0xD1, // mov r10,rcx
        0x0F, 0x05, // syscall
        0xC3 // retn
    };

    public static unsafe bool IsDebuggerPresent()
    {
        if (IntPtr.Size == 4)
            return false;
        
        uint debugFlag = 0;
        var safeByte = new byte[20];

        var syscallNumberQueryInformationProcess = SyscallStub.GetSyscallNumber("ntdll", "NtQueryInformationProcess");
        var syscallNumberQueryInformationProcessPointer = __makeref(syscallNumberQueryInformationProcess);

        var addressApi =
            SyscallStub.GetMappedProcAddress(SyscallStub.GetModuleBaseAddress("ntdll.dll"), "NtAddBootEntry");
        if (addressApi == IntPtr.Zero)
            return false;

        VirtualProtect(addressApi, 0x1024, 0x40, out var oldProtect);
        Marshal.Copy(*(IntPtr*) &syscallNumberQueryInformationProcessPointer, _sysCall64, 1, 2);
        Marshal.Copy(addressApi, safeByte, 0, safeByte.Length);
        Marshal.Copy(_sysCall64, 0, addressApi, _sysCall64.Length);

        var ntQueryInformationProcess =
            Marshal.GetDelegateForFunctionPointer<NtApis.NtQueryInformationProcess>(addressApi);

        try
        {
            var status = ntQueryInformationProcess((IntPtr) (-1), 0x1f, ref debugFlag, (uint) sizeof(uint), IntPtr.Zero);
            if (status >= 0 && debugFlag == 0)
            {
                return true;
            }
        }
        finally
        {
            VirtualProtect(addressApi, 0x1024, oldProtect, out oldProtect);
        }

        return false;
    }
}