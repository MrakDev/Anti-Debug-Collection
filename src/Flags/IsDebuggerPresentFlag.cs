using System.Runtime.InteropServices;

namespace Anti_Debug_Collection.Flags;

internal static class IsDebuggerPresentFlag
{
    [DllImport("kernel32.dll", CharSet = CharSet.Auto, ExactSpelling = true)]
    private static extern bool IsDebuggerPresent();

    public static bool CheckDebuggerPresentFlag()
    {
        return IsDebuggerPresent();
    }
}