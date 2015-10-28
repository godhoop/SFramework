Imports System.Runtime.InteropServices

Namespace API
    Public MustInherit Class KERNEL32
        <DllImport("kernel32.dll")>
        Public Shared Function GetCurrentThreadId() As Integer
        End Function
    End Class
End Namespace
