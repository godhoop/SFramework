Imports Microsoft.VisualBasic.CompilerServices
Imports System.ComponentModel
Imports System.ComponentModel.Design
Imports System.Windows.Forms
Imports System.Drawing
Imports System.Drawing.Drawing2D
Imports System.Runtime.InteropServices

Imports SFramework.API
Imports SFramework.Drawing
Imports SFramework.Windows.Winform.Animation
Imports SFramework.Windows.Winform.Animation.DynamicAnimation

Namespace Windows.Winform.UI.Skin
    <DesignerGenerated>
    <Designer("System.Windows.Forms.Design.ParentControlDesigner, System.Design", GetType(IDesigner))>
    <Description("Window Form Skin - Windows 10 Skin")>
    Public Class Windows10
        Inherits UserControl
        Implements IFormSkin, IVersion

#Region " [ 스킨 정보 ] "
        Public ReadOnly Property Version As String = "Beta" Implements IVersion.Version

        Public ReadOnly Property LastUpdate As New Date(2015, 10, 28) Implements IVersion.LastUpdate
        Public Shadows ReadOnly Property Created As New Date(2015, 10, 26) Implements IVersion.Created

        Public ReadOnly Property Description As String = "Windows 10 Skin for Window Form" Implements IVersion.Description
#End Region

        Private Const DeactivateOpacity As Single = 0.3F

        ' - Debug
        Private _isDebug As Boolean = True

        ' - Aero
        Private _useAeroSnap As Boolean = False

        ' - Area Event
        Private EventAreas As New List(Of EventRectangle)
        Private LastEvent As EventRectangle = Nothing

        ' - Titlebar
        Private TitleBarArea As EventRectangle
        Private CloseArea As EventRectangle
        Private MinimizeArea As EventRectangle
        Private MaximizeArea As EventRectangle

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

        ' - Resize
        Private Resize_TL As EventRectangle,
                Resize_TC As EventRectangle,
                Resize_TR As EventRectangle,
                Resize_CL As EventRectangle,
                Resize_CR As EventRectangle,
                Resize_BL As EventRectangle,
                Resize_BC As EventRectangle,
                Resize_BR As EventRectangle

        ' - Target Window
        Private WithEvents _ParentWindow As Form
        Private _ParentWndHook As WindowHook

        Private _isInited As Boolean = False
        Private mActivated As Boolean = False

        ' - Resource
        Private img_Close_off_Normal As Bitmap,
                img_Close_on_Normal As Bitmap,
                img_Close_off_Over As Bitmap,
                img_Close_on_Over As Bitmap

        Private img_Minimize_off As Bitmap,
                img_Minimize_on As Bitmap

        Private img_Maximize_off As Bitmap,
                img_Maximize_on As Bitmap

#Region " [ 속성 ] "
        <Category("그래픽")>
        <Description("그래픽 디버깅모드 여부를 가져오거나 설정합니다.")>
        Public Property DebugMode As Boolean
            Get
                Return _isDebug
            End Get
            Set(value As Boolean)
                _isDebug = value
                Me.Invalidate()
            End Set
        End Property

        <Category("그래픽")>
        <Description("Window Aero Snap 동작여부를 가져오거나 설정합니다.")>
        Public Property UseAeroSnap As Boolean
            Get
                Return _useAeroSnap
            End Get
            Set(value As Boolean)
                _useAeroSnap = value
            End Set
        End Property

        Private ReadOnly Property IsInited As Boolean
            Get
                Return _isInited
            End Get
        End Property

        Private ReadOnly Property IsMaximized As Boolean
            Get
                Return ParentWindow.WindowState = FormWindowState.Maximized
            End Get
        End Property

        Private ReadOnly Property ParentWindow As Form
            Get
                Return _ParentWindow
            End Get
        End Property

        Private ReadOnly Property DrawingSize As Size
            Get
                Return Me.Size - If(IsMaximized, Size.Empty, New Size(1, 1))
            End Get
        End Property

        Private ReadOnly Property DrawingPoint As Point
            Get
                Return If(IsMaximized, Point.Empty, New Point(1, 1))
            End Get
        End Property
