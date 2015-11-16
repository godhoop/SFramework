Public Class SText
    Public Shared Iterator Function SplitW(data As String, length As Integer) As IEnumerable(Of String)
        For i As Integer = 0 To data.Length Step length
            If data.Length - i >= length Then
                Yield data.Substring(i, length)
            Else
                Yield data.Substring(i)
            End If
        Next
    End Function
End Class