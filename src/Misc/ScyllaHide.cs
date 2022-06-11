using System;
using System.Diagnostics;
using System.Runtime.InteropServices;

namespace Anti_Debug_Collection.Misc;

public class ScyllaHide
{
    private delegate IntPtr PebAddress();

    [DllImport("kernel32.dll")]
    private static extern bool VirtualProtectEx(IntPtr hProcess, IntPtr lpAddress, UIntPtr dwSize,
        uint flNewProtect, out uint lpflOldProtect);

    [StructLayout(LayoutKind.Sequential)]
    public unsafe struct _TEB
    {
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 12)]
        public IntPtr[] Reserved1;
        public IntPtr ProcessEnvironmentBlock;
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 399)]
        public IntPtr[] Reserved2;
        public fixed byte Reserved3[1952];
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 64)]
        public IntPtr[] TlsSlots;
        public fixed byte Reserved4[8];
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 26)]
        public IntPtr[] Reserved5;
        public IntPtr ReservedForOle;
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 4)]
        public IntPtr[] Reserved6;
        public IntPtr TlsExpansionSlots;

        public static _TEB Create(IntPtr tebAddress)
        {
            _TEB teb = (_TEB)Marshal.PtrToStructure(tebAddress, typeof(_TEB));
            return teb;
        }
    }

    public static unsafe bool CheckForPatchedMemory()
    {
        byte[] assembledCode;
        if (IntPtr.Size == 8)
            assembledCode = new byte[]
            {
                0x65, 0x48, 0x8B, 0x04, 0x25, 0x30, 0x00, // mov rax, QWORD PTR gs:0x30 
                0x00, 0x00, 0xC2, 0x00, 0x00 // ret 0x0
            };
        else
            assembledCode = new byte[]
            {
                0x64, 0xA1, 0x18, 0x00, 0x00, 0x00, // mov eax,fs:0x18
                0xC2, 0x00, 0x00 // ret 0x0
            };

        fixed (byte* ptr = assembledCode)
        {
            var memoryAddress = (IntPtr)ptr;

            if (!VirtualProtectEx(Process.GetCurrentProcess().Handle, memoryAddress, (UIntPtr)assembledCode.Length,
                    0x40 /* EXECUTE_READWRITE */, out _))
                return false;

            var tebAddress = Marshal.GetDelegateForFunctionPointer<PebAddress>(memoryAddress);

            IntPtr gateAddress = IntPtr.Zero;
            if (IntPtr.Size == 8)
                gateAddress = Marshal.ReadIntPtr(tebAddress(), 256); // WOW64Reserved
            else
                gateAddress = Marshal.ReadIntPtr(tebAddress(), 192); // WOW32Reserved

            if (gateAddress == IntPtr.Zero)
                return false;

            var gateJumpOffset = Marshal.ReadByte(gateAddress + 0x05);
            return gateJumpOffset != 51;
        }
    }
}