Imports System.Net
Imports System.Net.Sockets
Imports System.Text
Imports System.Threading

Namespace Net.Sockets.TCP
    Friend Class AsyncServer
        Implements IServer

        Public Event Connected(sender As Object, session As IClient) Implements IServer.Connected
        Public Event Disconnected(sender As Object, session As IClient) Implements IServer.Disconnected
        Public Event OnPacketReceived(sender As Object, session As IClient, packet() As Byte) Implements IServer.OnPacketReceived
        Public Event PreviewAccept(sender As Object, e As AcceptEventArgs) Implements IServer.PreviewAccept

        Private mMre As ManualResetEvent
        Private mLifeCycle As Thread

        Private _Socket As Socket
        Private _Sessions As List(Of IClient)
        Private _Port As Integer
        Private _IsWorking As Boolean = False
        Private _BufferSize As Integer

        Sub New(Optional bufferSize As Integer = 1024)
            _BufferSize = bufferSize
            _Sessions = New List(Of IClient)
        End Sub

        Public ReadOnly Property Port As Integer Implements IServer.Port
            Get
                Return _Port
            End Get
        End Property

        Public ReadOnly Property Sessions As IList(Of IClient) Implements IServer.Sessions
            Get
                Return _Sessions
            End Get
        End Property

        Public ReadOnly Property IsWorking As Boolean Implements IServer.IsWorking
            Get
                Return _IsWorking
            End Get
        End Property

        Public ReadOnly Property Socket As Socket Implements IServer.Socket
            Get
                Return _Socket
            End Get
        End Property

        Public ReadOnly Property BufferSize As Integer Implements IServer.BufferSize
            Get
                Return _BufferSize
            End Get
        End Property

        Public Overridable Sub BroadCast(packet() As Byte) Implements IServer.BroadCast
            For Each c As IClient In New ArrayList(Sessions)
                c.Send(packet)
            Next
        End Sub

        Public Sub BroadCast(message As String, Optional encoding As Encoding = Nothing) Implements IServer.BroadCast
            If encoding Is Nothing Then encoding = Encoding.Default

            BroadCast(encoding.GetBytes(message))
        End Sub

        Public Sub Start(port As Integer) Implements IServer.Start
            If Not IsWorking Then
                _Port = port

                Dim ep As New IPEndPoint(IPAddress.Any, port)

                _Socket = New Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp)
                Socket.Bind(ep)
                Socket.Listen(10000)

                NextAccept()

                mMre = New ManualResetEvent(False)

                mLifeCycle = New Thread(AddressOf ClientsLifeCycle)
                mLifeCycle.IsBackground = True
                mLifeCycle.Start()
            End If
        End Sub

        Public Sub [Stop]() Implements IServer.Stop
            If IsWorking Then
                If _Socket IsNot Nothing Then
                    For Each c As IClient In New ArrayList(Sessions)
                        c.Close()
                    Next

                    _Socket.Shutdown(SocketShutdown.Both)
                    _Socket.Close()
                    _Socket.Dispose()
                    _Socket = Nothing

                    _Sessions.Clear()
                End If

                If mLifeCycle IsNot Nothing Then
                    mLifeCycle.Abort()
                    mMre.Dispose()

                    mMre = Nothing
                    mLifeCycle = Nothing
                End If

                _IsWorking = False
            End If
        End Sub

        Private Sub ClientsLifeCycle()
            Dim sw As New Stopwatch
            sw.Start()

            Do
                mMre.Reset()

                If sw.ElapsedMilliseconds >= 100 Then
                    For Each c As AsyncClient In New ArrayList(Sessions)
                        If Not c.LifeCheck() Then
                            c.Close()
                        End If
                    Next

                    mMre.Set()

                    sw.Restart()
                End If

                mMre.WaitOne(10)
            Loop
        End Sub

        Protected Friend Overridable Sub NextAccept()
            Dim callback As New AsyncCallback(AddressOf OnAccept)
            Dim sid As String = SessionIDManager.CreateSessionID(30)

            Socket.BeginAccept(callback, sid)
        End Sub

        Protected Friend Overridable Sub OnAccept(ar As IAsyncResult)
            Dim sid As String = ar.AsyncState.ToString

            Dim aeArg As New AcceptEventArgs(Socket.EndAccept(ar))

            RaiseEvent PreviewAccept(Me, aeArg)

            If Not aeArg.Accept Then
                aeArg.Socket.Shutdown(SocketShutdown.Both)
                aeArg.Socket.Close()
            Else
                Dim client As New AsyncClient(aeArg.Socket, BufferSize)

                AddHandler client.OnPacketReceived, AddressOf _PacketReceived
                AddHandler client.Disconnected, AddressOf _Disconnected
                AddHandler client.Connected, AddressOf _Connected

                _Sessions.Add(client)

                RaiseEvent Connected(Me, client)
            End If

            NextAccept()
        End Sub

        Private Sub _Connected(sender As IClient)
            RaiseEvent Connected(Me, sender)
        End Sub

        Private Sub _Disconnected(sender As IClient)
            SyncLock _Sessions
                Sessions.Remove(sender)
            End SyncLock

            RaiseEvent Disconnected(Me, sender)
        End Sub

        Private Sub _PacketReceived(sender As IClient, packet() As Byte)
            RaiseEvent OnPacketReceived(Me, sender, packet)
        End Sub
    End Class
End Namespace