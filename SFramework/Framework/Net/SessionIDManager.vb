Imports System.Text

Namespace Net
    Public MustInherit Class SessionIDManager
        Private Shared Sessions As New List(Of String)
        Private Shared Random As New Random
        Private Shared CharSet As New List(Of Char)

        Shared Sub New()
            CharSet.AddRange(Enumerable.Range(48, 10).Select(Function(n As Integer) Chr(n)))
            CharSet.AddRange(Enumerable.Range(65, 25).Select(Function(n As Integer) Chr(n)))
            CharSet.AddRange(Enumerable.Range(97, 25).Select(Function(n As Integer) Chr(n)))
        End Sub

        Public Shared Function CreateSessionID(length As Integer) As String
            Dim sb As New StringBuilder

            While sb.Length < length
                Dim idx As Integer = Random.Next(0, CharSet.Count - 1)
                Dim c As Char = CharSet(idx)

                sb.Append(c)
            End While

            Dim id As String = Convert.ToBase64String(Encoding.ASCII.GetBytes(sb.ToString))

            If Not Sessions.Contains(id) Then
                Sessions.Add(id)

                Return id
            Else
                Return CreateSessionID(length)
            End If
        End Function
    End Class
End Namespace