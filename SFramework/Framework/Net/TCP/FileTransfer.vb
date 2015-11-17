Imports System.IO
Imports System.Net.NetworkInformation

Namespace Net.Sockets.TCP
    Public Class FileTransfer
        Implements IFileTransfer

        Private Const MinPort As Integer = 1000
        Private Const MaxPort As Integer = 65535

        Private parentService As TCPService

        Private _sessions As New List(Of IClient)
        Private _service As TCPService
        Private _serviceType As TCPServiceType
        Private _port As Integer
        Private _deadEnd As Date

        Private mState As FileTransferState = FileTransferState.None
        Private mCreated As date
        Private WithEvents mServer As IServer

        Public ReadOnly Property DeadEnd As Date
            Get
                Return _deadEnd
            End Get
        End Property

        Public ReadOnly Property Port As Integer
            Get
                Return _port
            End Get
        End Property

        Public ReadOnly Property ServiceType As TCPServiceType
            Get
                Return _serviceType
            End Get
        End Property

        Public ReadOnly Property Service As TCPService
            Get
                Return _service
            End Get
        End Property

        Sub New(parent As TCPService, client As IClient, type As TCPServiceType)
            parentService = parent
            _sessions.Add(client)
            _serviceType = type

            Initiailize()
        End Sub

        Sub New(parent As TCPService, clients() As IClient, type As TCPServiceType)
            parentService = parent
            _sessions.AddRange(clients)
            _serviceType = type

            Initiailize()
        End Sub

        Private Sub Initiailize()
            _service = New TCPService(ServiceType, _sessions.Count, 1024)
            _port = GetPort()

            Service.Start(Port)

            mServer = Service.Server
            mCreated = DateTime.Now
            _deadEnd = mCreated.AddMinutes(3)
        End Sub

        Private Function GetPort() As Integer
            Dim ports() As Integer =
                IPGlobalProperties.
                GetIPGlobalProperties.
                GetActiveTcpConnections.
                Select(Function(tcpi As TcpConnectionInformation) tcpi.LocalEndPoint.Port).ToArray

            ports = ports.Concat({parentService.Server.Port}).Distinct.ToArray

            If ports.Count = 65535 Then
                Throw New Exception("사용가능한 포트가 없습니다.")
                Return -1
            End If

            Dim r As New Random

            Do
                Dim port As Integer = MinPort + r.Next * (MaxPort - MinPort)

                If Not ports.Contains(port) Then
                    Return port

                    Exit Do
                End If
            Loop
        End Function

        Private Iterator Function GetFileBlocks(fileName As String, size As Integer) As IEnumerable(Of Byte())
            If Not File.Exists(fileName) Then Throw New FileNotFoundException

            Using fs As New FileStream(fileName, FileMode.Open)
                Dim offset As ULong = 0

                Do
                    fs.Seek(offset, SeekOrigin.Begin)

                    Dim readSize As Integer = Math.Min(fs.Length - offset, size)

                    If readSize > 0 Then
                        Dim block(readSize - 1) As Byte
                        Dim read As Integer = fs.Read(block, 0, block.Length)

                        Yield block
                    Else
                        Exit Do
                    End If
                Loop
            End Using
        End Function

        Private Sub WriteBlocks(fs As FileStream, block() As Byte)
            If fs.CanSeek Then
                fs.Write(block, 0, block.Length)
                fs.Seek(fs.Position, SeekOrigin.Begin)
            End If
        End Sub

        Private Sub mServer_Connected(sender As Object, session As IClient) Handles mServer.Connected

        End Sub

        Private Sub mServer_Disconnected(sender As Object, session As IClient) Handles mServer.Disconnected
            'RaiseEvent Failed(Me)
        End Sub

        Private Sub mServer_OnPacketReceived(sender As Object, session As IClient, packet() As Byte) Handles mServer.OnPacketReceived

        End Sub
    End Class
End Namespace