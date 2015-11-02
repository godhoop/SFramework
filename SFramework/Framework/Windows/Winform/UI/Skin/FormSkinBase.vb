
Imports Microsoft.VisualBasic.CompilerServices
Imports System.ComponentModel
Imports System.ComponentModel.Design
Imports System.Windows.Forms
Imports System.Drawing
Imports System.Drawing.Drawing2D
Imports System.Runtime.InteropServices
Imports SFramework.API
Imports SFramework.Control

Namespace Windows.Winform.UI.Skin
    <ComVisible(True)>
    <ClassInterface(ClassInterfaceType.AutoDispatch)>
    <DesignerGenerated>
    <Designer("System.Windows.Forms.Design.ParentControlDesigner, System.Design", GetType(IDesigner))>
    Public MustInherit Class FormSkinBase
        Inherits UserControl
        Implements IFormSkin
        Implements IVirtualControl

        Public Property Childs As VirtualControlCollection Implements IVirtualControl.Childs
        Private LastEvent As EventRectangle = Nothing

        ' - Aero
        Private _useAeroSnap As Boolean = False

        ' - Titlebar
        Protected WithEvents _
            Titlebar_Area As MouseCaptureControl,
            Titlebar_Close As MouseCaptureControl,
            Titlebar_Maximize As MouseCaptureControl,
            Titlebar_Minimize As MouseCaptureControl

        Protected Titlebar_Close_State As MouseState = MouseState.None
        Protected Titlebar_Maximize_State As MouseState = MouseState.None
        Protected Titlebar_Minimize_State As MouseState = MouseState.None

        ' - Resize
        Protected Resizers As VirtualControlCollection
        Protected WithEvents _
            Resize_TopLeft As MouseCaptureControl,
            Resize_TopCenter As MouseCaptureControl,
            Resize_TopRight As MouseCaptureControl,
            Resize_CenterLeft As MouseCaptureControl,
            Resize_CenterRight As MouseCaptureControl,
            Resize_BottomLeft As MouseCaptureControl,
            Resize_BottomCenter As MouseCaptureControl,
            Resize_BottomRight As MouseCaptureControl

        ' - Target Window
        Private WithEvents _ParentWindow As Form
        Private _ParentWndHook As WindowHook

        Private _isInited As Boolean = False
        Private _isActivated As Boolean = False

#Region " [ 속성 ] "
        <Category("동작")>
        <Description("Window Aero Snap 동작여부를 가져오거나 설정합니다.")>
        Public Property UseAeroSnap As Boolean
            Get
                Return _useAeroSnap
            End Get
            Set(value As Boolean)
                _useAeroSnap = value
            End Set
        End Property

        '! 퇴화 속성
        Private Property IsEnabled As Boolean Implements IVirtualControl.IsEnabled

        Protected ReadOnly Property ParentWindow As Form
            Get
                Return _ParentWindow
            End Get
        End Property

        Protected ReadOnly Property ParentWindowHook As WindowHook
            Get
                Return _ParentWndHook
            End Get
        End Property

        Protected ReadOnly Property IsInited As Boolean
            Get
                Return _isInited
            End Get
        End Property

        Protected ReadOnly Property IsActivated As Boolean
            Get
                Return _isActivated
            End Get
        End Property

        Protected ReadOnly Property IsMaximized As Boolean
            Get
                Return ParentWindow.WindowState = FormWindowState.Maximized
            End Get
        End Property

        Protected ReadOnly Property DrawingSize As Size
            Get
                Return Me.Size - If(IsMaximized, Size.Empty, New Size(1, 1))
            End Get
        End Property

        Protected ReadOnly Property DrawingPoint As Point
            Get
                Return If(IsMaximized, Point.Empty, New Point(1, 1))
            End Get
        End Property
#End Region

        Private Shared Function GetFlags(input As [Enum]) As IEnumerable(Of [Enum])
            Dim lst As New List(Of [Enum])

            For Each value As [Enum] In [Enum].GetValues(input.[GetType]())
                If input.HasFlag(value) Then
                    lst.Add(value)
                End If
            Next

            Return lst
        End Function

