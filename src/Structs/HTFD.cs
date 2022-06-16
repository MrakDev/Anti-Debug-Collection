using System;
using System.Runtime.InteropServices;

namespace Anti_Debug_Collection.Structs;

public class HTFD
{
    [Flags]
    public enum NtStatus : uint
    {
        // Success
        Success = 0x00000000
    }
    
    [DllImport("kernel32.dll", SetLastError = true)]
    public static extern IntPtr OpenThread(ThreadAccess dwDesiredAccess, bool bInheritHandle,
        uint dwThreadId);
    
    [Flags]
    public enum ThreadAccess : int
    {
        SET_INFORMATION = (0x0020)
    }
    
    [DllImport("ntdll.dll")]
    public static extern NtStatus NtSetInformationThread(IntPtr ThreadHandle,
        ThreadInformationClass threadInformationClass, IntPtr ThreadInformation, ulong ThreadInformationLenght);
    
    [Flags]
    public enum ThreadInformationClass
    {
        ThreadHideFromDebugger = 17
    }
}