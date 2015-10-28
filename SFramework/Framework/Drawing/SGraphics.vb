Imports System.Drawing
Imports System.Drawing.Imaging

Namespace Drawing
    Public Class SGraphics
        Implements IDisposable

        Private _g As Graphics

        Sub New(g As Graphics)
            Me._g = g
        End Sub

        Public ReadOnly Property Graphic As Graphics
            Get
                Return _g
            End Get
        End Property

        Public Sub Clear(color As Color)
            _g.Clear(color)
        End Sub

#Region " [ DrawLine ] "
        Public Sub DrawLine(color As Color, pt1 As Point, pt2 As Point)
            Using sb As New SolidBrush(color)
                Using p As New Pen(sb)
                    _g.DrawLine(p, pt1, pt2)
                End Using
            End Using
        End Sub

        Public Sub DrawLine(color As Color, pt1 As PointF, pt2 As PointF)
            Using sb As New SolidBrush(color)
                Using p As New Pen(sb)
                    _g.DrawLine(p, pt1, pt2)
                End Using
            End Using
        End Sub

        Public Sub DrawLine(color As Color, x1 As Integer, y1 As Integer, x2 As Integer, y2 As Integer)
            Using sb As New SolidBrush(color)
                Using p As New Pen(sb)
                    _g.DrawLine(p, x1, y1, x2, y2)
                End Using
            End Using
        End Sub

        Public Sub DrawLine(color As Color, x1 As Single, y1 As Single, x2 As Single, y2 As Single)
            Using sb As New SolidBrush(color)
                Using p As New Pen(sb)
                    _g.DrawLine(p, x1, y1, x2, y2)
                End Using
            End Using
        End Sub

#End Region

#Region " [ DrawLines ] "
        Public Sub DrawLines(color As Color, points() As Point)
            Using sb As New SolidBrush(color)
                Using p As New Pen(sb)
                    _g.DrawLines(p, points)
                End Using
            End Using
        End Sub

        Public Sub DrawLines(color As Color, points() As PointF)
            Using sb As New SolidBrush(color)
                Using p As New Pen(sb)
                    _g.DrawLines(p, points)
                End Using
            End Using
        End Sub
#End Region

#Region " [ DrawRectangle ] "
        Public Sub DrawRectangle(color As Color, rect As Rectangle)
            Using sb As New SolidBrush(color)
                Using p As New Pen(sb)
                    _g.DrawRectangle(p, rect)
                End Using
            End Using
        End Sub

        Public Sub DrawRectangle(color As Color, x As Integer, y As Integer, width As Integer, height As Integer)
            Using sb As New SolidBrush(color)
                Using p As New Pen(sb)
                    _g.DrawRectangle(p, x, y, width, height)
                End Using
            End Using
        End Sub

        Public Sub DrawRectangle(color As Color, x As Single, y As Single, width As Single, height As Single)
            Using sb As New SolidBrush(color)
                Using p As New Pen(sb)
                    _g.DrawRectangle(p, x, y, width, height)
                End Using
            End Using
        End Sub

        Public Sub DrawRectangles(color As Color, rects() As Rectangle)
            Using sb As New SolidBrush(color)
                Using p As New Pen(sb)
                    _g.DrawRectangles(p, rects)
                End Using
            End Using
        End Sub

        Public Sub DrawRectangles(color As Color, rects() As RectangleF)
            Using sb As New SolidBrush(color)
                Using p As New Pen(sb)
                    _g.DrawRectangles(p, rects)
                End Using
            End Using
        End Sub
#End Region

#Region " [ DrawEllipse ] "
        Public Sub DrawEllipse(color As Color, rect As Rectangle)
            Using sb As New SolidBrush(color)
                Using p As New Pen(sb)
                    _g.DrawEllipse(p, rect)
                End Using
            End Using
        End Sub

        Public Sub DrawEllipse(color As Color, rect As RectangleF)
            Using sb As New SolidBrush(color)
                Using p As New Pen(sb)
                    _g.DrawEllipse(p, rect)
                End Using
            End Using
        End Sub

        Public Sub DrawEllipse(color As Color, x As Integer, y As Integer, width As Integer, height As Integer)
            Using sb As New SolidBrush(color)
                Using p As New Pen(sb)
                    _g.DrawEllipse(p, x, y, width, height)
                End Using
            End Using
        End Sub

        Public Sub DrawEllipse(color As Color, x As Single, y As Single, width As Single, height As Single)
            Using sb As New SolidBrush(color)
                Using p As New Pen(sb)
                    _g.DrawEllipse(p, x, y, width, height)
                End Using
            End Using
        End Sub
