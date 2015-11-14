Imports System.Net.Sockets
Imports SFramework.Net.TCP

Namespace Net.Sockets
    Public Class TCPService
        Implements IService

        Private WithEvents _Server As IServer
        Private _ServiceType As TCPServiceType

        Private _SessionLimit As UShort = 0

#Region " [ 초기화 ] "
        Sub New(serviceType As TCPServiceType, Optional bufferSize As Integer = 1024)
            _ServiceType = serviceType

            Initialize(bufferSize)
        End Sub

        Sub New(serviceType As TCPServiceType, sessionLimit As Integer, Optional bufferSize As Integer = 1024)
            _ServiceType = serviceType
            _SessionLimit = sessionLimit

            Initialize(bufferSize)
        End Sub

        Protected Overridable Sub Initialize(buffersize As Integer)
            Select Case ServiceType
                Case TCPServiceType.Asynchronous
                    _Server = New AsyncServer(buffersize)

                Case TCPServiceType.MultiThread

            End Select
        End Sub
#End Region

#Region " [ 속성 ] "
        Public ReadOnly Property Server As IServer Implements IService.Server
            Get
                Return _Server
            End Get
        End Property

        Public ReadOnly Property ServiceType As TCPServiceType Implements IService.ServiceType
            Get
                Return _ServiceType
            End Get
        End Property

        Public ReadOnly Property SessionLimit As UShort
            Get
                Return _SessionLimit
            End Get
        End Property
#End Region

        Public Sub Start(port As Integer)
            Server.Start(port)
        End Sub

        Protected Overridable Sub OnAccept(sender As Object, e As AcceptEventArgs) Handles _Server.PreviewAccept
            e.Accept = (SessionLimit = 0 Or Server.Sessions.Count < SessionLimit)
        End Sub
    End Class
End Namespace