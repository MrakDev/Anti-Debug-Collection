using System;
using System.Runtime.InteropServices;

namespace Anti_Debug_Collection.Structs;

[StructLayout(LayoutKind.Sequential, CharSet = CharSet.Unicode)]
public struct WinTrustFileInfo
{
    private uint StructSize = (uint) Marshal.SizeOf(typeof(WinTrustFileInfo));
    private IntPtr pszFilePath;
    private IntPtr hFile = IntPtr.Zero;
    private IntPtr pgKnownSubject = IntPtr.Zero;

    public WinTrustFileInfo(string filePath)
    {
        pszFilePath = Marshal.StringToCoTaskMemAuto(filePath);
    }

    public void Dispose()
    {
        if (pszFilePath == IntPtr.Zero) return;
        Marshal.FreeCoTaskMem(pszFilePath);
        pszFilePath = IntPtr.Zero;
    }
}

[StructLayout(LayoutKind.Sequential, CharSet = CharSet.Unicode)]
public struct WinTrustData
{
    private uint StructSize = (uint) Marshal.SizeOf(typeof(WinTrustData));
    private IntPtr PolicyCallbackData = IntPtr.Zero;
    private IntPtr SIPClientData = IntPtr.Zero;
    private uint UIChoice = 2;
    private uint RevocationChecks = 0;
    private uint UnionChoice = 1;
    private IntPtr FileInfoPtr;
    private uint StateAction = 0;
    private IntPtr StateData = IntPtr.Zero;
    private string URLReference = null;
    private uint ProvFlags = 0x00000080;
    private uint UIContext = 0;

    public WinTrustData(WinTrustFileInfo fileInfo)
    {
        if (Environment.OSVersion.Version.Major > 6 ||
            (Environment.OSVersion.Version.Major == 6 && Environment.OSVersion.Version.Minor > 1) ||
            (Environment.OSVersion.Version.Major == 6 && Environment.OSVersion.Version.Minor == 1 &&
             !string.IsNullOrEmpty(Environment.OSVersion.ServicePack)))
        {
            ProvFlags |= 0x00002000;
        }

        FileInfoPtr = Marshal.AllocCoTaskMem(Marshal.SizeOf(typeof(WinTrustFileInfo)));
        Marshal.StructureToPtr(fileInfo, FileInfoPtr, false);
    }

    public void Dispose()
    {
        if (FileInfoPtr == IntPtr.Zero) return;
        Marshal.FreeCoTaskMem(FileInfoPtr);
        FileInfoPtr = IntPtr.Zero;
    }
}