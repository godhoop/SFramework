Imports System.ComponentModel
Imports SFramework.Windows.Winform.UI.Skin

Namespace Control
    Public Class VirtualControlBase
        Implements IVirtualControl

        Private _isEnabled As Boolean = True
        Private _mState As MouseState = MouseState.None

        Public Property Childs As VirtualControlCollection Implements IVirtualControl.Childs

        Public Property IsEnabled As Boolean Implements IVirtualControl.IsEnabled
            Get
                Return _isEnabled
            End Get
            Set(value As Boolean)
                _isEnabled = value
            End Set
        End Property

        Protected Friend Property MouseState As MouseState Implements IVirtualControl.MouseState
            Get
                Return _mState
            End Get
            Set(value As MouseState)
                _mState = value
            End Set
        End Property

        Sub New()
            Childs = New VirtualControlCollection(Me)
        End Sub
    End Class

End Namespace