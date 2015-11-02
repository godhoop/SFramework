Imports System.Runtime.InteropServices

Namespace API
    Public MustInherit Class DWMAPI
        <DllImport("dwmapi.dll")>
        Public Shared Function DwmExtendFrameIntoClientArea(ByVal hwnd As IntPtr, ByRef margins As Struct.MARGINS) As Integer
        End Function
    End Class
End Namespace
