Imports System.Drawing

Namespace Windows.Winform.UI.Skin
    Public Class EventRectangle
        Public Property Tag As Object

        Private _Rect As Rectangle
        Private _handler As Action(Of Object, Object)

        Private _isEnabled As Boolean = True

        Public Property IsEnabled As Boolean
            Get
                Return _isEnabled
            End Get
            Set(value As Boolean)
                _isEnabled = value
            End Set
        End Property

        Public Property Rect As Rectangle
            Get
                Return _Rect
            End Get
            Set(value As Rectangle)
                _Rect = value
            End Set
        End Property

        Public Property Handler As Action(Of Object, Object)
            Get
                Return _handler
            End Get
            Set(value As Action(Of Object, Object))
                _handler = value
            End Set
        End Property

        Sub New(rect As Rectangle, action As Action(Of Object, Object))
            Me.Rect = rect
            Handler = action
        End Sub

        Public Property X As Integer
            Get
                Return _Rect.X
            End Get
            Set(value As Integer)
                _Rect.X = value
            End Set
        End Property

        Public Property Top As Integer
            Get
                Return _Rect.Top
            End Get
            Set(value As Integer)
                _Rect.Y = value
            End Set
        End Property

        Public Property Right As Integer
            Get
                Return _Rect.Right
            End Get
            Set(value As Integer)
                _Rect.X = value - _Rect.Width
            End Set
        End Property

        Public Property Bottom As Integer
            Get
                Return _Rect.Bottom
            End Get
            Set(value As Integer)
                _Rect.Y = value - _Rect.Height
            End Set
        End Property

        Public Property Y As Integer
            Get
                Return _Rect.Y
            End Get
            Set(value As Integer)
                _Rect.Y = value
            End Set
        End Property

        Public Property Width As Integer
            Get
                Return _Rect.Width
            End Get
            Set(value As Integer)
                _Rect.Width = value
            End Set
        End Property

        Public Property Height As Integer
            Get
                Return _Rect.Height
            End Get
            Set(value As Integer)
                _Rect.Height = value
            End Set
        End Property

        Public Property Location As Point
            Get
                Return _Rect.Location
            End Get
            Set(value As Point)
                _Rect.Location = value
            End Set
        End Property

        Public Property Size As Size
            Get
                Return _Rect.Size
            End Get
            Set(value As Size)
                _Rect.Size = value
            End Set
        End Property

    End Class
End Namespace