#Region " [ 초기화 ] "
        Private Sub FormSkinBase_Load(sender As Object, e As EventArgs) Handles Me.Load
            If GetType(Form).IsAssignableFrom(Parent.GetType) Then
                Me.SetStyle(ControlStyles.UserPaint Or
                            ControlStyles.AllPaintingInWmPaint Or
                            ControlStyles.OptimizedDoubleBuffer, True)

                InitializeEvent()

                _ParentWindow = Parent
                _ParentWndHook = New WindowHook(ParentWindow)
                ParentWindow.FormBorderStyle = FormBorderStyle.None
                ParentWindow.MinimumSize = New Size(122, 32)

                Me.Dock = DockStyle.Fill

                If UseAeroSnap Then
                    Dim wLong As Struct.WindowStyles = USER32.GetWindowLong(ParentWindow.Handle, Struct.WindowLongFlags.GWL_STYLE)

                    USER32.SetWindowLong(ParentWindow.Handle, Struct.WindowLongFlags.GWL_STYLE,
                                         wLong Or
                                         Struct.WindowStyles.WS_MINIMIZEBOX Or
                                         Struct.WindowStyles.WS_SIZEFRAME Or
                                         Struct.WindowStyles.WS_SYSMENU)
                End If

                _isInited = True
            Else
                MsgBox("Skin 배치가 잘못되었습니다.", MsgBoxStyle.Exclamation, "Skin.Windows10")
            End If
        End Sub

        Sub New()
            InitializeComponent()

            Childs = New VirtualControlCollection(Nothing)
            Resizers = New VirtualControlCollection(Nothing)
        End Sub

        ' - 기본 구성요소 초기화
        <DebuggerStepThrough>
        Public Overridable Sub InitializeComponent()
            Me.SuspendLayout()

            Me.AutoScaleDimensions = New SizeF(7.0!, 12.0!)
            Me.AutoScaleMode = AutoScaleMode.Font

            Me.ResumeLayout(False)
        End Sub

        ' - Titlebar, Resize 영역 이벤트 초기화
        <DebuggerStepThrough>
        Public Overridable Sub InitializeEvent()
            InitializeTitleEvent()
            InitializeResizeEvent()

            Childs.AddRange(
                {Resize_TopLeft, Resize_TopRight,       ' 모서리 Top
                 Resize_BottomLeft, Resize_BottomRight, ' 모서리 Bottom
                 Titlebar_Close,
                 Titlebar_Maximize,
                 Titlebar_Minimize,
                 Resize_TopCenter,
                 Resize_CenterLeft, Resize_CenterRight,
                 Resize_BottomCenter,
                 Titlebar_Area})
        End Sub

        Private Sub InitializeTitleEvent()
            Dim area As New Rectangle(New Point(0, 1), New Size(45, 29))

            Titlebar_Close = New MouseCaptureControl(area)
            Titlebar_Maximize = New MouseCaptureControl(area)
            Titlebar_Minimize = New MouseCaptureControl(area)

            Titlebar_Area = New MouseCaptureControl(New Rectangle(1, 1, 100, 30))
        End Sub

        Private Sub InitializeResizeEvent()
            Dim area As New Rectangle(Point.Empty, New Size(5, 5))
            Dim coverArea As New Rectangle(Point.Empty, New Size(8, 8))

            Resize_TopLeft = New MouseCaptureControl(coverArea)
            Resize_TopCenter = New MouseCaptureControl(area)
            Resize_TopRight = New MouseCaptureControl(coverArea)
            Resize_CenterLeft = New MouseCaptureControl(area)
            Resize_CenterRight = New MouseCaptureControl(area)
            Resize_BottomLeft = New MouseCaptureControl(coverArea)
            Resize_BottomCenter = New MouseCaptureControl(area)
            Resize_BottomRight = New MouseCaptureControl(coverArea)

            Resizers.AddRange(
                {Resize_TopLeft, Resize_TopCenter, Resize_TopRight,
                 Resize_CenterLeft, Resize_CenterRight,
                 Resize_BottomLeft, Resize_BottomCenter, Resize_BottomRight})
        End Sub
