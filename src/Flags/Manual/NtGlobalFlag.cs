﻿using System;
using System.Diagnostics;
using System.Runtime.InteropServices;

namespace Anti_Debug_Collection.Flags.Manual;

internal static class NtGlobalFlag
{
    private delegate IntPtr PebAddress();

    [DllImport("kernel32.dll")]
    private static extern bool VirtualProtectEx(IntPtr hProcess, IntPtr lpAddress, UIntPtr dwSize,
        uint flNewProtect, out uint lpflOldProtect);

    public static unsafe bool CheckGlobalFlag()
    {
        byte[] assembledCode;
        if (IntPtr.Size == 8)
            assembledCode = new byte[]
            {
                0x65, 0x48, 0x8B, 0x04, 0x25, 0x60, // mov    rax, QWORD PTR gs:0x60
                0x00, 0x00, 0x00, 0xC2, 0x00, 0x00  // ret    0x0
            };
        else
            assembledCode = new byte[]
            {
                0x64, 0xA1, 0x30, 0x00, 0x00, 0x00, // mov    eax, fs:0x30
                0xC2, 0x00, 0x00                    // ret    0x0
            };

        fixed (byte* ptr = assembledCode)
        {
            var memoryAddress = (IntPtr) ptr;

            if (!VirtualProtectEx(Process.GetCurrentProcess().Handle, memoryAddress, (UIntPtr) assembledCode.Length,
                    0x40 /* EXECUTE_READWRITE */, out _))
                return false;

            var pebAddress = Marshal.GetDelegateForFunctionPointer<PebAddress>(memoryAddress);
            var dwNtGlobalFlag =  Marshal.ReadByte(pebAddress(), IntPtr.Size == 8 ? 0x68 : 0xBC);
            if ((dwNtGlobalFlag & (0x10 | 0x20 | 0x40)) != 0)
                return true;
            return false;
        }
    }
}