using System;

namespace Anti_Debug_Collection.Structs;

public class NtApis
{
    public delegate int ZwOpenSection(out IntPtr sectionHandle, uint desiredAccess,
        ref OBJECT_ATTRIBUTES objectAttributes);

    public delegate int ZwMapViewOfSection(IntPtr sectionHandle, IntPtr processHandle, ref IntPtr baseAddress,
        UIntPtr zeroBits, UIntPtr commitSize, out ulong sectionOffset, out uint viewSize, uint inheritDisposition,
        uint allocationType, uint win32Protect);

    public delegate int ZwUnmapViewOfSection(IntPtr processHandle, IntPtr baseAddress);

    public delegate int NtClose(IntPtr handle);

    public delegate int NtQueryInformationProcess(IntPtr processHandle, int processInformationClass, ref uint processInformation, uint processInformationLength, IntPtr returnLength);
}