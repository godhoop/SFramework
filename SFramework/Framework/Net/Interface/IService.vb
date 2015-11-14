Imports SFramework.Net.Sockets

Namespace Net
    Public Interface IService
        ReadOnly Property ServiceType As TCPServiceType
        ReadOnly Property Server As IServer
    End Interface
End Namespace