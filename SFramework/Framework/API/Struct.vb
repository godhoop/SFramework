Imports System.Drawing
Imports System.Runtime.InteropServices

Namespace API
    Public MustInherit Class Struct
        <StructLayout(LayoutKind.Sequential)>
        Structure MARGINS
            Public leftWidth As Integer
            Public rightWidth As Integer
            Public topHeight As Integer
            Public bottomHeight As Integer
        End Structure

        <StructLayout(LayoutKind.Sequential)>
        Structure NCCALCSIZE_PARAMS
            Public rgrc0 As RECT, rgrc1 As RECT, rgrc2 As RECT
            Public lppos As WINDOWPOS
        End Structure

        <StructLayout(LayoutKind.Sequential)>
        Structure WINDOWPOS
            Public hWnd As IntPtr, hWndInsertAfter As IntPtr
            Public x As Integer, y As Integer, cx As Integer, cy As Integer, flags As Integer
        End Structure

        <Flags>
        Enum SetWindowPosFlags As UInteger
            SynchronousWindowPosition = &H4000
            DeferErase = &H2000
            DrawFrame = &H20
            FrameChanged = &H20
            HideWindow = &H80
            DoNotActivate = &H10
            DoNotCopyBits = &H100
            IgnoreMove = &H2
            DoNotChangeOwnerZOrder = &H200
            DoNotRedraw = &H8
            DoNotReposition = &H200
            DoNotSendChangingEvent = &H400
            IgnoreResize = &H1
            IgnoreZOrder = &H4
            ShowWindow = &H40
        End Enum

        <Flags()>
        Enum WindowStyles As UInteger
            WS_BORDER = &H800000
            WS_CAPTION = &HC00000
            WS_CHILD = &H40000000
            WS_CLIPCHILDREN = &H2000000
            WS_CLIPSIBLINGS = &H4000000
            WS_DISABLED = &H8000000
            WS_DLGFRAME = &H400000
            WS_GROUP = &H20000
            WS_HSCROLL = &H100000
            WS_MAXIMIZE = &H1000000
            WS_MAXIMIZEBOX = &H10000
            WS_MINIMIZE = &H20000000
            WS_MINIMIZEBOX = &H20000
            WS_OVERLAPPED = &H0
            WS_OVERLAPPEDWINDOW = WS_OVERLAPPED Or WS_CAPTION Or WS_SYSMENU Or WS_SIZEFRAME Or WS_MINIMIZEBOX Or WS_MAXIMIZEBOX
            WS_POPUP = &H80000000UI
            WS_POPUPWINDOW = WS_POPUP Or WS_BORDER Or WS_SYSMENU
            WS_SIZEFRAME = &H40000
            WS_SYSMENU = &H80000
            WS_TABSTOP = &H10000
            WS_VISIBLE = &H10000000
            WS_VSCROLL = &H200000
            WS_THICKFRAME = 262144
        End Enum

        <Flags()>
        Enum WindowStylesEx As UInteger
            WS_EX_ACCEPTFILES = &H10
            WS_EX_APPWINDOW = &H40000
            WS_EX_CLIENTEDGE = &H200
            WS_EX_COMPOSITED = &H2000000
            WS_EX_CONTEXTHELP = &H400
            WS_EX_CONTROLPARENT = &H10000
            WS_EX_DLGMODALFRAME = &H1
            WS_EX_LAYERED = &H80000
            WS_EX_LAYOUTRTL = &H400000
            WS_EX_LEFT = &H0
            WS_EX_LEFTSCROLLBAR = &H4000
            WS_EX_LTRREADING = &H0
            WS_EX_MDICHILD = &H40
            WS_EX_NOACTIVATE = &H8000000
            WS_EX_NOINHERITLAYOUT = &H100000
            WS_EX_NOPARENTNOTIFY = &H4
            WS_EX_OVERLAPPEDWINDOW = WS_EX_WINDOWEDGE Or WS_EX_CLIENTEDGE
            WS_EX_PALETTEWINDOW = WS_EX_WINDOWEDGE Or WS_EX_TOOLWINDOW Or WS_EX_TOPMOST
            WS_EX_RIGHT = &H1000
            WS_EX_RIGHTSCROLLBAR = &H0
            WS_EX_RTLREADING = &H2000
            WS_EX_STATICEDGE = &H20000
            WS_EX_TOOLWINDOW = &H80
            WS_EX_TOPMOST = &H8
            WS_EX_TRANSPARENT = &H20
            WS_EX_WINDOWEDGE = &H100
        End Enum

        <StructLayout(LayoutKind.Sequential, CharSet:=CharSet.Auto)>
        Class MONITORINFO
            Public cbSize As Integer = Marshal.SizeOf(GetType(MONITORINFO))
            Public rcMonitor As RECT
            Public rcWork As RECT
            Public dwFlags As Integer
        End Class

        <StructLayout(LayoutKind.Sequential)>
        Structure MINMAXINFO
            Public ptReserved As POINT
            Public ptMaxSize As POINT
            Public ptMaxPosition As POINT
            Public ptMinTrackSize As POINT
            Public ptMaxTrackSize As POINT
        End Structure

        <StructLayout(LayoutKind.Sequential)>
        Structure POINT
            Public X As Integer
            Public Y As Integer

            Public Sub New(ByVal X As Integer, ByVal Y As Integer)
                Me.X = X
                Me.Y = Y
            End Sub

            Public Sub New(ByVal point As System.Drawing.Point)
                Me.X = point.X
                Me.Y = point.Y
            End Sub

            Public Overrides Function ToString() As String
                Return "{" & String.Format("X={0}, Y={1}", Me.X, Me.Y) & "}"
            End Function

            Public Function ToDrawingPoint() As System.Drawing.Point
                Return New System.Drawing.Point(Me.X, Me.Y)
            End Function

            Public Shared Operator =(ByVal point1 As POINT, ByVal potint2 As POINT) As Boolean
                Return point1.Equals(potint2)
            End Operator

            Public Shared Operator <>(ByVal point1 As POINT, ByVal potint2 As POINT) As Boolean
                Return Not point1.Equals(potint2)
            End Operator

            Public Overloads Function Equals(ByVal point As POINT) As Boolean
                Return Me.X = point.X AndAlso Me.Y = point.Y
            End Function

            Public Overloads Overrides Function Equals(ByVal [Object] As Object) As Boolean
                If TypeOf [Object] Is POINT Then
                    Return Equals(DirectCast([Object], POINT))
                ElseIf TypeOf [Object] Is System.Drawing.Point Then
                    Return Equals(New POINT(DirectCast([Object], System.Drawing.Point)))
                End If

                Return False
            End Function
        End Structure

        <StructLayout(LayoutKind.Sequential)>
        Structure RECT
            Private _Left As Integer, _Top As Integer, _Right As Integer, _Bottom As Integer

            Public Sub New(ByVal Rectangle As System.Drawing.Rectangle)
                Me.New(Rectangle.Left, Rectangle.Top, Rectangle.Right, Rectangle.Bottom)
            End Sub
            Public Sub New(ByVal Left As Integer, ByVal Top As Integer, ByVal Right As Integer, ByVal Bottom As Integer)
                _Left = Left
                _Top = Top
                _Right = Right
                _Bottom = Bottom
            End Sub

            Public Function ToRectangle() As Rectangle
                Return New Rectangle(X, Y, Width, Height)
            End Function

            Public Property X As Integer
                Get
                    Return _Left
                End Get
                Set(ByVal value As Integer)
                    _Right = _Right - _Left + value
                    _Left = value
                End Set
            End Property
            Public Property Y As Integer
                Get
                    Return _Top
                End Get
                Set(ByVal value As Integer)
                    _Bottom = _Bottom - _Top + value
                    _Top = value
                End Set
            End Property
            Public Property Left As Integer
                Get
                    Return _Left
                End Get
                Set(ByVal value As Integer)
                    _Left = value
                End Set
            End Property
            Public Property Top As Integer
                Get
                    Return _Top
                End Get
                Set(ByVal value As Integer)
                    _Top = value
                End Set
            End Property
            Public Property Right As Integer
                Get
                    Return _Right
                End Get
                Set(ByVal value As Integer)
                    _Right = value
                End Set
            End Property
            Public Property Bottom As Integer
                Get
                    Return _Bottom
                End Get
                Set(ByVal value As Integer)
                    _Bottom = value
                End Set
            End Property
            Public Property Height() As Integer
                Get
                    Return _Bottom - _Top
                End Get
                Set(ByVal value As Integer)
                    _Bottom = value + _Top
                End Set
            End Property
            Public Property Width() As Integer
                Get
                    Return _Right - _Left
                End Get
                Set(ByVal value As Integer)
                    _Right = value + _Left
                End Set
            End Property
            Public Property Location() As POINT
                Get
                    Return New POINT(Left, Top)
                End Get
                Set(ByVal value As POINT)
                    _Right = _Right - _Left + value.X
                    _Bottom = _Bottom - _Top + value.Y
                    _Left = value.X
                    _Top = value.Y
                End Set
            End Property
            Public Property Size() As Size
                Get
                    Return New Size(Width, Height)
                End Get
                Set(ByVal value As Size)
                    _Right = value.Width + _Left
                    _Bottom = value.Height + _Top
                End Set
            End Property

            Public Shared Widening Operator CType(ByVal Rectangle As RECT) As System.Drawing.Rectangle
                Return New System.Drawing.Rectangle(Rectangle.Left, Rectangle.Top, Rectangle.Width, Rectangle.Height)
            End Operator
            Public Shared Widening Operator CType(ByVal Rectangle As System.Drawing.Rectangle) As RECT
                Return New RECT(Rectangle.Left, Rectangle.Top, Rectangle.Right, Rectangle.Bottom)
            End Operator
            Public Shared Operator =(ByVal Rectangle1 As RECT, ByVal Rectangle2 As RECT) As Boolean
                Return Rectangle1.Equals(Rectangle2)
            End Operator
            Public Shared Operator <>(ByVal Rectangle1 As RECT, ByVal Rectangle2 As RECT) As Boolean
                Return Not Rectangle1.Equals(Rectangle2)
            End Operator

            Public Overrides Function ToString() As String
                Return "{Left: " & _Left & "; " & "Top: " & _Top & "; Right: " & _Right & "; Bottom: " & _Bottom & "}"
            End Function

            Public Overloads Function Equals(ByVal Rectangle As RECT) As Boolean
                Return Rectangle.Left = _Left AndAlso Rectangle.Top = _Top AndAlso Rectangle.Right = _Right AndAlso Rectangle.Bottom = _Bottom
            End Function

            Public Overloads Overrides Function Equals(ByVal [Object] As Object) As Boolean
                If TypeOf [Object] Is RECT Then
                    Return Equals(DirectCast([Object], RECT))
                ElseIf TypeOf [Object] Is Rectangle Then
                    Return Equals(New RECT(DirectCast([Object], System.Drawing.Rectangle)))
                End If

                Return False
            End Function
        End Structure

        <StructLayout(LayoutKind.Sequential)>
        Structure CWPSTRUCT
            Public lParam As IntPtr
            Public wParam As IntPtr
            Public message As Integer
            Public hWnd As IntPtr
        End Structure

        <StructLayout(LayoutKind.Sequential)>
        Structure CWPRETSTRUCT
            Public lResult As IntPtr
            Public lParam As IntPtr
            Public wParam As IntPtr
            Public message As UInteger
            Public hWnd As IntPtr
        End Structure

        <Flags>
        Enum HookType As Integer
            WH_JOURNALRECORD = 0
            WH_JOURNALPLAYBACK = 1
            WH_KEYBOARD = 2
            WH_GETMESSAGE = 3
            WH_CALLWNDPROC = 4
            WH_CBT = 5
            WH_SYSMSGFILTER = 6
            WH_MOUSE = 7
            WH_HARDWARE = 8
            WH_DEBUG = 9
            WH_SHELL = 10
            WH_FOREGROUNDIDLE = 11
            WH_CALLWNDPROCRET = 12
            WH_KEYBOARD_LL = 13
            WH_MOUSE_LL = 14
        End Enum

        <Flags>
        Enum SCType As UInteger
            SC_SIZE = &HF000
            SC_MOVE = &HF010
            SC_MINIMIZE = &HF020
            SC_MAXIMIZE = &HF030
            SC_NEXTWINDOW = &HF040
            SC_PREVWINDOW = &HF050
            SC_CLOSE = &HF060
            SC_VSCROLl = &HF070
            SC_HSCROLL = &HF080
            SC_MOUSEMENU = &HF090
            SC_KEYMENU = &HF100
            SC_ARRANGE = &HF110
            SC_RESTORE = &HF120
            SC_TASKLIST = &HF130
            SC_SCREENSAVE = &HF140
            SC_HOTKEY = &HF150
            SC_DEFAULT = &HF160
            SC_MONITORPOWER = &HF170
            SC_CONTEXTHELP = &HF180
            SC_SEPARATOR = &HF00F

            SC_DRAG_RESIZEL = 61441UI
            SC_DRAG_RESIZER = 61442UI
            SC_DRAG_RESIZEU = 61443UI
            SC_DRAG_RESIZEUL = 61444UI
            SC_DRAG_RESIZEUR = 61445UI
            SC_DRAG_RESIZED = 61446UI
            SC_DRAG_RESIZEDL = 61447UI
            SC_DRAG_RESIZEDR = 61448UI
        End Enum

        <Flags>
        Enum WindowLongFlags As Integer
            GWL_EXSTYLE = -20
            GWLP_HINSTANCE = -6
            GWLP_HWNDPARENT = -8
            GWL_ID = -12
            GWL_STYLE = -16
            GWL_USERDATA = -21
            GWL_WNDPROC = -4
            DWLP_USER = &H8
            DWLP_MSGRESULT = &H0
            DWLP_DLGPROC = &H4
        End Enum

        <Flags>
        Enum WindowsMessages As UInteger
            WM_NULL = &H00
            WM_CREATE = &H01
            WM_DESTROY = &H02
            WM_MOVE = &H03
            WM_SIZE = &H05
            WM_ACTIVATE = &H06
            WM_SETFOCUS = &H07
            WM_KILLFOCUS = &H08
            WM_ENABLE = &H0A
            WM_SETREDRAW = &H0B
            WM_SETTEXT = &H0C
            WM_GETTEXT = &H0D
            WM_GETTEXTLENGTH = &H0E
            WM_PAINT = &H0F
            WM_CLOSE = &H10
            WM_QUERYENDSESSION = &H11
            WM_QUIT = &H12
            WM_QUERYOPEN = &H13
            WM_ERASEBKGND = &H14
            WM_SYSCOLORCHANGE = &H15
            WM_ENDSESSION = &H16
            WM_SYSTEMERROR = &H17
            WM_SHOWWINDOW = &H18
            WM_CTLCOLOR = &H19
            WM_WININICHANGE = &H1A
            WM_SETTINGCHANGE = &H1A
            WM_DEVMODECHANGE = &H1B
            WM_ACTIVATEAPP = &H1C
            WM_FONTCHANGE = &H1D
            WM_TIMECHANGE = &H1E
            WM_CANCELMODE = &H1F
            WM_SETCURSOR = &H20
            WM_MOUSEACTIVATE = &H21
            WM_CHILDACTIVATE = &H22
            WM_QUEUESYNC = &H23
            WM_GETMINMAXINFO = &H24
            WM_PAINTICON = &H26
            WM_ICONERASEBKGND = &H27
            WM_NEXTDLGCTL = &H28
            WM_SPOOLERSTATUS = &H2A
            WM_DRAWITEM = &H2B
            WM_MEASUREITEM = &H2C
            WM_DELETEITEM = &H2D
            WM_VKEYTOITEM = &H2E
            WM_CHARTOITEM = &H2F
            WM_SETFONT = &H30
            WM_GETFONT = &H31
            WM_SETHOTKEY = &H32
            WM_GETHOTKEY = &H33
            WM_QUERYDRAGICON = &H37
            WM_COMPAREITEM = &H39
            WM_COMPACTING = &H41
            WM_WINDOWPOSCHANGING = &H46
            WM_WINDOWPOSCHANGED = &H47
            WM_POWER = &H48
            WM_COPYDATA = &H4A
            WM_CANCELJOURNAL = &H4B
            WM_NOTIFY = &H4E
            WM_INPUTLANGCHANGEREQUEST = &H50
            WM_INPUTLANGCHANGE = &H51
            WM_TCARD = &H52
            WM_HELP = &H53
            WM_USERCHANGED = &H54
            WM_NOTIFYFORMAT = &H55
            WM_CONTEXTMENU = &H7B
            WM_STYLECHANGING = &H7C
            WM_STYLECHANGED = &H7D
            WM_DISPLAYCHANGE = &H7E
            WM_GETICON = &H7F
            WM_SETICON = &H80
            WM_NCCREATE = &H81
            WM_NCDESTROY = &H82
            WM_NCCALCSIZE = &H83
            WM_NCHITTEST = &H84
            WM_NCPAINT = &H85
            WM_NCACTIVATE = &H86
            WM_GETDLGCODE = &H87
            WM_NCMOUSEMOVE = &HA0
            WM_NCLBUTTONDOWN = &HA1
            WM_NCLBUTTONUP = &HA2
            WM_NCLBUTTONDBLCLK = &HA3
            WM_NCRBUTTONDOWN = &HA4
            WM_NCRBUTTONUP = &HA5
            WM_NCRBUTTONDBLCLK = &HA6
            WM_NCMBUTTONDOWN = &HA7
            WM_NCMBUTTONUP = &HA8
            WM_NCMBUTTONDBLCLK = &HA9
            WM_KEYFIRST = &H100
            WM_KEYDOWN = &H100
            WM_KEYUP = &H101
            WM_CHAR = &H102
            WM_DEADCHAR = &H103
            WM_SYSKEYDOWN = &H104
            WM_SYSKEYUP = &H105
            WM_SYSCHAR = &H106
            WM_SYSDEADCHAR = &H107
            WM_KEYLAST = &H108
            WM_IME_STARTCOMPOSITION = &H10D
            WM_IME_ENDCOMPOSITION = &H10E
            WM_IME_COMPOSITION = &H10F
            WM_IME_KEYLAST = &H10F
            WM_INITDIALOG = &H110
            WM_COMMAND = &H111
            WM_SYSCOMMAND = &H112
            WM_TIMER = &H113
            WM_HSCROLL = &H114
            WM_VSCROLL = &H115
            WM_INITMENU = &H116
            WM_INITMENUPOPUP = &H117
            WM_MENUSELECT = &H11F
            WM_MENUCHAR = &H120
            WM_ENTERIDLE = &H121
            WM_CTLCOLORMSGBOX = &H132
            WM_CTLCOLOREDIT = &H133
            WM_CTLCOLORLISTBOX = &H134
            WM_CTLCOLORBTN = &H135
            WM_CTLCOLORDLG = &H136
            WM_CTLCOLORSCROLLBAR = &H137
            WM_CTLCOLORSTATIC = &H138
            WM_MOUSEFIRST = &H200
            WM_MOUSEMOVE = &H200
            WM_LBUTTONDOWN = &H201
            WM_LBUTTONUP = &H202
            WM_LBUTTONDBLCLK = &H203
            WM_RBUTTONDOWN = &H204
            WM_RBUTTONUP = &H205
            WM_RBUTTONDBLCLK = &H206
            WM_MBUTTONDOWN = &H207
            WM_MBUTTONUP = &H208
            WM_MBUTTONDBLCLK = &H209
            WM_MOUSEWHEEL = &H20A
            WM_MOUSEHWHEEL = &H20E
            WM_PARENTNOTIFY = &H210
            WM_ENTERMENULOOP = &H211
            WM_EXITMENULOOP = &H212
            WM_NEXTMENU = &H213
            WM_SIZING = &H214
            WM_CAPTURECHANGED = &H215
            WM_MOVING = &H216
            WM_POWERBROADCAST = &H218
            WM_DEVICECHANGE = &H219
            WM_MDICREATE = &H220
            WM_MDIDESTROY = &H221
            WM_MDIACTIVATE = &H222
            WM_MDIRESTORE = &H223
            WM_MDINEXT = &H224
            WM_MDIMAXIMIZE = &H225
            WM_MDITILE = &H226
            WM_MDICASCADE = &H227
            WM_MDIICONARRANGE = &H228
            WM_MDIGETACTIVE = &H229
            WM_MDISETMENU = &H230
            WM_ENTERSIZEMOVE = &H231
            WM_EXITSIZEMOVE = &H232
            WM_DROPFILES = &H233
            WM_MDIREFRESHMENU = &H234
            WM_IME_SETCONTEXT = &H281
            WM_IME_NOTIFY = &H282
            WM_IME_CONTROL = &H283
            WM_IME_COMPOSITIONFULL = &H284
            WM_IME_SELECT = &H285
            WM_IME_CHAR = &H286
            WM_IME_KEYDOWN = &H290
            WM_IME_KEYUP = &H291
            WM_MOUSEHOVER = &H2A1
            WM_NCMOUSELEAVE = &H2A2
            WM_MOUSELEAVE = &H2A3
            WM_CUT = &H300
            WM_COPY = &H301
            WM_PASTE = &H302
            WM_CLEAR = &H303
            WM_UNDO = &H304
            WM_RENDERFORMAT = &H305
            WM_RENDERALLFORMATS = &H306
            WM_DESTROYCLIPBOARD = &H307
            WM_DRAWCLIPBOARD = &H308
            WM_PAINTCLIPBOARD = &H309
            WM_VSCROLLCLIPBOARD = &H30A
            WM_SIZECLIPBOARD = &H30B
            WM_ASKCBFORMATNAME = &H30C
            WM_CHANGECBCHAIN = &H30D
            WM_HSCROLLCLIPBOARD = &H30E
            WM_QUERYNEWPALETTE = &H30F
            WM_PALETTEISCHANGING = &H310
            WM_PALETTECHANGED = &H311
            WM_HOTKEY = &H312
            WM_PRINT = &H317
            WM_PRINTCLIENT = &H318
            WM_HANDHELDFIRST = &H358
            WM_HANDHELDLAST = &H35F
            WM_PENWINFIRST = &H380
            WM_PENWINLAST = &H38F
            WM_COALESCE_FIRST = &H390
            WM_COALESCE_LAST = &H39F
            WM_DDE_FIRST = &H3E0
            WM_DDE_INITIATE = &H3E0
            WM_DDE_TERMINATE = &H3E1
            WM_DDE_ADVISE = &H3E2
            WM_DDE_UNADVISE = &H3E3
            WM_DDE_ACK = &H3E4
            WM_DDE_DATA = &H3E5
            WM_DDE_REQUEST = &H3E6
            WM_DDE_POKE = &H3E7
            WM_DDE_EXECUTE = &H3E8
            WM_DDE_LAST = &H3E8
            WM_USER = &H400
            WM_APP = &H8000
        End Enum
    End Class
End Namespace