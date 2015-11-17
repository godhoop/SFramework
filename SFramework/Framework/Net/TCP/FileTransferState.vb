
Namespace Net.Sockets.TCP
    Friend Enum FileTransferState
        [None] = 0
        [Wait] = 4
        [Sending] = 6
        [Complete] = 7

        [TimeOut] = -1
    End Enum
End Namespace
