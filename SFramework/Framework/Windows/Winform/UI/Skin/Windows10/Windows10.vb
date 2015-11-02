Imports System.ComponentModel
Imports System.Drawing
Imports System.Windows.Forms
Imports SFramework.Drawing
Imports SFramework.Windows.Winform.Animation
Imports SFramework.Windows.Winform.Animation.DynamicAnimation

Namespace Windows.Winform.UI.Skin
    <Description("Window Form Skin - Windows 10 Skin")>
    Public Class Windows10
        Inherits FormSkinBase
        Implements IVersion

#Region " [ 스킨 정보 ] "
        Public ReadOnly Property Version As String = "Beta" Implements IVersion.Version

        Public ReadOnly Property LastUpdate As New Date(2015, 10, 28) Implements IVersion.LastUpdate
        Public Shadows ReadOnly Property Created As New Date(2015, 10, 26) Implements IVersion.Created

        Public ReadOnly Property Description As String = "Windows 10 Skin for Window Form" Implements IVersion.Description
#End Region

        Private Const DeactivateOpacity As Single = 0.3F

        ' - Titlegbar Animation
        Private WithEvents _animClose As New AnimationBoard
        Private WithEvents _animMaximize As New AnimationBoard
        Private WithEvents _animMinimize As New AnimationBoard
        Private _isAnimClose As Boolean = False
        Private _isAnimMaximize As Boolean = False
        Private _isAnimMinimize As Boolean = False
        Private _closeBack_Opacity As New ColorOpacity(Color.FromArgb(232, 17, 35), 0)
        Private _maximizeBack_Opacity As New ColorOpacity(Color.FromArgb(229, 229, 229), 0)
        Private _minimizeBack_Opacity As New ColorOpacity(Color.FromArgb(229, 229, 229), 0)

        ' - Resource
        Private img_Close_off_Normal As Bitmap,
                img_Close_on_Normal As Bitmap,
                img_Close_off_Over As Bitmap,
                img_Close_on_Over As Bitmap

        Private img_Minimize_off As Bitmap,
                img_Minimize_on As Bitmap

        Private img_Maximize_off As Bitmap,
                img_Maximize_on As Bitmap

#Region " [ 초기화 ] "
        Private Sub Windows10_Load(sender As Object, e As EventArgs) Handles Me.Load
            If IsInited Then
                InitializeResources()
            End If
        End Sub

        Public Overrides Sub InitializeComponent()
            Me.Name = "Windows10"

            MyBase.InitializeComponent()
        End Sub

        <DebuggerStepThrough>
        Private Sub InitializeResources()
            img_Close_off_Normal = LoadResource(Of Bitmap)("close_off_normal.png")
            img_Close_on_Normal = LoadResource(Of Bitmap)("close_on_normal.png")
            img_Close_off_Over = LoadResource(Of Bitmap)("close_off_over.png")
            img_Close_on_Over = LoadResource(Of Bitmap)("close_on_over.png")

            img_Minimize_off = LoadResource(Of Bitmap)("minimize_off_normal.png")
            img_Minimize_on = LoadResource(Of Bitmap)("minimize_on_normal.png")

            img_Maximize_off = LoadResource(Of Bitmap)("maximize_off_normal.png")
            img_Maximize_on = LoadResource(Of Bitmap)("maximize_on_normal.png")
        End Sub

        <DebuggerNonUserCode>
        Protected Overrides Sub Dispose(disposing As Boolean)
            img_Close_off_Normal?.Dispose()
            img_Close_on_Normal?.Dispose()
            img_Close_off_Over?.Dispose()
            img_Close_on_Over?.Dispose()

            img_Minimize_off?.Dispose()
            img_Minimize_on?.Dispose()

            img_Maximize_off?.Dispose()
            img_Maximize_on?.Dispose()
        End Sub
#End Region