#End Region

#Region " [ 초기화 ] "
        <DebuggerStepThrough>
        Private Sub InitializeComponent()
            Me.SuspendLayout()

            Me.AutoScaleDimensions = New SizeF(7.0!, 12.0!)
            Me.AutoScaleMode = AutoScaleMode.Font
            Me.Name = "Windows10"

            Me.ResumeLayout(False)
        End Sub

        <DebuggerStepThrough>
        Private Sub InitializeResources()
            img_Close_off_Normal = Assembly.AssemblyResource.GetData(Of Bitmap)("close_off_normal.png")
            img_Close_on_Normal = Assembly.AssemblyResource.GetData(Of Bitmap)("close_on_normal.png")
            img_Close_off_Over = Assembly.AssemblyResource.GetData(Of Bitmap)("close_off_over.png")
            img_Close_on_Over = Assembly.AssemblyResource.GetData(Of Bitmap)("close_on_over.png")

            img_Minimize_off = Assembly.AssemblyResource.GetData(Of Bitmap)("minimize_off_normal.png")
            img_Minimize_on = Assembly.AssemblyResource.GetData(Of Bitmap)("minimize_on_normal.png")

            img_Maximize_off = Assembly.AssemblyResource.GetData(Of Bitmap)("maximize_off_normal.png")
            img_Maximize_on = Assembly.AssemblyResource.GetData(Of Bitmap)("maximize_on_normal.png")
        End Sub

        <DebuggerStepThrough>
        Private Sub InitializeEvent()
            InitializeTitleEvent()
            InitializeResizeEvent()

            EventAreas.AddRange(
                {Resize_TL, Resize_TR, ' 모서리 Top
                 Resize_BL, Resize_BR, ' 모서리 Bottom
                 CloseArea, MaximizeArea, MinimizeArea,
                 Resize_TC,
                 Resize_CL, Resize_CR,
                 Resize_BC,
                 TitleBarArea})
        End Sub

        Private Sub InitializeTitleEvent()
            Dim area As New Rectangle(New Point(0, 1), New Size(45, 29))

            CloseArea = New EventRectangle(area, New Action(Of Object, Object)(AddressOf Close_Event))
            MaximizeArea = New EventRectangle(area, New Action(Of Object, Object)(AddressOf Maximize_Event))
            MinimizeArea = New EventRectangle(area, New Action(Of Object, Object)(AddressOf Minimize_Event))

            TitleBarArea = New EventRectangle(New Rectangle(1, 1, 100, 30), New Action(Of Object, Object)(AddressOf Titlebar_Event))
        End Sub

        Private Sub InitializeResizeEvent()
            Dim area As New Rectangle(Point.Empty, New Size(5, 5))
            Dim coverArea As New Rectangle(Point.Empty, New Size(8, 8))

            Dim t As String = "Resize"

            Resize_TL = New EventRectangle(coverArea, New Action(Of Object, Object)(AddressOf Resize_TL_Event)) With {.Tag = t}
            Resize_TC = New EventRectangle(area, New Action(Of Object, Object)(AddressOf Resize_TC_Event)) With {.Tag = t}
            Resize_TR = New EventRectangle(coverArea, New Action(Of Object, Object)(AddressOf Resize_TR_Event)) With {.Tag = t}
            Resize_CL = New EventRectangle(area, New Action(Of Object, Object)(AddressOf Resize_CL_Event)) With {.Tag = t}
            Resize_CR = New EventRectangle(area, New Action(Of Object, Object)(AddressOf Resize_CR_Event)) With {.Tag = t}
            Resize_BL = New EventRectangle(coverArea, New Action(Of Object, Object)(AddressOf Resize_BL_Event)) With {.Tag = t}
            Resize_BC = New EventRectangle(area, New Action(Of Object, Object)(AddressOf Resize_BC_Event)) With {.Tag = t}
            Resize_BR = New EventRectangle(coverArea, New Action(Of Object, Object)(AddressOf Resize_BR_Event)) With {.Tag = t}
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

        Private Sub Windows10_Load(sender As Object, e As EventArgs) Handles Me.Load
            If GetType(Form).IsAssignableFrom(Parent.GetType) Then
                Me.SetStyle(ControlStyles.UserPaint Or
                        ControlStyles.AllPaintingInWmPaint Or
                        ControlStyles.OptimizedDoubleBuffer, True)

                InitializeEvent()
                InitializeResources()

                _ParentWindow = Parent
                _ParentWndHook = New WindowHook(ParentWindow)
                ParentWindow.FormBorderStyle = FormBorderStyle.None
                ParentWindow.MinimumSize = New Size(122, 32)

                Me.Dock = DockStyle.Fill

                ' [ 임시 코드 ]
                ' [ TODO: AeroSnap용 Window 스타일 지정 ]
                Dim wLong As Integer = USER32.GetWindowLong(ParentWindow.Handle, Struct.WindowLongFlags.GWL_STYLE)
                USER32.SetWindowLong(ParentWindow.Handle, Struct.WindowLongFlags.GWL_STYLE, wLong)
                ' Struct.WindowStyles.WS_SIZEFRAME

                _isInited = True
            Else
                MsgBox("Skin 배치가 잘못되었습니다.", MsgBoxStyle.Exclamation, "Skin.Windows10")
            End If
        End Sub
