Imports System.Windows.Forms
Imports SFramework.API.Struct

Namespace Windows.Winform.UI
    Public Class Filter
        Public Delegate Sub MessageProc(hwnd As IntPtr, ByRef wParam As IntPtr, ByRef lParam As IntPtr, ByRef Result As IntPtr, ByRef cancel As Boolean)

        Private _proc As MessageProc
        Private _msg As WindowsMessages

        Public Sub New(msg As WindowsMessages, proc As MessageProc)
            _proc = proc
            _msg = msg
        End Sub

        Friend Function OnProc(ByRef m As Message) As Boolean
            If m.Msg = _msg Then
                Dim c As Boolean = False
                _proc(m.HWnd, m.WParam, m.LParam, m.Result, c)

                Return c
            End If

            Return False
        End Function
    End Class
End Namespace
