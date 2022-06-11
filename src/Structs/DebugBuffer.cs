using System;
using System.Runtime.InteropServices;

namespace Anti_Debug_Collection.Structs;

internal struct RtlProcessHeapInformation
{
    public IntPtr Base;
    public ulong Flags;
}

internal struct RtlProcessHeaps
{
    public ulong NumberOfHeaps;

    [MarshalAs(UnmanagedType.ByValArray, SizeConst = 1)]
    public RtlProcessHeapInformation[] Heaps;
}

[StructLayout(LayoutKind.Sequential)]
internal struct DebugBuffer
{
    public IntPtr SectionHandle;
    public IntPtr SectionBase;
    public IntPtr RemoteSectionBase;
    public IntPtr SectionBaseDelta;
    public IntPtr EventPairHandle;
    public IntPtr RemoteEventPair;
    public IntPtr RemoteProcessId;
    public IntPtr RemoteThreadHandle;
    public int InfoClassMask;
    public IntPtr SizeOfInfo;
    public IntPtr AllocatedSize;
    public IntPtr SectionSize;
    public IntPtr ModuleInformation;
    public IntPtr BackTraceInformation;
    public IntPtr HeapInformation;
    public IntPtr LockInformation;
    public IntPtr SpecificHeap;
    public IntPtr TargetProcessHandle;

    [MarshalAs(UnmanagedType.ByValArray, SizeConst = 6)]
    public IntPtr[] Reserved;
}