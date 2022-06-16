using System;
using Anti_Debug_Collection.Flags;
using Anti_Debug_Collection.Flags.Manual;
using Anti_Debug_Collection.Hook;
using Anti_Debug_Collection.Misc;
using Anti_Debug_Collection.ObjectHandles;
using Anti_Debug_Collection.Syscall;
using static Anti_Debug_Collection.Misc.HideThreadFromDebugger;
using static Anti_Debug_Collection.Misc.Timer;

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
            
            Console.WriteLine("Hook:");
            Console.WriteLine($"\tHeavenGate (ScyllaHide): {ScyllaHide.CheckForPatchedMemory()}");
            Console.WriteLine($"\tIsBadNumberObject: {IsBadNumberObject.IsBadNumberObjectFlag()}");

            Console.WriteLine("Object Handles:");
            Console.WriteLine($"\tCloseHandleTrick: {CloseHandleTrick.IsDebugged()}");
            
            Console.WriteLine("Syscall:");
            Console.WriteLine($"\tNtQueryInformationProcess: {NtQueryInformationProcess.IsDebuggerPresent()}");

            Console.WriteLine("HideThreadFromDebugger:");
            HideThreadsFromDebugger();

            Console.WriteLine("Timer:");
            Console.WriteLine($"\nIsDebuggerSlow: {TimerCheck()}");
            
            Console.ReadKey();
            Console.Clear();
        }
    }
}