#End Region

#Region " [ Render ] "
        Private Sub Windows10_SizeChanged(sender As Object, e As EventArgs) Handles Me.SizeChanged
            Me.Invalidate()
        End Sub

        Private Sub Windows10_Paint(sender As Object, e As PaintEventArgs) Handles Me.Paint
            Try
                Using buffer As BufferedGraphics = BufferedGraphicsManager.Current.Allocate(e.Graphics, Me.ClientRectangle)
                    Dim g As Graphics = buffer.Graphics
                    Dim gCon As GraphicsContainer = g.BeginContainer

                    g.InterpolationMode = InterpolationMode.High
                    Draw(g)
                    g.EndContainer(gCon)
                    buffer.Render()

                    buffer.Dispose()
                    g.Dispose()
                End Using
            Catch ex As Exception
            End Try
        End Sub

        Private Sub Draw(g As Graphics)
            ' - Graphic Debugging
            Dim _dbg_rcts As New List(Of Rectangle)

            Dim sg As New SGraphics(g)

            UpdateLayout()

            ' < Background >
            g.Clear(ParentWindow.BackColor)

            ' < TitleBar >
            sg.FillRectangle(Color.White, New Rectangle(DrawingPoint, New Size(Me.Width, TitleBarArea.Height)))

            ' - Title
            Using title_font As New Font("맑은 고딕", 9)
                Dim title_pt As New Point(28, TitleBarArea.Height / 2 - title_font.Height / 2)
                Dim trimming_area As New Rectangle(title_pt, TitleBarArea.Size - New Size(title_pt))

                If Not ParentWindow.ShowIcon Then
                    title_pt.X = 7
                End If

                Using format As New StringFormat
                    format.Trimming = StringTrimming.Character
                    format.FormatFlags = StringFormatFlags.NoWrap

                    If trimming_area.Width > 0 Then
                        _dbg_rcts.Add(trimming_area)

                        Using sb As New SolidBrush(If(mActivated,
                                       ColorPreset.TitleActivate,
                                       ColorPreset.TitleDeactivate))
                            g.DrawString(ParentWindow.Text, title_font, sb, trimming_area, format)
                        End Using
                    End If

                End Using
            End Using

            ' < Title Button >
            Dim btnOpac As Single = If(mActivated, 1.0F, DeactivateOpacity)

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

            sg.FillRectangle(_closeBack_Opacity.Color, CloseArea.Rect)
            sg.FillRectangle(_minimizeBack_Opacity.Color, MinimizeArea.Rect)
            sg.FillRectangle(_maximizeBack_Opacity.Color, MaximizeArea.Rect)

            sg.DrawImage(bufClose, CloseArea.Location, 0, btnOpac)
            sg.DrawImage(bufMaximize, MaximizeArea.Location, 0, btnOpac)
            sg.DrawImage(bufMinimize, MinimizeArea.Location, 0, btnOpac)

            ' < Title Icon >
            If ParentWindow.ShowIcon Then
                Using icon As New Icon(ParentWindow.Icon, New Size(16, 16))
                    g.DrawIcon(icon, New Rectangle(9, Math.Ceiling(TitleBarArea.Height / 2 - icon.Height / 2) + 1, 16, 16))
                End Using
            End If

            ' < Edge >
            If Not IsMaximized Then
                sg.DrawRectangle(If(mActivated,
                             ColorPreset.EdgeActivate,
                             ColorPreset.EdgeDeactivate),
                             New Rectangle(Point.Empty, DrawingSize))
            End If

            ' < Debug >
            If DebugMode Then
                _dbg_rcts.AddRange(EventAreas.Select(Function(er As EventRectangle) er.Rect).ToArray)

                For Each area As Rectangle In _dbg_rcts
                    sg.FillRectangle(Color.FromArgb(50, Color.Red), area)
                    sg.DrawRectangle(Color.Blue, area)
                Next
            End If

            sg.Dispose()
        End Sub

        <Description("스킨 구성요소를 재배치합니다.")>
        Public Sub UpdateLayout()
            TitleBarArea.Height = If(IsMaximized, 23, 30)

            CloseArea.Height = TitleBarArea.Height - If(IsMaximized, 2, 1)
            MaximizeArea.Height = CloseArea.Height
            MinimizeArea.Height = CloseArea.Height

            CloseArea.X = DrawingSize.Width - CloseArea.Width
            MaximizeArea.X = CloseArea.X - MaximizeArea.Width - 1
            MinimizeArea.X = MaximizeArea.X - MinimizeArea.Width - 1

            TitleBarArea.Width = MinimizeArea.X - 1

            CloseArea.Y = DrawingPoint.Y
            MaximizeArea.Y = DrawingPoint.Y
            MinimizeArea.Y = DrawingPoint.Y
            TitleBarArea.Y = DrawingPoint.Y

            UpdateResize()

            EventAreas.ForEach(
                Sub(er As EventRectangle)
                    If er.Tag = "Resize" Then
                        er.IsEnabled = Not IsMaximized
                    End If
                End Sub)
        End Sub

        Private Sub UpdateResize()
            ' 모서리
            Resize_TR.X = Me.Width - Resize_TR.Width - DrawingPoint.X
            Resize_BL.Y = Me.Height - Resize_BL.Height - DrawingPoint.X
            Resize_BR.X = Resize_TR.X
            Resize_BR.Y = Resize_BL.Y

            ' 위아래/양옆
            Resize_TC.X = Resize_TL.Right
            Resize_TC.Width = Resize_TR.X - Resize_TL.Right

            Resize_BC.X = Resize_BL.Right
            Resize_BC.Width = Resize_BR.X - Resize_BL.Right
            Resize_BC.Y = Me.Height - Resize_BC.Height - DrawingPoint.X

            Resize_CL.Y = Resize_TL.Bottom
            Resize_CL.Height = Resize_BL.Y - Resize_TL.Bottom

            Resize_CR.Y = Resize_TR.Bottom
            Resize_CR.Height = Resize_BR.Y - Resize_TR.Bottom
            Resize_CR.X = Me.Width - Resize_CR.Width - DrawingPoint.X
        End Sub
