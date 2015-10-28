Imports System.Runtime.InteropServices

Public Class BaseHooker
    Implements IHooker

    Public Function GetLParam(Of T)(lParam As IntPtr) As T Implements IHooker.GetLParam
        Return Marshal.PtrToStructure(lParam, GetType(T))
    End Function

    Public Overridable Function Hook() As Boolean Implements IHooker.Hook
        Throw New NotImplementedException()
    End Function

    Public Overridable Function UnHook() As Boolean Implements IHooker.UnHook
        Throw New NotImplementedException()
    End Function
End Class