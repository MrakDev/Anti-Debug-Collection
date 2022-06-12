using System;
using System.Runtime.InteropServices;
using Anti_Debug_Collection.Structs;

namespace Anti_Debug_Collection.Hook;

public static class IsBadNumberObject
{
    [DllImport("ntdll.dll")]
    private static extern int NtCreateDebugObject(out IntPtr debugHandle, int desiredAccess,
        ref OBJECT_ATTRIBUTES objectAttributes, int flags);

    [DllImport("ntdll.dll")]
    private static extern int NtQueryObject(IntPtr objectHandle, int informationClass, ref IntPtr informationPtr,
        uint informationLength, ref IntPtr returnLength);

    [DllImport("ntdll.dll")]
    private static extern int NtQueryObject(IntPtr objectHandle, int informationClass, IntPtr informationPtr,
        uint informationLength, ref IntPtr returnLength);

    [DllImport("kernel32.dll")]
    private static extern IntPtr VirtualAlloc(IntPtr lpAddress, uint dwSize, uint flAllocationType, uint flProtect);

    [DllImport("ntdll.dll", ExactSpelling = true, SetLastError = false)]
    private static extern int NtClose(IntPtr hObject);

    [DllImport("kernel32.dll", ExactSpelling = true, SetLastError = false)]
    private static extern bool VirtualFree(IntPtr lpAddress, uint dwSize, uint dwFreeType);

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

    public static unsafe bool IsBadNumberObjectFlag()
    {
        var maxNumberOfObjects = 0;
        var objectName = new UnicodeString();
        InitializeObjectAttributes(out var objectAttribute, ref objectName, 0, IntPtr.Zero, IntPtr.Zero);
        var status = NtCreateDebugObject(out var debugObject, 0x1F000F, ref objectAttribute, 0);
        if (status >= 0)
        {
            var length = IntPtr.Zero;
            status = NtQueryObject(IntPtr.Zero, 3, ref length, sizeof(ulong), ref length);
            var buffer = VirtualAlloc(IntPtr.Zero, (uint)length, 0x00002000 | 0x00001000, 0x04);
            if (buffer == IntPtr.Zero)
            {
                _ = NtClose(debugObject);
                return false;
            }

            status = NtQueryObject((IntPtr)(-1), 3, buffer, (uint)length, ref length);
            if (!(status >= 0))
            {
                _ = NtClose(debugObject);
                VirtualFree(buffer, (uint)length, 0x8000);
                return false;
            }

            var objectAllInfo = Marshal.PtrToStructure<ObjectAllInformation>(buffer);
            var pinnedArray = GCHandle.Alloc(objectAllInfo.ObjectTypeInformation[0], GCHandleType.Pinned);
            var objInfoLocation = (byte*)pinnedArray.AddrOfPinnedObject();

            var a = sizeof(ObjectTypeInformation);

            for (var i = 0; i < objectAllInfo.NumberOfObjectsTypes; ++i)
            {
                var objectTypeInfo = *(ObjectTypeInformation*)objInfoLocation;
                if (objectTypeInfo.Name.ToString() == "DebugObject")
                {
                    if (objectTypeInfo.TotalNumberOfObjects > 0)
                    {
                        maxNumberOfObjects += objectTypeInfo.TotalNumberOfObjects;
                    }
                }

                objInfoLocation = (byte*)objectTypeInfo.Name.buffer;
                objInfoLocation += objectTypeInfo.Name.MaximumLength;

                var tmp = (ulong)objInfoLocation & (ulong)-sizeof(void*);
                if (tmp != (ulong)objInfoLocation)
                    tmp += (ulong)sizeof(void*);

                objInfoLocation = (byte*)tmp;
            }

            pinnedArray.Free();
            VirtualFree(buffer, 0, 0x00008000);
            _ = NtClose(debugObject);
            return maxNumberOfObjects < 1;
        }

        return false;
    }


}