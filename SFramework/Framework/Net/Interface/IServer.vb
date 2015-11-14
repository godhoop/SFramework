﻿Namespace Net
    Public Interface IServer
        Event Connected(sender As Object, session As IClient)
        Event Disconnected(sender As Object, session As IClient)
        Event OnPacketReceived(sender As Object, session As IClient, packet() As Byte)

        Event PreviewAccept(sender As Object, e As Sockets.AcceptEventArgs)

        ReadOnly Property Port As Integer
        ReadOnly Property Sessions As IList(Of IClient)
        ReadOnly Property IsWorking As Boolean
        ReadOnly Property Socket As System.Net.Sockets.Socket
        ReadOnly Property BufferSize As Integer

        Sub Start(port As Integer)
        Sub Send(session As IClient, packet() As Byte)
        Sub BroadCast(packet() As Byte)
    End Interface
End Namespace