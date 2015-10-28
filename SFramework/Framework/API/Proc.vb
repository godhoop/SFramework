Namespace API
    Public MustInherit Class Proc
        Delegate Sub WinEventDelegate(ByVal hWinEventHook As IntPtr, ByVal eventType As UInteger, ByVal hwnd As IntPtr, ByVal idObject As Integer, ByVal idChild As Integer, ByVal dwEventThread As UInteger, ByVal dwmsEventTime As UInteger)
        Delegate Function HookProc(ByVal code As Integer, ByVal wParam As IntPtr, lParam As IntPtr) As Integer
    End Class
End Namespace
