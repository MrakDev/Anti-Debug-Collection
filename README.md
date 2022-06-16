### Anti Debug Collection
This repository contains a collection of anti-debugging tricks that I have collected in C#.

## Methods
#### Flags
- IsDebuggerPresent
- IsRemoteDebugPresent
- ProcessDebugPortFlag
- ProcessDebugObjectHandleFlag
#### Manual Flags
- PEB
- NtGlobalFlag
#### Misc
- ParentProcess
- RaiseException
- HideThreadsFromDebugger

#### Hooks
- HeavenGate (detect ScyllaHide)
- IsBadHookNumberObject (detect ScyllaHide)

#### Syscall
- NtQueryInformationProcess (detect ScyllaHide)

#### Object Handles
- CloseHandle

## Reference and Credits
- [Anti-Debug Tricks](https://anti-debug.checkpoint.com/)
- [Ahora57](https://github.com/Ahora57)
- [Trollicus](https://github.com/Trollicus)