Imports System.Drawing
Imports System.IO

Namespace SAssembly
    Public MustInherit Class AssemblyResource
        Private Shared CurrentAssembly As Reflection.Assembly

        Shared Sub New()
            CurrentAssembly = Reflection.Assembly.GetExecutingAssembly
        End Sub

        Public Shared Function GetData(Of T)(assembly As Reflection.Assembly, resourceName As String) As T
            Using s As Stream = assembly.GetManifestResourceStream(String.Format("{0}.{1}", assembly.GetName.Name.Replace(Space(1), "_"), resourceName))
                If s Is Nothing Then
                    Return Nothing
                End If

                Return Convert(Of T)(s)
            End Using
        End Function

        Public Shared Function GetData(Of T)(resourceName As String) As T
            Return GetData(Of T)(CurrentAssembly, resourceName)
        End Function

        Private Shared Function Convert(Of T)(s As Stream) As T
            Dim r As Object = Nothing

            Select Case GetType(T)
                Case GetType(Bitmap)
                    r = New Bitmap(s)

                Case GetType(String)
                    Using sr As New StreamReader(s)
                        r = sr.ReadToEnd
                    End Using

                Case GetType(Integer)
                    r = Integer.Parse(Convert(Of String)(s))

                Case GetType(Byte())
                    Dim data(s.Length - 1) As Byte
                    s.Read(data, 0, s.Length)

                    r = data

                Case GetType(Reflection.Assembly)
                    r = Reflection.Assembly.Load(Convert(Of Byte())(s))
            End Select

            Return r
        End Function
    End Class
End Namespace