#End Region

#Region " [ FillRectangle ]"
        Public Sub FillRectangle(color As Color, rect As Rectangle)
            Using sb As New SolidBrush(color)
                _g.FillRectangle(sb, rect)
            End Using
        End Sub

        Public Sub FillRectangle(color As Color, rect As RectangleF)
            Using sb As New SolidBrush(color)
                _g.FillRectangle(sb, rect)
            End Using
        End Sub

        Public Sub FillRectangle(color As Color, x As Integer, y As Integer, width As Integer, height As Integer)
            Using sb As New SolidBrush(color)
                _g.FillRectangle(sb, x, y, width, height)
            End Using
        End Sub

        Public Sub FillRectangle(color As Color, x As Single, y As Single, width As Single, height As Single)
            Using sb As New SolidBrush(color)
                _g.FillRectangle(sb, x, y, width, height)
            End Using
        End Sub

        Public Sub FillRectangles(color As Color, rects() As Rectangle)
            Using sb As New SolidBrush(color)
                _g.FillRectangles(sb, rects)
            End Using
        End Sub

        Public Sub FillRectangles(color As Color, rects() As RectangleF)
            Using sb As New SolidBrush(color)
                _g.FillRectangles(sb, rects)
            End Using
        End Sub
#End Region

#Region " [ FillEllipse ] "
        Public Sub FillEllipse(color As Color, rect As Rectangle)
            Using sb As New SolidBrush(color)
                _g.FillEllipse(sb, rect)
            End Using
        End Sub

        Public Sub FillEllipse(color As Color, rect As RectangleF)
            Using sb As New SolidBrush(color)
                _g.FillEllipse(sb, rect)
            End Using
        End Sub

        Public Sub FillEllipse(color As Color, x As Integer, y As Integer, width As Integer, height As Integer)
            Using sb As New SolidBrush(color)
                _g.FillEllipse(sb, x, y, width, height)
            End Using
        End Sub

        Public Sub FillEllipse(color As Color, x As Single, y As Single, width As Single, height As Single)
            Using sb As New SolidBrush(color)
                _g.FillEllipse(sb, x, y, width, height)
            End Using
        End Sub
#End Region

        Public Sub DrawImage(img As Bitmap, location As PointF, Optional rotate As Double = 0F, Optional opacity As Double = 1.0F)
            If img Is Nothing Then Return

            Dim hw As Double = img.Width / 2.0F
            Dim hh As Double = img.Height / 2.0F

            If rotate <> 0 Then
                _g.TranslateTransform(hw + location.X, hh + location.Y)
                _g.RotateTransform(rotate)
                _g.TranslateTransform(-hw, -hh)
            End If

            If img.HorizontalResolution <> _g.DpiX Or
                img.VerticalResolution <> _g.DpiY Then
                img.SetResolution(_g.DpiX, _g.DpiY)
            End If

            If opacity < 1.0F Then
                Using iAttr As New ImageAttributes
                    Dim cm As New ColorMatrix

                    cm.Matrix33 = opacity
                    iAttr.SetColorMatrix(cm)

                    _g.DrawImage(img, New Rectangle(New Point(location.X, location.Y), img.Size), 0, 0, img.Width, img.Height, GraphicsUnit.Pixel, iAttr)
                End Using
            Else
                If rotate <> 0 Then
                    _g.DrawImage(img, Point.Empty)
                    _g.ResetTransform()
                Else
                    _g.DrawImage(img, location)
                End If
            End If
        End Sub

        Public Sub DrawString(text As String, font As Font, color As Color, location As PointF)
            Using sb As New SolidBrush(color)
                _g.DrawString(text, font, sb, location)
            End Using
        End Sub

        Public Sub Dispose() Implements IDisposable.Dispose
            _g = Nothing
        End Sub
    End Class

End Namespace
