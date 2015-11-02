Imports System.Drawing
Imports System.Windows.Forms
Imports SFramework.Control

Namespace Windows.Winform.UI.Skin
    Public Class MouseCaptureControl
        Inherits VirtualControlBase

        Public Event PreviewMouseEvent(sender As Object, arg As MouseEventArgs, e As MouseEvent)

        Public Event MouseMove(sender As Object, e As MouseEventArgs)
        Public Event MouseDown(sender As Object, e As MouseEventArgs)
        Public Event MouseUp(sender As Object, e As MouseEventArgs)
        Public Event MouseEnter(sender As Object, e As MouseEventArgs)
        Public Event MouseLeave(sender As Object, e As MouseEventArgs)
        Public Event MouseClick(sender As Object, e As MouseEventArgs)
        Public Event MouseDoubleClick(sender As Object, e As MouseEventArgs)

        Private _rect As EventRectangle

        Sub New(area As Rectangle)
            _rect = New EventRectangle(area, AddressOf OnEvent)
        End Sub

        Public Overridable Sub OnEvent(ea As MouseEventArgs, e As MouseEvent)
            If IsEnabled Then
                RaiseEvent PreviewMouseEvent(Me, ea, e)

                Select Case e
                    Case MouseEvent.Move
                        RaiseEvent MouseMove(Me, ea)
                    Case MouseEvent.Down
                        RaiseEvent MouseDown(Me, ea)
                    Case MouseEvent.Up
                        RaiseEvent MouseUp(Me, ea)
                    Case MouseEvent.Enter
                        RaiseEvent MouseEnter(Me, ea)
                    Case MouseEvent.Leave
                        RaiseEvent MouseLeave(Me, ea)
                    Case MouseEvent.Click
                        RaiseEvent MouseClick(Me, ea)
                    Case MouseEvent.DoubleClick
                        RaiseEvent MouseDoubleClick(Me, ea)
                End Select
            End If
        End Sub

        Public Property Rect As Rectangle
            Get
                Return _rect.Rect
            End Get
            Set(value As Rectangle)
                _rect.X = value.X
                _rect.Y = value.Y
                _rect.Width = value.Width
                _rect.Height = value.Height
            End Set
        End Property

        Public ReadOnly Property EventRect As EventRectangle
            Get
                Return _rect
            End Get
        End Property
    End Class
End Namespace