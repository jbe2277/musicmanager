using System.Runtime.InteropServices;

namespace Waf.MusicManager.Interop;

public static partial class ThreadExecutionStateHelper
{
    const uint ES_CONTINUOUS = 0x80000000;
    const uint ES_SYSTEM_REQUIRED = 0x00000001;

    [LibraryImport("kernel32.dll"), DefaultDllImportSearchPaths(DllImportSearchPath.System32)]
    private static partial uint SetThreadExecutionState(uint esFlags);
    
    public static void PreventSleep() => SetThreadExecutionState(ES_CONTINUOUS | ES_SYSTEM_REQUIRED);

    public static void RestoreSleep() => SetThreadExecutionState(ES_CONTINUOUS);
}
