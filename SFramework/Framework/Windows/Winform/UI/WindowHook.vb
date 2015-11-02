Imports System.Runtime.InteropServices
Imports System.Windows.Forms

Namespace Windows.Winform.UI
    Public Class WindowHook
        Inherits NativeWindow
        Implements IHooker

        Private _parent As Form
        Public Filters As New List(Of Filter)

        Sub New(form As Form)
            _parent = form
        End Sub

        Protected Overrides Sub WndProc(ByRef m As Message)
            For Each f As Filter In Filters
                If f.OnProc(m) Then
                    Return
                End If
            Next

            MyBase.WndProc(m)
        End Sub

        Public Function Hook() As Boolean Implements IHooker.Hook
            AssignHandle(_parent.Handle)

            Return True
        End Function

        Public Function UnHook() As Boolean Implements IHooker.UnHook
            ReleaseHandle()

            Return True
        End Function

        Public Function GetLParam(Of T)(lParam As IntPtr) As T Implements IHooker.GetLParam
            Return Marshal.PtrToStructure(lParam, GetType(T))
        End Function
    End Class
End Namespace