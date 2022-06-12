using System;
using System.Runtime.InteropServices;

namespace Anti_Debug_Collection.Structs;

[StructLayout(LayoutKind.Sequential)]
public struct UnicodeString : IDisposable
{
    public ushort Length;
    public ushort MaximumLength;
    public IntPtr buffer;

    public UnicodeString(string s)
    {
        Length = (ushort) (s.Length * 2);
        MaximumLength = (ushort) (Length + 2);
        buffer = Marshal.StringToHGlobalUni(s);
    }

    public void Dispose()
    {
        Marshal.FreeHGlobal(buffer);
        buffer = IntPtr.Zero;
    }

    public override string ToString()
    {
        return Marshal.PtrToStringUni(buffer);
    }
}