#End Region

#Region " [ Parent ] "
        Private Sub _ParentWindow_Load(sender As Object, e As EventArgs) Handles _ParentWindow.Load
            _ParentWndHook.Hook()

            ' - 최대화 오류 해결
            _ParentWndHook.Filters.Add(
                New Filter(Struct.WindowsMessages.WM_GETMINMAXINFO,
                           New Filter.MessageProc(AddressOf Parent_GETMINMAXINFO)))

            ' - AeroSnap 보더 제거
            _ParentWndHook.Filters.Add(
                New Filter(Struct.WindowsMessages.WM_NCCALCSIZE,
                           New Filter.MessageProc(AddressOf Parent_NCCALSIZE)))
        End Sub

        Private Sub Parent_NCCALSIZE(hwnd As IntPtr, ByRef wParam As IntPtr, ByRef lParam As IntPtr, ByRef Result As IntPtr)
            If UseAeroSnap Then
                If wParam.Equals(IntPtr.Zero) Then
                    Dim rc As Struct.RECT = _ParentWndHook.GetLParam(Of Struct.RECT)(lParam)
                    Dim r As Rectangle = rc.ToRectangle()
                    r.Inflate(8, 8)

                    Marshal.StructureToPtr(New Struct.RECT(r), lParam, True)
                Else
                    Dim csp As Struct.NCCALCSIZE_PARAMS = _ParentWndHook.GetLParam(Of Struct.NCCALCSIZE_PARAMS)(lParam)
                    Dim r As Rectangle = csp.rgrc0.ToRectangle()
                    r.Inflate(8, 8)
                    csp.rgrc0 = New Struct.RECT(r)

                    Marshal.StructureToPtr(csp, lParam, True)
                End If

                Result = IntPtr.Zero
            End If
        End Sub

        Private Sub Parent_GETMINMAXINFO(hwnd As IntPtr, ByRef wParam As IntPtr, ByRef lParam As IntPtr, ByRef Result As IntPtr)
            Dim mmi As Struct.MINMAXINFO = Marshal.PtrToStructure(lParam, GetType(Struct.MINMAXINFO))

            Dim MONITOR_DEFAULTTONEAREST As Integer = &H2
            Dim monitor As IntPtr = USER32.MonitorFromWindow(hwnd, MONITOR_DEFAULTTONEAREST)

            If monitor <> IntPtr.Zero Then
                Dim monitorInfo As New Struct.MONITORINFO()
                USER32.GetMonitorInfo(monitor, monitorInfo)

                Dim rcWorkArea As Struct.RECT = monitorInfo.rcWork
                Dim rcMonitorArea As Struct.RECT = monitorInfo.rcMonitor

                mmi.ptMaxPosition.X = Math.Abs(rcWorkArea.Left - rcMonitorArea.Left)
                mmi.ptMaxPosition.Y = Math.Abs(rcWorkArea.Top - rcMonitorArea.Top)
                mmi.ptMaxSize.X = Math.Abs(rcWorkArea.Right - rcWorkArea.Left)
                mmi.ptMaxSize.Y = Math.Abs(rcWorkArea.Bottom - rcWorkArea.Top)
            End If

            Marshal.StructureToPtr(mmi, lParam, True)
        End Sub

        Private Sub _ParentWindow_TextChanged(sender As Object, e As EventArgs) Handles _ParentWindow.TextChanged
            Me.Invalidate()
        End Sub

        Private Sub _ParentWindow_Activated(sender As Object, e As EventArgs) Handles _ParentWindow.Activated
            mActivated = True
            Me.Invalidate()
        End Sub

        Private Sub _ParentWindow_Deactivate(sender As Object, e As EventArgs) Handles _ParentWindow.Deactivate
            mActivated = False
            Me.Invalidate()
        End Sub