#Region " [ Render ] "
        Protected Overrides Sub OnDraw(g As Graphics)
            Dim sg As New SGraphics(g)

            ' < Background >
            g.Clear(ParentWindow.BackColor)

            ' < TitleBar >
            sg.FillRectangle(Color.White, New Rectangle(DrawingPoint, New Size(Me.Width, Titlebar_Area.Rect.Height)))

            ' - Title
            Using title_font As New Font("맑은 고딕", 9)
                Dim title_pt As New Point(28, Titlebar_Area.Rect.Height / 2 - title_font.Height / 2)
                Dim trimming_area As New Rectangle(title_pt, Titlebar_Area.Rect.Size - New Size(title_pt))

                If Not ParentWindow.ShowIcon Then
                    title_pt.X = 7
                End If

                Using format As New StringFormat
                    format.Trimming = StringTrimming.Character
                    format.FormatFlags = StringFormatFlags.NoWrap

                    If trimming_area.Width > 0 Then
                        Using sb As New SolidBrush(If(IsActivated,
                                       ColorPreset.TitleActivate,
                                       ColorPreset.TitleDeactivate))
                            g.DrawString(ParentWindow.Text, title_font, sb, trimming_area, format)
                        End Using
                    End If

                End Using
            End Using

            ' < Title Button >
            Dim btnOpac As Single = If(IsActivated, 1.0F, DeactivateOpacity)

            Dim bufClose As Bitmap = Nothing
            Dim bufMinimize As Bitmap = Nothing
            Dim bufMaximize As Bitmap = Nothing

            If IsMaximized Then
                bufClose = If(_isAnimClose, img_Close_on_Over, img_Close_on_Normal)
                bufMinimize = img_Minimize_on
                bufMaximize = img_Maximize_on
            Else
                bufClose = If(_isAnimClose, img_Close_off_Over, img_Close_off_Normal)
                bufMinimize = img_Minimize_off
                bufMaximize = img_Maximize_off
            End If

            If Titlebar_Close_State <> MouseState.Down Then
                sg.FillRectangle(_closeBack_Opacity.Color, Titlebar_Close.Rect)
            Else
                sg.FillRectangle(Color.FromArgb(241, 112, 122), Titlebar_Close.Rect)
            End If

            If Titlebar_Minimize_State <> MouseState.Down Then
                sg.FillRectangle(_minimizeBack_Opacity.Color, Titlebar_Minimize.Rect)
            Else
                sg.FillRectangle(Color.FromArgb(204, 204, 204), Titlebar_Minimize.Rect)
            End If

            If Titlebar_Maximize_State <> MouseState.Down Then
                sg.FillRectangle(_maximizeBack_Opacity.Color, Titlebar_Maximize.Rect)
            Else
                sg.FillRectangle(Color.FromArgb(204,204,204), Titlebar_Maximize.Rect)
            End If

            sg.DrawImage(bufClose, Titlebar_Close.Rect.Location, 0, btnOpac)
            sg.DrawImage(bufMaximize, Titlebar_Maximize.Rect.Location, 0, btnOpac)
            sg.DrawImage(bufMinimize, Titlebar_Minimize.Rect.Location, 0, btnOpac)

            ' < Title Icon >
            If ParentWindow.ShowIcon Then
                Using icon As New Icon(ParentWindow.Icon, New Size(16, 16))
                    g.DrawIcon(icon, New Rectangle(9, Math.Ceiling(Titlebar_Area.Rect.Height / 2 - icon.Height / 2) + 1, 16, 16))
                End Using
            End If

            ' < Edge >
            If Not IsMaximized Then
                sg.DrawRectangle(If(IsActivated,
                             ColorPreset.EdgeActivate,
                             ColorPreset.EdgeDeactivate),
                             New Rectangle(Point.Empty, DrawingSize))
            End If

            sg.Dispose()
        End Sub
#End Region

#Region " [ Event ] "

        Private Sub Close_Event(sender As Object, arg As MouseEventArgs, e As MouseEvent) Handles Titlebar_Close.PreviewMouseEvent
            Select Case e
                Case MouseEvent.Enter
                    Animation_FadeIn(_animClose, _closeBack_Opacity)
                    _isAnimClose = True

                Case MouseEvent.Leave
                    Animation_FadeOut(_animClose, _closeBack_Opacity)
                    _isAnimClose = False
            End Select
        End Sub

        Private Sub Minimize_Event(sender As Object, arg As MouseEventArgs, e As MouseEvent) Handles Titlebar_Minimize.PreviewMouseEvent
            Select Case e
                Case MouseEvent.Enter
                    Animation_FadeIn(_animMinimize, _minimizeBack_Opacity)
                    _isAnimMinimize = True

                Case MouseEvent.Leave
                    Animation_FadeOut(_animMinimize, _minimizeBack_Opacity)
                    _isAnimMinimize = False
            End Select
        End Sub

        Private Sub Maximize_Event(sender As Object, arg As MouseEventArgs, e As MouseEvent) Handles Titlebar_Maximize.PreviewMouseEvent
            Select Case e
                Case MouseEvent.Enter
                    Animation_FadeIn(_animMaximize, _maximizeBack_Opacity)
                    _isAnimMaximize = True

                Case MouseEvent.Leave
                    Animation_FadeOut(_animMaximize, _maximizeBack_Opacity)
                    _isAnimMaximize = False
            End Select
        End Sub

        Private Sub _animClose_Updated(sender As AnimationBoard, value As Double) Handles _animClose.Updated
            Me.Invalidate()
        End Sub

        Private Sub _animMaximize_Updated(sender As AnimationBoard, value As Double) Handles _animMaximize.Updated
            Me.Invalidate()
        End Sub

        Private Sub _animMinimize_Updated(sender As AnimationBoard, value As Double) Handles _animMinimize.Updated
            Me.Invalidate()
        End Sub
#End Region

#Region " [ Animation ] "
        Private Sub Animation_FadeIn(ByRef board As AnimationBoard, target As Object)
            board.Stop()
            board = New AnimationBoard
            board.Items.Add(
                New DynamicAnimation.Animation(target, "Opacity", 1, 100, New Ease(Ease.EaseType.Cubic, Ease.EasingMode.Out)))
            board.Begin()
        End Sub

        Private Sub Animation_FadeOut(ByRef board As AnimationBoard, target As Object)
            board.Stop()
            board = New AnimationBoard
            board.Items.Add(
                New DynamicAnimation.Animation(target, "Opacity", 0, 150, New Ease(Ease.EaseType.Quad, Ease.EasingMode.In)))
            board.Begin()
        End Sub
#End Region

        ''' <summary>
        ''' 색상 투명도 애니메이션을 위한 클래스
        ''' </summary>
        Private Class ColorOpacity
            Private _blend As Single = 1.0
            Private _baseColor As Color

            Public Property Opacity As Single
                Get
                    Return _blend
                End Get
                Set(value As Single)
                    _blend = value
                End Set
            End Property

            Public Property Color As Color
                Get
                    Return Color.FromArgb(CByte(255 * Opacity), _baseColor)
                End Get
                Set(value As Color)
                    _baseColor = value
                End Set
            End Property

            Sub New(color As Color, opacity As Single)
                _blend = opacity
                _baseColor = color
            End Sub
        End Class
    End Class
End Namespace