Imports System.ComponentModel
Imports System.Runtime.InteropServices
Imports SFramework.Control

<ListBindable(False)>
<ComVisible(False)>
Public Class VirtualControlCollection
    Inherits List(Of VirtualControlBase)

    ''' <summary>
    ''' <see cref="T:SFramework.Control.VirtualControlCollection"/>을 소유하는 컨트롤을 가져옵니다.
    ''' </summary>
    ''' 
    ''' <returns>
    ''' The <see cref="T:SFramework.Control.VirtualControlBase"/> that owns this <see cref="T:SFramework.Control.VirtualControlCollection"/>.
    ''' </returns>
    Public ReadOnly Property Owner As VirtualControlBase

    Sub New(owner As VirtualControlBase)
        Me.Owner = owner
    End Sub
End Class