#End Region

#Region " [ Event Capture ] "
        Private Sub Windows10_MouseLeave(sender As Object, e As EventArgs) Handles Me.MouseLeave
            If LastEvent IsNot Nothing Then
                LastEvent.Handler.Invoke(Nothing, MouseEvent.Leave)
                LastEvent = Nothing
            End If
        End Sub

        Private Sub Windows10_MouseMove(sender As Object, e As MouseEventArgs) Handles Me.MouseMove
            CaptureMouseEvent(MouseEvent.Move, e)
        End Sub

        Private Sub Windows10_MouseUp(sender As Object, e As MouseEventArgs) Handles Me.MouseUp
            CaptureMouseEvent(MouseEvent.Up, e)
        End Sub

        Private Sub Windows10_MouseDown(sender As Object, e As MouseEventArgs) Handles Me.MouseDown
            CaptureMouseEvent(MouseEvent.Down, e)
        End Sub

        Private Sub Windows10_MouseClick(sender As Object, e As MouseEventArgs) Handles Me.MouseClick
            CaptureMouseEvent(MouseEvent.Click, e)
            Me.Invalidate()
        End Sub

        Private Sub Windows10_MouseDoubleClick(sender As Object, e As MouseEventArgs) Handles Me.MouseDoubleClick
            CaptureMouseEvent(MouseEvent.DoubleClick, e)
            Me.Invalidate()
        End Sub

        Private Sub CaptureMouseEvent(state As MouseEvent, e As MouseEventArgs)
            Dim mRct As New Rectangle(e.X, e.Y, 1, 1)
            Dim isRaised As Boolean = False

            For Each er As EventRectangle In EventAreas.Where(Function(ea As EventRectangle) ea.IsEnabled AndAlso ea.Rect.IntersectsWith(mRct))
                If LastEvent IsNot Nothing AndAlso
                        Not LastEvent.Equals(er) Then

                    er.Handler.Invoke(e, MouseEvent.Enter)
                    LastEvent.Handler.Invoke(e, MouseEvent.Leave)
                End If

                If LastEvent Is Nothing Then
                    er.Handler.Invoke(e, MouseEvent.Enter)
                End If

                er.Handler.Invoke(e, state)
                LastEvent = er

                isRaised = True

                Exit For
            Next

            If Not isRaised AndAlso LastEvent IsNot Nothing Then
                LastEvent.Handler.Invoke(e, MouseEvent.Leave)
                LastEvent = Nothing
            End If
        End Sub
