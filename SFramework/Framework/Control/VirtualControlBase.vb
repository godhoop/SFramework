Imports System.ComponentModel

Namespace Control
    Public Class VirtualControlBase
        Implements IVirtualControl

        Private _isEnabled As Boolean = True

        Public Property Childs As VirtualControlCollection Implements IVirtualControl.Childs

        Public Property IsEnabled As Boolean Implements IVirtualControl.IsEnabled
            Get
                Return _isEnabled
            End Get
            Set(value As Boolean)
                _isEnabled = value
            End Set
        End Property

        Sub New()
            Childs = New VirtualControlCollection(Me)
        End Sub
    End Class

End Namespace