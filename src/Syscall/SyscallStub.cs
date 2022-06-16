using System;
using System.Collections;
using System.Diagnostics;
using System.Runtime.InteropServices;
using System.Text;
using Anti_Debug_Collection.Structs;

namespace Anti_Debug_Collection.Syscall;

public static class SyscallStub
{
    [DllImport("kernel32", CharSet = CharSet.Ansi, ExactSpelling = true, SetLastError = true)]
    private static extern IntPtr GetProcAddress(IntPtr hModule, string procName);

    [DllImport("ntdll.dll")]
    private static extern void RtlInitUnicodeString(ref UnicodeString destinationString,
        [MarshalAs(UnmanagedType.LPWStr)] string sourceString);

    static unsafe void InitializeObjectAttributes(out OBJECT_ATTRIBUTES initializedAttributes,
        ref UnicodeString objectName, uint attributes, IntPtr rootDirectory, IntPtr securityDescriptor)
    {
        fixed (UnicodeString* objectNamePtr = &objectName)
        {
            initializedAttributes = new OBJECT_ATTRIBUTES
            {
                Length = sizeof(OBJECT_ATTRIBUTES),
                RootDirectory = rootDirectory,
                Attributes = attributes,
                ObjectName = objectNamePtr,
                SecurityDescriptor = securityDescriptor,
                SecurityQualityOfService = IntPtr.Zero
            };
        }
    }

    private static int RemapModule(string moduleName, ref IntPtr moduleBaseAddress)
    {
        const string strKnowDll = "\\KnownDlls\\";

        var usSectionName = new UnicodeString();
        RtlInitUnicodeString(ref usSectionName, strKnowDll + "ntdll.dll");

        InitializeObjectAttributes(out var objAttrib, ref usSectionName, 0x00000040, IntPtr.Zero, IntPtr.Zero);

        var ntDllBase = GetModuleBaseAddress("ntdll.dll");
        var zwOpenSection =
            Marshal.GetDelegateForFunctionPointer<NtApis.ZwOpenSection>(
                GetMappedProcAddress(ntDllBase, "ZwOpenSection"));

        var zwMapViewOfSection =
            Marshal.GetDelegateForFunctionPointer<NtApis.ZwMapViewOfSection>(
                GetMappedProcAddress(ntDllBase, "ZwMapViewOfSection"));
        var ntClose = Marshal.GetDelegateForFunctionPointer<NtApis.NtClose>(GetMappedProcAddress(ntDllBase, "NtClose"));

        var status = zwOpenSection(out var sectionHandle, 0x0004, ref objAttrib);
        if (status != 0)
        {
            return status;
        }

        uint viewSize = 0;
        status = zwMapViewOfSection(sectionHandle, (IntPtr) (-1), ref moduleBaseAddress, UIntPtr.Zero,
            UIntPtr.Zero, out _, out viewSize, 1, 0, 0x02);

        if (!(status >= 0))
        {
            return status;
        }

        if (sectionHandle != IntPtr.Zero)
        {
            status = ntClose(sectionHandle);
            if (!(status >= 0))
            {
                return status;
            }
        }

        return status;
    }

    public static IntPtr GetModuleBaseAddress(string name)
    {
        var hProc = Process.GetCurrentProcess();
        foreach (ProcessModule m in hProc.Modules)
        {
            if (m.ModuleName != null && m.ModuleName.ToUpper().StartsWith(name.ToUpper()))
                return m.BaseAddress;
        }

        return IntPtr.Zero;
    }

    public static unsafe short GetSyscallNumber(string nameModule, string apiName)
    {
        var mappedDll = IntPtr.Zero;
        if (RemapModule(nameModule, ref mappedDll) < 0)
            return 0;

        var baseNtDll = GetModuleBaseAddress("ntdll.dll");
        if (mappedDll == IntPtr.Zero || baseNtDll == IntPtr.Zero)
        {
            return 0;
        }

        var originalFunc = GetMappedProcAddress(mappedDll, apiName);
        var zwUnmapViewOfSection =
            Marshal.GetDelegateForFunctionPointer<NtApis.ZwUnmapViewOfSection>(GetProcAddress(baseNtDll,
                "ZwUnmapViewOfSection"));

        if (originalFunc == IntPtr.Zero)
        {
            zwUnmapViewOfSection((IntPtr) (-1), mappedDll);
            return 0;
        }

        var originalSyscall = *(short*) (originalFunc + 4);
        zwUnmapViewOfSection((IntPtr) (-1), mappedDll);
        return originalSyscall;
    }

    // https://github.com/Mrakovic-ORG/HookDetector.NET/blob/main/src/HookDetector.NET/HookDetector.cs#L62
    public static unsafe IntPtr GetMappedProcAddress(IntPtr moduleBaseAddress, string desiredFunction)
    {
        var eMagicBytes = new byte[2];
        Marshal.Copy(moduleBaseAddress, eMagicBytes, 0, 2);
        var eMagic = Encoding.UTF8.GetString(eMagicBytes);

        if (eMagic != "MZ")
            return IntPtr.Zero;


        var elfaNew = Marshal.ReadInt32(moduleBaseAddress, 0x3c);
        var virtualAddress = Marshal.ReadInt32(moduleBaseAddress + elfaNew + 0x18 + 0x70 + 0x00);

        var addressOfFunctions = Marshal.ReadInt32(moduleBaseAddress + virtualAddress + 0x1c);
        var addressOfNames = Marshal.ReadInt32(moduleBaseAddress + virtualAddress + 0x20);
        var addressOfNameOrdinals = Marshal.ReadInt32(moduleBaseAddress + virtualAddress + 0x24);
        var numberOfNames = Marshal.ReadInt32(moduleBaseAddress + virtualAddress + 0x18);

        var functionTable = (uint*) (moduleBaseAddress + addressOfFunctions);
        var nameTable = (uint*) (moduleBaseAddress + addressOfNames);
        var ordinalTable = (ushort*) (moduleBaseAddress + addressOfNameOrdinals);

        // var functionTable = (uint*) (moduleBaseAddress + (int) exportDirectory.AddressOfFunctions);
        // var nameTable = (uint*) (moduleBaseAddress + (int) exportDirectory.AddressOfNames);
        // var ordinalTable = (ushort*) (moduleBaseAddress + (int) exportDirectory.AddressOfNameOrdinals);

        for (var i = 0; i < numberOfNames; ++i)
        {
            var functionName = Marshal.PtrToStringAnsi(moduleBaseAddress + (int) nameTable[i]);
            if (functionName == desiredFunction)
                return moduleBaseAddress + (int) functionTable[ordinalTable[i]];
        }

        return IntPtr.Zero;
    }
}