#End Region

#Region " [ Event ] "
        Private Sub Resize_TL_Event(arg1 As Object, arg2 As Object)
            Resize_Event(arg1, arg2, Cursors.SizeNWSE, Struct.SCType.SC_DRAG_RESIZEUL)
        End Sub

        Private Sub Resize_TC_Event(arg1 As Object, arg2 As Object)
            Resize_Event(arg1, arg2, Cursors.SizeNS, Struct.SCType.SC_DRAG_RESIZEU)
        End Sub

        Private Sub Resize_TR_Event(arg1 As Object, arg2 As Object)
            Resize_Event(arg1, arg2, Cursors.SizeNESW, Struct.SCType.SC_DRAG_RESIZEUR)
        End Sub

        Private Sub Resize_CL_Event(arg1 As Object, arg2 As Object)
            Resize_Event(arg1, arg2, Cursors.SizeWE, Struct.SCType.SC_DRAG_RESIZEL)
        End Sub

        Private Sub Resize_CR_Event(arg1 As Object, arg2 As Object)
            Resize_Event(arg1, arg2, Cursors.SizeWE, Struct.SCType.SC_DRAG_RESIZER)
        End Sub

        Private Sub Resize_BL_Event(arg1 As Object, arg2 As Object)
            Resize_Event(arg1, arg2, Cursors.SizeNESW, Struct.SCType.SC_DRAG_RESIZEDL)
        End Sub

        Private Sub Resize_BC_Event(arg1 As Object, arg2 As Object)
            Resize_Event(arg1, arg2, Cursors.SizeNS, Struct.SCType.SC_DRAG_RESIZED)
        End Sub

        Private Sub Resize_BR_Event(arg1 As Object, arg2 As Object)
            Resize_Event(arg1, arg2, Cursors.SizeNWSE, Struct.SCType.SC_DRAG_RESIZEDR)
        End Sub

        Private Sub Resize_Event(arg1 As MouseEventArgs, arg2 As MouseEvent, cursor As Cursor, sctype As Struct.SCType)
            Static cursorLock As Boolean = False

            Select Case CType(arg2, MouseEvent)
                Case MouseEvent.Leave
                    If Not cursorLock Then
                        Me.Cursor = Cursors.Default
                    End If

                Case MouseEvent.Down
                    If Not IsMaximized AndAlso
                        CType(arg1, MouseEventArgs).Button = MouseButtons.Left Then
                        cursorLock = True

                        USER32.ReleaseCapture()
                        USER32.SendMessage(ParentWindow.Handle, Struct.WindowsMessages.WM_SYSCOMMAND, sctype, IntPtr.Zero)

                        cursorLock = False
                    End If

                Case MouseEvent.Move
                    If Not IsMaximized Then
                        Me.Cursor = cursor
                    End If
            End Select
        End Sub

        Private Sub Titlebar_Event(arg1 As Object, arg2 As Object)
            Select Case CType(arg2, MouseEvent)
                Case MouseEvent.Move
                    If CType(arg1, MouseEventArgs).Button = MouseButtons.Left Then
                        USER32.ReleaseCapture()
                        USER32.PostMessage(ParentWindow.Handle, Struct.WindowsMessages.WM_NCLBUTTONDOWN, 2, IntPtr.Zero)
                    End If

                Case MouseEvent.DoubleClick
                    Maximize_Event(arg1, MouseEvent.Click)
            End Select
        End Sub

        Private Sub Close_Event(arg1 As Object, arg2 As Object)
            Select Case CType(arg2, MouseEvent)
                Case MouseEvent.Enter
                    Animation_FadeIn(_animClose, _closeBack_Opacity)
                    _isAnimClose = True

                Case MouseEvent.Leave
                    Animation_FadeOut(_animClose, _closeBack_Opacity)
                    _isAnimClose = False

                Case MouseEvent.Click
                    USER32.PostMessage(ParentWindow.Handle, Struct.WindowsMessages.WM_SYSCOMMAND, Struct.SCType.SC_CLOSE, IntPtr.Zero)
            End Select
        End Sub

        Private Sub Minimize_Event(arg1 As Object, arg2 As Object)
            Select Case CType(arg2, MouseEvent)
                Case MouseEvent.Enter
                    Animation_FadeIn(_animMinimize, _minimizeBack_Opacity)
                    _isAnimMinimize = True

                Case MouseEvent.Leave
                    Animation_FadeOut(_animMinimize, _minimizeBack_Opacity)
                    _isAnimMinimize = False

                Case MouseEvent.Click
                    If ParentWindow.WindowState = FormWindowState.Minimized Then
                        USER32.PostMessage(ParentWindow.Handle, Struct.WindowsMessages.WM_SYSCOMMAND, Struct.SCType.SC_RESTORE, IntPtr.Zero)
                    Else
                        USER32.PostMessage(ParentWindow.Handle, Struct.WindowsMessages.WM_SYSCOMMAND, Struct.SCType.SC_MINIMIZE, IntPtr.Zero)
                    End If
            End Select
        End Sub

        Private Sub Maximize_Event(arg1 As Object, arg2 As Object)
            Select Case CType(arg2, MouseEvent)
                Case MouseEvent.Enter
                    Animation_FadeIn(_animMaximize, _maximizeBack_Opacity)
                    _isAnimMaximize = True

                Case MouseEvent.Leave
                    Animation_FadeOut(_animMaximize, _maximizeBack_Opacity)
                    _isAnimMaximize = False

                Case MouseEvent.Click
                    If ParentWindow.WindowState = FormWindowState.Maximized Then
                        USER32.PostMessage(ParentWindow.Handle, Struct.WindowsMessages.WM_SYSCOMMAND, Struct.SCType.SC_RESTORE, IntPtr.Zero)
                    Else
                        USER32.PostMessage(ParentWindow.Handle, Struct.WindowsMessages.WM_SYSCOMMAND, Struct.SCType.SC_MAXIMIZE, IntPtr.Zero)
                    End If
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