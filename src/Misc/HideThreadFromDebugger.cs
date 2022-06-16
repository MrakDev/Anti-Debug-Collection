using System;
using System.Diagnostics;
using static Anti_Debug_Collection.Structs.HTFD;

namespace Anti_Debug_Collection.Misc;

public class HideThreadFromDebugger
{
    public static void HideThreadsFromDebugger()
    {
        ProcessThreadCollection currentThreads = Process.GetCurrentProcess().Threads;

        foreach (ProcessThread thread in currentThreads)
        {
            IntPtr hProc = OpenThread(ThreadAccess.SET_INFORMATION, false, (uint)thread.Id);

            NtStatus status = NtSetInformationThread(hProc,
                ThreadInformationClass.ThreadHideFromDebugger, /* Hide Threads from the Debugger */
                IntPtr.Zero, 0);

            if (status == NtStatus.Success)
            {
                Console.WriteLine($"{thread.Id} hidden from debugger.\n");
            }
        }
    }
}