#End Region

#Region " [ Event Capture ] "
        Private Sub FormSkinBase_MouseLeave(sender As Object, e As EventArgs) Handles Me.MouseLeave
            If LastEvent IsNot Nothing Then
                LastEvent.Handler.Invoke(Nothing, MouseEvent.Leave)
                LastEvent = Nothing

                ResetTitlebarMouseState()
            End If
        End Sub

        Private Sub FormSkinBase_MouseMove(sender As Object, e As MouseEventArgs) Handles Me.MouseMove
            CaptureMouseEvent(MouseEvent.Move, e)
        End Sub

        Private Sub FormSkinBase_MouseUp(sender As Object, e As MouseEventArgs) Handles Me.MouseUp
            CaptureMouseEvent(MouseEvent.Up, e)
        End Sub

        Private Sub FormSkinBase_MouseDown(sender As Object, e As MouseEventArgs) Handles Me.MouseDown
            CaptureMouseEvent(MouseEvent.Down, e)
        End Sub

        Private Sub FormSkinBase_MouseClick(sender As Object, e As MouseEventArgs) Handles Me.MouseClick
            CaptureMouseEvent(MouseEvent.Click, e)
            Me.Invalidate()
        End Sub

        Private Sub FormSkinBase_MouseDoubleClick(sender As Object, e As MouseEventArgs) Handles Me.MouseDoubleClick
            CaptureMouseEvent(MouseEvent.DoubleClick, e)
            Me.Invalidate()
        End Sub

        Private Sub CaptureMouseEvent(state As MouseEvent, e As MouseEventArgs)
            Dim mRct As New Rectangle(e.X, e.Y, 1, 1)
            Dim isRaised As Boolean = False

            For Each er As EventRectangle In Childs.Where(Function(mc As MouseCaptureControl) mc.IsEnabled AndAlso mc.Rect.IntersectsWith(mRct)).
                                                    Select(Function(mc As MouseCaptureControl) mc.EventRect)
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

                ResetTitlebarMouseState
            End If
        End Sub
#End Region

