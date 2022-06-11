using System;
using Anti_Debug_Collection.Flags;
using Anti_Debug_Collection.Flags.Manual;
using Anti_Debug_Collection.Misc;
using Anti_Debug_Collection.ObjectHandles;

namespace Anti_Debug_Collection;

internal class Program
{
    private static void Main(string[] args)
    {
        while (true)
        {
            Console.WriteLine("Flags:");
            Console.WriteLine($"\tIsDebuggerPresent: {IsDebuggerPresentFlag.CheckDebuggerPresentFlag()}");
            Console.WriteLine($"\tIsRemoteDebugPresent: {IsRemoteDebuggerPresentFlag.CheckRemoteDebuggerPresentFlag()}");
            Console.WriteLine($"\tProcessDebugPortFlag: {ProcessDebugPortFlag.CheckProcessDebugPort()}");
            Console.WriteLine($"\tProcessDebugObjectHandleFlag: {ProcessDebugObjectHandleFlag.CheckProcessDebugObjectHandle()}");
            
            Console.WriteLine("Manual Flags:");
            Console.WriteLine($"\tPEB: {PEBBeingDebugged.CheckPeb()}");
            Console.WriteLine($"\tNtGlobalFlag: {NtGlobalFlag.CheckGlobalFlag()}");
            
            Console.WriteLine("Misc:");
            Console.WriteLine($"\tParentProcess Trigger: {ParentProcess.IsExplorerParentProcess()}");
            Console.WriteLine($"\tRaiseExceptionTrick: {RaiseExceptionTrick.IsDebugged()}");
            Console.WriteLine($"\tHeavenGate (ScyllaHide): {ScyllaHide.CheckForPatchedMemory()}");
            
            Console.WriteLine("Object Handles:");
            Console.WriteLine($"\tCloseHandleTrick: {CloseHandleTrick.IsDebugged()}");
            Console.ReadKey();
            Console.Clear();
        }
    }
}