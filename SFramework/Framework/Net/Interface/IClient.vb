Imports System.Net.Sockets

Namespace Net
    Public Interface IClient
        Event Connected(sender As IClient)
        Event Disconnected(sender As IClient)
        Event OnPacketReceived(sender As IClient, packet As Byte())

        ReadOnly Property Socket As Socket
        ReadOnly Property SessionID As String
        ReadOnly Property State As ClientState
        ReadOnly Property IP As String
        ReadOnly Property IsConnected As Boolean

        Sub Send(packet() As Byte)
        Sub Close()
    End Interface
End Namespace