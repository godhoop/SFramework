Imports System.Drawing
Imports System.IO

Namespace Assembly
    Public MustInherit Class AssemblyResource
        Private Shared CurrentAssembly As Reflection.Assembly
        Private Shared ApplicationName As String = ""

        Shared Sub New()
            CurrentAssembly = Reflection.Assembly.GetExecutingAssembly
            ApplicationName = CurrentAssembly.GetName.Name
        End Sub

        Public Shared Function GetData(Of T)(resourceName As String) As T
            Using s As Stream = CurrentAssembly.GetManifestResourceStream(String.Format("{0}.{1}", ApplicationName, resourceName))
                If s Is Nothing Then
                    Return Nothing
                End If

                Return Convert(Of T)(s)
            End Using
        End Function

        Private Shared Function Convert(Of T)(s As Stream) As T
            Dim r As T

            Select Case GetType(T)
                Case GetType(Bitmap)
                    r = CObj(New Bitmap(s))

                Case GetType(String)
                    Using sr As New StreamReader(s)
                        r = CObj(sr.ReadToEnd)
                    End Using

                Case GetType(Integer)
                    r = CObj(Integer.Parse(Convert(Of String)(s)))

                Case GetType(Byte())
                    Dim data(s.Length - 1) As Byte
                    s.Read(data, 0, s.Length)

                    r = CObj(data)
            End Select

            Return r
        End Function
    End Class
End Namespace
