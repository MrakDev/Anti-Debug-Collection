using System;
using System.Runtime.InteropServices;

namespace Anti_Debug_Collection.Structs;

[StructLayout(LayoutKind.Sequential)]
public struct ParentProcessUtilities
{
    public IntPtr Reserved1;
    public IntPtr PebBaseAddress;
    public IntPtr Reserved2_0;
    public IntPtr Reserved2_1;
    public IntPtr UniqueProcessId;
    public IntPtr InheritedFromUniqueProcessId;
}