Imports System.Windows.Forms
Imports SFramework.API.Struct

Namespace Windows.Winform.UI
    Public Class Filter
        Public Delegate Sub MessageProc(hwnd As IntPtr, ByRef wParam As IntPtr, ByRef lParam As IntPtr, ByRef Result As IntPtr)

        Private _proc As MessageProc
        Private _msg As WindowsMessages

        Public Sub New(msg As WindowsMessages, proc As MessageProc)
            _proc = proc
            _msg = msg
        End Sub

        Friend Sub OnProc(ByRef m As Message)
            If m.Msg = _msg Then
                _proc(m.HWnd, m.WParam, m.LParam, m.Result)
            End If
        End Sub
    End Class
End Namespace