#Region " [ Event ] "
        Private Sub Titlebar_Close_PreviewMouseEvent(sender As Object, arg As MouseEventArgs, e As MouseEvent) Handles Titlebar_Close.PreviewMouseEvent
            Dim state As MouseState

            Select Case e
                Case MouseEvent.Leave, MouseEvent.Up
                    state = MouseState.None

                Case MouseEvent.Move
                    state = MouseState.Over

                Case MouseEvent.Down
                    state = MouseState.Down
            End Select

            If state <> Titlebar_Minimize_State Then
                Titlebar_Close_State = state
                Me.Invalidate()
            End If
        End Sub

        Private Sub Titlebar_Maximize_PreviewMouseEvent(sender As Object, arg As MouseEventArgs, e As MouseEvent) Handles Titlebar_Maximize.PreviewMouseEvent
            Dim state As MouseState

            Select Case e
                Case MouseEvent.Leave, MouseEvent.Up
                    state = MouseState.None

                Case MouseEvent.Move
                    state = MouseState.Over

                Case MouseEvent.Down
                    state = MouseState.Down
            End Select

            If state <> Titlebar_Minimize_State Then
                Titlebar_Maximize_State = state
                Me.Invalidate()
            End If
        End Sub

        Private Sub Titlebar_Minimize_PreviewMouseEvent(sender As Object, arg As MouseEventArgs, e As MouseEvent) Handles Titlebar_Minimize.PreviewMouseEvent
            Dim state As MouseState

            Select Case e
                Case MouseEvent.Leave, MouseEvent.Up
                    state = MouseState.None

                Case MouseEvent.Move
                    state = MouseState.Over

                Case MouseEvent.Down
                    state = MouseState.Down
            End Select

            If state <> Titlebar_Minimize_State Then
                Titlebar_Minimize_State = state
                Me.Invalidate()
            End If
        End Sub

        Private Sub Titlebar_Area_PreviewMouseEvent(sender As Object, arg As MouseEventArgs, e As MouseEvent) Handles Titlebar_Area.PreviewMouseEvent
            Select Case e
                Case MouseEvent.Move
                    If arg.Button = MouseButtons.Left Then
                        USER32.ReleaseCapture()
                        USER32.PostMessage(ParentWindow.Handle, Struct.WindowsMessages.WM_NCLBUTTONDOWN, 2, IntPtr.Zero)
                    End If

                Case MouseEvent.DoubleClick
                    Titlebar_Maximize_MouseClick(Titlebar_Maximize, arg)
            End Select
        End Sub

        Private Sub Titlebar_Close_MouseClick(sender As Object, e As MouseEventArgs) Handles Titlebar_Close.MouseClick
            USER32.PostMessage(ParentWindow.Handle, Struct.WindowsMessages.WM_SYSCOMMAND, Struct.SCType.SC_CLOSE, IntPtr.Zero)
        End Sub

        Private Sub Titlebar_Minimize_MouseClick(sender As Object, e As MouseEventArgs) Handles Titlebar_Minimize.MouseClick
            If ParentWindow.WindowState = FormWindowState.Minimized Then
                USER32.PostMessage(ParentWindow.Handle, Struct.WindowsMessages.WM_SYSCOMMAND, Struct.SCType.SC_RESTORE, IntPtr.Zero)
            Else
                USER32.PostMessage(ParentWindow.Handle, Struct.WindowsMessages.WM_SYSCOMMAND, Struct.SCType.SC_MINIMIZE, IntPtr.Zero)
            End If
        End Sub

        Private Sub Titlebar_Maximize_MouseClick(sender As Object, e As MouseEventArgs) Handles Titlebar_Maximize.MouseClick
            If ParentWindow.WindowState = FormWindowState.Maximized Then
                USER32.PostMessage(ParentWindow.Handle, Struct.WindowsMessages.WM_SYSCOMMAND, Struct.SCType.SC_RESTORE, IntPtr.Zero)
            Else
                USER32.PostMessage(ParentWindow.Handle, Struct.WindowsMessages.WM_SYSCOMMAND, Struct.SCType.SC_MAXIMIZE, IntPtr.Zero)
            End If
        End Sub

        Private Sub Resize_BottomCenter_PreviewMouseEvent(sender As Object, arg As MouseEventArgs, e As MouseEvent) Handles Resize_BottomCenter.PreviewMouseEvent
            Resize_Event(arg, e, Cursors.SizeNS, Struct.SCType.SC_DRAG_RESIZED)
        End Sub

        Private Sub Resize_BottomLeft_PreviewMouseEvent(sender As Object, arg As MouseEventArgs, e As MouseEvent) Handles Resize_BottomLeft.PreviewMouseEvent
            Resize_Event(arg, e, Cursors.SizeNESW, Struct.SCType.SC_DRAG_RESIZEDL)
        End Sub

        Private Sub Resize_BottomRight_PreviewMouseEvent(sender As Object, arg As MouseEventArgs, e As MouseEvent) Handles Resize_BottomRight.PreviewMouseEvent
            Resize_Event(arg, e, Cursors.SizeNWSE, Struct.SCType.SC_DRAG_RESIZEDR)
        End Sub

        Private Sub Resize_CenterLeft_PreviewMouseEvent(sender As Object, arg As MouseEventArgs, e As MouseEvent) Handles Resize_CenterLeft.PreviewMouseEvent
            Resize_Event(arg, e, Cursors.SizeWE, Struct.SCType.SC_DRAG_RESIZEL)
        End Sub

        Private Sub Resize_CenterRight_PreviewMouseEvent(sender As Object, arg As MouseEventArgs, e As MouseEvent) Handles Resize_CenterRight.PreviewMouseEvent
            Resize_Event(arg, e, Cursors.SizeWE, Struct.SCType.SC_DRAG_RESIZER)
        End Sub

        Private Sub Resize_TopCenter_PreviewMouseEvent(sender As Object, arg As MouseEventArgs, e As MouseEvent) Handles Resize_TopCenter.PreviewMouseEvent
            Resize_Event(arg, e, Cursors.SizeNS, Struct.SCType.SC_DRAG_RESIZEU)
        End Sub

        Private Sub Resize_TopLeft_PreviewMouseEvent(sender As Object, arg As MouseEventArgs, e As MouseEvent) Handles Resize_TopLeft.PreviewMouseEvent
            Resize_Event(arg, e, Cursors.SizeNWSE, Struct.SCType.SC_DRAG_RESIZEUL)
        End Sub

        Private Sub Resize_TopRight_PreviewMouseEvent(sender As Object, arg As MouseEventArgs, e As MouseEvent) Handles Resize_TopRight.PreviewMouseEvent
            Resize_Event(arg, e, Cursors.SizeNESW, Struct.SCType.SC_DRAG_RESIZEUR)
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
#End Region

