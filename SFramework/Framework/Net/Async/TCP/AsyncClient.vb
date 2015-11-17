Imports System.Net
Imports System.Net.Sockets
Imports System.Text
Imports System.Threading

Namespace Net.Sockets.TCP
    Public Class AsyncClient
        Implements IClient

        Public Event Connected(sender As IClient) Implements IClient.Connected
        Public Event Disconnected(sender As IClient) Implements IClient.Disconnected
        Public Event OnPacketReceived(sender As IClient, packet() As Byte) Implements IClient.OnPacketReceived

        Private mBuffer() As Byte
        Private mLifeCycle As Timer

        Private _connected As Boolean = False
        Private _bufferSize As Integer = 1024
        Private _ip As String = "0.0.0.0"
        Private _ep As IPEndPoint
        Private _sid As String
        Private _Socket As Socket
        Private _State As ClientState = ClientState.None

        Sub New(socket As Socket, bufferSize As Integer)
            _Socket = socket
            _connected = socket.Connected

            If IsConnected Then
                _State = ClientState.Wait

                _bufferSize = bufferSize

                Initialize()
            End If
        End Sub

        Sub New(Optional bufferSize As Integer = 1024)
            _bufferSize = bufferSize
        End Sub

        Public Sub Close() Implements IClient.Close
            If _connected Then
                If _Socket IsNot Nothing Then
                    _Socket.Shutdown(SocketShutdown.Both)
                    _Socket.Disconnect(False)
                    _Socket.Close()
                    _Socket.Dispose()

                    _Socket = Nothing
                End If

                _connected = False

                RaiseEvent Disconnected(Me)
            End If
        End Sub

        Public Sub Connect(ip As String, port As Integer)
            Dim ep As New IPEndPoint(IPAddress.Parse(ip), port)
            Dim callback As New AsyncCallback(AddressOf OnConnected)

            _Socket = New Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp)

            _Socket.BeginConnect(ep, callback, Me)
        End Sub

        Private Sub Initialize()
            If Socket?.Connected Then
                _ep = Socket.RemoteEndPoint
                _ip = _ep.ToString.Split(":")(0)

                mBuffer = New Byte(BufferSize - 1) {}

                NextReceive()
                '// 1차 인증 시작
            End If
        End Sub

        Private Sub OnConnected(ar As IAsyncResult)
            If _Socket.Connected Then
                _connected = True

                _Socket.EndConnect(ar)

                _State = ClientState.Wait

                NextPoll()
                Initialize()

                RaiseEvent Connected(Me)
            End If
        End Sub

        Public ReadOnly Property SessionID As String Implements IClient.SessionID
            Get
                Return _sid
            End Get
        End Property

        Public ReadOnly Property Socket As Socket Implements IClient.Socket
            Get
                Return _Socket
            End Get
        End Property

        Public ReadOnly Property State As ClientState Implements IClient.State
            Get
                Return _State
            End Get
        End Property

        Public ReadOnly Property IP As String Implements IClient.IP
            Get
                Return _ip
            End Get
        End Property

        Public ReadOnly Property BufferSize As Integer
            Get
                Return _bufferSize
            End Get
        End Property

        Public ReadOnly Property IsConnected As Boolean Implements IClient.IsConnected
            Get
                Return _connected
            End Get
        End Property

        Public ReadOnly Property RemoteEndPoint As IPEndPoint Implements IClient.RemoteEndPoint
            Get
                Return _ep
            End Get
        End Property

        Public Sub Send(packet() As Byte) Implements IClient.Send
            Dim callback As New AsyncCallback(AddressOf OnSended)

            Try
                Socket.BeginSend(packet, 0, packet.Length, SocketFlags.None, callback, packet)
            Catch ex As Exception
                Me.Close()
            End Try
        End Sub

        Public Sub Send(message As String, Optional encoding As Encoding = Nothing) Implements IClient.Send
            If encoding Is Nothing Then encoding = Encoding.Default

            Send(encoding.GetBytes(message))
        End Sub

        Private Sub OnSended(ar As IAsyncResult)
            Dim packet() As Byte = ar.AsyncState

            Try
                Socket.EndSend(ar)
            Catch
            End Try
        End Sub

        Private Sub NextReceive()
            Dim callback As New AsyncCallback(AddressOf EndReceive)

            Try
                Socket.BeginReceive(mBuffer, 0, mBuffer.Length, SocketFlags.None, callback, Me)
            Catch ex As Exception
                Me.Close()
            End Try
        End Sub

        Private Sub EndReceive(ar As IAsyncResult)
            Try
                Dim read As Integer = Socket.EndReceive(ar)

                If read > 0 Then
                    RaiseEvent OnPacketReceived(Me, mBuffer)

                    Array.Clear(mBuffer, 0, mBuffer.Length)

                    NextReceive()
                Else
                    Me.Close()
                End If
            Catch ex As Exception
            End Try
        End Sub

        '// 단일 클라이언트용 라이프 사이클
        Private Sub NextPoll()
            Dim callback As New TimerCallback(AddressOf EndPoll)
            Dim t As New Timer(callback, Nothing, 1000, Timeout.Infinite)
        End Sub

        Private Sub EndPoll(state As Object)
            If Not LifeCheck() Then
                Me.Close()
            Else
                NextPoll()
            End If
        End Sub

        Protected Friend Function LifeCheck() As Boolean
            Dim p As Boolean = False

            Try
                p = Not (Socket?.Poll(0, SelectMode.SelectRead) AndAlso Socket?.Available = 0)
            Catch
                p = False
            End Try

            Return p
        End Function
    End Class
End Namespace