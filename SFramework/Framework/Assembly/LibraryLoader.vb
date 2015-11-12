Imports System.Reflection

Namespace SAssembly
    Public Class LibraryLoader
        Public Event Loaded(sender As Object, dllAssm As Assembly, result As LoadResult)

        Private WithEvents _domain As AppDomain
        Private _targetAssem As Assembly
        Private _dict As Dictionary(Of String, Assembly)

        Sub New(assm As Assembly)
            _domain = AppDomain.CurrentDomain
            _targetAssem = assm
            _dict = New Dictionary(Of String, Assembly)
        End Sub

        Public ReadOnly Property Domain As AppDomain
            Get
                Return _domain
            End Get
        End Property

        Public Property Assembly As Assembly
            Get
                Return _targetAssem
            End Get
            Set(value As Assembly)
                _targetAssem = value
            End Set
        End Property

        Public Function Load(libraryName As String) As Boolean
            Dim assm As Assembly = AssemblyResource.GetData(Of Assembly)(Me.Assembly, libraryName)

            If assm IsNot Nothing Then
                _dict.Add(assm.FullName, assm)
                Return True
            End If

            Return False
        End Function

        Public Function Load(libraryNames() As String) As Boolean
            For Each name As String In libraryNames
                If Not Load(name) Then
                    Return False
                End If
            Next

            Return True
        End Function

        Private Function _domain_AssemblyResolve(sender As Object, args As ResolveEventArgs) As Assembly Handles _domain.AssemblyResolve
            If _dict.ContainsKey(args.Name) Then
                Dim assm As Assembly = _dict(args.Name)
                _dict.Remove(args.Name)

                RaiseEvent Loaded(Me, assm, LoadResult.Success)

                Return assm
            End If

            RaiseEvent Loaded(Me, Nothing, LoadResult.Failed)

            Return Nothing
        End Function
    End Class
End Namespace