#Region " [ Render ] "
        Private Sub FormSkinBase_SizeChanged(sender As Object, e As EventArgs) Handles Me.SizeChanged
            Me.Invalidate()
        End Sub

        Private Sub Windows10_Paint(sender As Object, e As PaintEventArgs) Handles Me.Paint
            Using buffer As BufferedGraphics = BufferedGraphicsManager.Current.Allocate(e.Graphics, Me.ClientRectangle)
                Dim g As Graphics = buffer.Graphics
                Dim gCon As GraphicsContainer = g.BeginContainer

                g.InterpolationMode = InterpolationMode.High

                UpdateLayout()
                OnDraw(g)

                g.EndContainer(gCon)
                buffer.Render()

                buffer.Dispose()
                g.Dispose()
            End Using
        End Sub

        <Description("스킨 구성요소를 렌더링합니다.")>
        Protected Overridable Sub OnDraw(g As Graphics)
        End Sub

        <Description("스킨 구성요소를 재배치합니다.")>
        Protected Overridable Sub UpdateLayout()
            Titlebar_Area.EventRect.Height = If(IsMaximized, 23, 30)

            Titlebar_Close.EventRect.Height = Titlebar_Area.Rect.Height - If(IsMaximized, 2, 1)
            Titlebar_Maximize.EventRect.Height = Titlebar_Close.Rect.Height
            Titlebar_Minimize.EventRect.Height = Titlebar_Close.Rect.Height

            Titlebar_Close.EventRect.X = DrawingSize.Width - Titlebar_Close.Rect.Width
            Titlebar_Maximize.EventRect.X = Titlebar_Close.Rect.X - Titlebar_Maximize.Rect.Width - 1
            Titlebar_Minimize.EventRect.X = Titlebar_Maximize.Rect.X - Titlebar_Minimize.Rect.Width - 1

            Titlebar_Area.EventRect.Width = Titlebar_Minimize.Rect.X - 1

            Titlebar_Close.EventRect.Y = DrawingPoint.Y
            Titlebar_Maximize.EventRect.Y = DrawingPoint.Y
            Titlebar_Minimize.EventRect.Y = DrawingPoint.Y
            Titlebar_Area.EventRect.Y = DrawingPoint.Y

            UpdateResize()

            Resizers.ForEach(
                Sub(er As MouseCaptureControl)
                    er.IsEnabled = Not IsMaximized
                End Sub)
        End Sub

        Protected Overridable Sub UpdateResize()
            ' 모서리
            Resize_TopRight.EventRect.X = Me.Width - Resize_TopRight.EventRect.Width - DrawingPoint.X
            Resize_BottomLeft.EventRect.Y = Me.Height - Resize_BottomLeft.EventRect.Height - DrawingPoint.X
            Resize_BottomRight.EventRect.X = Resize_TopRight.EventRect.X
            Resize_BottomRight.EventRect.Y = Resize_BottomLeft.EventRect.Y

            ' 위아래/양옆
            Resize_TopCenter.EventRect.X = Resize_TopLeft.EventRect.Right
            Resize_TopCenter.EventRect.Width = Resize_TopRight.EventRect.X - Resize_TopLeft.EventRect.Right

            Resize_BottomCenter.EventRect.X = Resize_BottomLeft.EventRect.Right
            Resize_BottomCenter.EventRect.Width = Resize_BottomRight.EventRect.X - Resize_BottomLeft.EventRect.Right
            Resize_BottomCenter.EventRect.Y = Me.Height - Resize_BottomCenter.EventRect.Height - DrawingPoint.X

            Resize_CenterLeft.EventRect.Y = Resize_TopLeft.EventRect.Bottom
            Resize_CenterLeft.EventRect.Height = Resize_BottomLeft.EventRect.Y - Resize_TopLeft.EventRect.Bottom

            Resize_CenterRight.EventRect.Y = Resize_TopRight.EventRect.Bottom
            Resize_CenterRight.EventRect.Height = Resize_BottomRight.EventRect.Y - Resize_TopRight.EventRect.Bottom
            Resize_CenterRight.EventRect.X = Me.Width - Resize_CenterRight.EventRect.Width - DrawingPoint.X
        End Sub
