Public Interface IHooker
    Function Hook() As Boolean
    Function UnHook() As Boolean
    Function GetLParam(Of T)(lParam As IntPtr) As T
End Interface
