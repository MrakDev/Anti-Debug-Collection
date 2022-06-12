using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace Anti_Debug_Collection.Structs
{
    
    [StructLayout(LayoutKind.Sequential, Pack = 0)]
    public unsafe struct OBJECT_ATTRIBUTES
    {
        public int Length;
        public IntPtr RootDirectory;
        public UnicodeString* ObjectName;
        public uint Attributes;
        public IntPtr SecurityDescriptor;
        public IntPtr SecurityQualityOfService;
    }
    
    [StructLayout(LayoutKind.Sequential)]
    public struct ObjectAllInformation
    {
        public int NumberOfObjectsTypes;

        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 1)]
        public ObjectTypeInformation[] ObjectTypeInformation;
    }

    [StructLayout(LayoutKind.Sequential)]
    public struct ObjectTypeInformation
    {
        public UnicodeString Name;
        public int TotalNumberOfObjects;
        public int TotalNumberOfHandles;
        public int TotalPagedPoolUsage;
        public int TotalNonPagedPoolUsage;
        public int TotalNamePoolUsage;
        public int TotalHandleTableUsage;
        public int HighWaterNumberOfObjects;
        public int HighWaterNumberOfHandles;
        public int HighWaterPagedPoolUsage;
        public int HighWaterNonPagedPoolUsage;
        public int HighWaterNamePoolUsage;
        public int HighWaterHandleTableUsage;
        public int InvalidAttributes;
        public GenericMapping GenericMapping;
        public int ValidAccessMask;
        public byte SecurityRequired;
        public byte MaintainHandleCount;
        public int PoolType;
        public int DefaultPagedPoolCharge;
        public int DefaultNonPagedPoolCharge;
    }

    [StructLayout(LayoutKind.Sequential)]
    public struct GenericMapping
    {
        private int GenericRead;
        private int GenericWrite;
        private int GenericExecute;
        private int GenericAll;
    }
}