#End Region

#Region " [ Parent ] "
        Private Sub _ParentWindow_SizeChanged(sender As Object, e As EventArgs) Handles _ParentWindow.SizeChanged
            Me.Invalidate()
        End Sub

        Private Sub _ParentWindow_TextChanged(sender As Object, e As EventArgs) Handles _ParentWindow.TextChanged
            Me.Invalidate()
        End Sub

        Private Sub _ParentWindow_Activated(sender As Object, e As EventArgs) Handles _ParentWindow.Activated
            _isActivated = True
            Me.Invalidate()
        End Sub

        Private Sub _ParentWindow_Deactivate(sender As Object, e As EventArgs) Handles _ParentWindow.Deactivate
            _isActivated = False
            Me.Invalidate()
        End Sub

        Private Sub _ParentWindow_Load(sender As Object, e As EventArgs) Handles _ParentWindow.Load
            'ParentWindow.FormBorderStyle = FormBorderStyle.Sizable

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

        Private Sub Parent_GETMINMAXINFO(hwnd As IntPtr, ByRef wParam As IntPtr, ByRef lParam As IntPtr, ByRef Result As IntPtr, ByRef cancel As Boolean)
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

        Private Sub Parent_NCCALSIZE(hwnd As IntPtr, ByRef wParam As IntPtr, ByRef lParam As IntPtr, ByRef Result As IntPtr, ByRef cancel As Boolean)
            If UseAeroSnap Then
                If wParam.Equals(IntPtr.Zero) Then
                    Dim rc As Struct.RECT = _ParentWndHook.GetLParam(Of Struct.RECT)(lParam)
                    Dim r As Rectangle = rc.ToRectangle()
                    r.Inflate(7, 7)

                    Marshal.StructureToPtr(New Struct.RECT(r), lParam, True)
                Else
                    Dim csp As Struct.NCCALCSIZE_PARAMS = _ParentWndHook.GetLParam(Of Struct.NCCALCSIZE_PARAMS)(lParam)
                    Dim r As Rectangle = csp.rgrc0.ToRectangle()
                    r.Inflate(7, 7)
                    csp.rgrc0 = New Struct.RECT(r)

                    Marshal.StructureToPtr(csp, lParam, True)
                End If

                Result = IntPtr.Zero
            End If
        End Sub
#End Region

#Region " [ Function ] "
        Private Sub ResetTitlebarMouseState()
            Titlebar_Maximize_State = MouseState.None
            Titlebar_Minimize_State = MouseState.None
            Titlebar_Close_State = MouseState.None

            Me.Invalidate()
        End Sub

        Protected Function LoadResource(Of T)(path As String) As T
            Return Assembly.AssemblyResource.GetData(Of T)(path)
        End Function
#End Region

    End Class
End Namespace