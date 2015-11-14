Imports System.Net.Sockets

Namespace Net.Sockets
    Public Class AcceptEventArgs
        Inherits EventArgs

        Private _Socket As Socket
        Private _Accept As Boolean = False

        Sub New(socket As Socket)
            _Socket = socket
        End Sub

        Public ReadOnly Property Socket As Socket
            Get
                Return _Socket
            End Get
        End Property

        Public Property Accept As Boolean
            Get
                Return _Accept
            End Get
            Set(value As Boolean)
                _Accept = value
            End Set
        End Property
    End Class

End Namespace