Imports System.Runtime.InteropServices
Imports SFramework.API.Proc
Imports SFramework.API.Struct

Namespace API
    Public MustInherit Class USER32
        Public Declare Auto Function IsIconic Lib "user32.dll" (ByVal hwnd As IntPtr) As Boolean

        <DllImport("user32.dll", SetLastError:=True)>
        Public Shared Function SetWindowPos(ByVal hWnd As IntPtr, ByVal hWndInsertAfter As IntPtr, ByVal X As Integer, ByVal Y As Integer, ByVal cx As Integer, ByVal cy As Integer, ByVal uFlags As SetWindowPosFlags) As Boolean
        End Function

        <DllImport("user32")>
        Public Shared Function GetMonitorInfo(hMonitor As IntPtr, lpmi As MONITORINFO) As Boolean
        End Function

        <DllImport("user32")>
        Public Shared Function MonitorFromWindow(handle As IntPtr, flags As Integer) As IntPtr
        End Function

        <DllImport("user32.dll", SetLastError:=True)>
        Public Shared Function FindWindow(lpClassName As String, lpWindowName As String) As IntPtr
        End Function

        <DllImport("user32.dll", SetLastError:=True)>
        Public Shared Function GetWindowThreadProcessId(ByVal hwnd As IntPtr,
                                                 ByRef lpdwProcessId As Integer) As Integer
        End Function

        <DllImport("user32.dll", EntryPoint:="SetWindowsHookExA")>
        Public Shared Function SetWindowsHookEx(ByVal hookType As HookType, ByVal lpfn As HookProc, ByVal hMod As IntPtr, ByVal dwThreadId As UInteger) As IntPtr
        End Function

        <DllImport("user32.dll", EntryPoint:="CallNextHookEx")>
        Public Shared Function CallNextHookEx(hHook As IntPtr, nCode As Integer, wParam As IntPtr, lParam As IntPtr) As IntPtr
        End Function

        <DllImport("user32.dll", EntryPoint:="SetWinEventHook")>
        Public Shared Function SetWinEventHook(ByVal eventMin As UInteger, ByVal eventMax As UInteger, ByVal hmodWinEventProc As IntPtr, ByVal lpfnWinEventProc As WinEventDelegate, ByVal idProcess As UInteger, ByVal idThread As UInteger, ByVal dwFlags As UInteger) As IntPtr
        End Function

        <DllImport("user32.dll")>
        Public Shared Function UnhookWinEvent(hWinEventHook As IntPtr) As Boolean
        End Function

        <DllImport("user32.dll")>
        Public Shared Function SetWindowLong(hWnd As IntPtr, <MarshalAs(UnmanagedType.I4)> nIndex As WindowLongFlags, dwNewLong As IntPtr) As Integer
        End Function

        <DllImport("user32.dll", SetLastError:=True)>
        Public Shared Function GetWindowLong(hWnd As IntPtr, <MarshalAs(UnmanagedType.I4)> nIndex As WindowLongFlags) As Integer
        End Function

        <DllImport("user32.dll", SetLastError:=True, CharSet:=CharSet.Auto)>
        Public Shared Function SendMessage(ByVal hWnd As IntPtr, ByVal Msg As UInteger, ByVal wParam As IntPtr, ByVal lParam As IntPtr) As IntPtr
        End Function

        <DllImport("user32.dll", SetLastError:=True, CharSet:=CharSet.Auto)>
        Public Shared Function PostMessage(ByVal hWnd As IntPtr, ByVal Msg As UInteger, ByVal wParam As IntPtr, ByVal lParam As IntPtr) As Boolean
        End Function

        <DllImport("user32.dll")>
        Public Shared Function DefWindowProc(ByVal hWnd As IntPtr, ByVal uMsg As WindowsMessages, ByVal wParam As IntPtr, ByVal lParam As IntPtr) As IntPtr
        End Function

        <DllImport("user32.dll")>
        Public Shared Function ReleaseCapture() As Boolean
        End Function
    End Class
End Namespace
