Public NotInheritable Class SMath
    Public Structure Fraction
        Dim Numerator As UInteger
        Dim Denominator As UInteger

        Sub New(n As UInteger, d As UInteger)
            Me.Numerator = n
            Me.Denominator = d
        End Sub
    End Structure

    ''' <summary>
    ''' 약수를 구합니다.
    ''' </summary>
    ''' <param name="n"></param>
    ''' <returns></returns>
    Public Shared Iterator Function GetDivisors(n As Integer) As IEnumerable(Of Integer)
        For i As Integer = 1 To n
            If n Mod i = 0 Then
                Yield i
            End If
        Next
    End Function

    ''' <summary>
    ''' 소수를 판별합니다.
    ''' </summary>
    ''' <param name="n"></param>
    ''' <returns></returns>
    Public Shared Function IsPrime(n As Integer) As Boolean
        Return GetDivisors(n).Count = 2
    End Function

    ''' <summary>
    ''' 소수를 구합니다.
    ''' </summary>
    ''' <param name="n"></param>
    ''' <returns></returns>
    Public Shared Function GetPrimes(n As Integer) As IEnumerable(Of Integer)
        Dim primes As New List(Of Integer)

        For i As Integer = 2 To n
            Dim f As Boolean = True

            For Each prime As Integer In primes
                If prime * prime > i Then Exit For

                If i Mod prime = 0 Then
                    f = False
                    Exit For
                End If
            Next

            If f Then primes.Add(i)
        Next

        Return primes
    End Function

    ''' <summary>
    ''' 소수를 병렬처리로 구합니다.
    ''' </summary>
    ''' <param name="n"></param>
    ''' <returns></returns>
    Public Shared Function GetPrimesParallel(n As Integer) As IEnumerable(Of Integer)
        Dim s As Integer = Math.Sqrt(n)
        Dim low As IEnumerable(Of Integer) = GetPrimes(s)
        Dim high As ParallelQuery(Of Integer) = (Enumerable.Range(s + 1, n - s).AsParallel().Where(Function(i) low.All(Function(prime) i Mod prime <> 0)))

        Return low.Concat(high)
    End Function

    ''' <summary>
    ''' 나눗셈을 합니다
    ''' </summary>
    ''' <param name="denominator"></param>
    ''' <param name="numerator"></param>
    ''' <returns></returns>
    Public Shared Function Division(numerator As Integer, denominator As Integer, Optional useReduction As Boolean = False) As Double
        If useReduction Then
            Dim d As IEnumerable(Of Integer) = GetFactorization(denominator)
            Dim n As IEnumerable(Of Integer) = GetFactorization(numerator)

            Return calc_ReductionDivision(n, d)
        Else
            Return numerator / denominator
        End If
    End Function

    Private Shared Function calc_ReductionDivision(n As IEnumerable(Of Integer), d As IEnumerable(Of Integer)) As Double
        Dim dex As IEnumerable(Of Integer) = d.Except(n)
        Dim nex As IEnumerable(Of Integer) = n.Except(d)

        Dim dx As Integer = 1
        Dim nx As Integer = 1

        If dex.Count > 0 Then dx = Helper.AllMultiply(dex)
        If nex.Count > 0 Then nx = Helper.AllMultiply(nex)

        Return nx / dx
    End Function

    ''' <summary>
    ''' 인수를 분해합니다.
    ''' </summary>
    ''' <param name="num"></param>
    ''' <returns></returns>
    Public Shared Iterator Function GetFactorization(num As Integer) As IEnumerable(Of Integer)
        While num Mod 2 = 0
            Yield 2
            num /= 2
        End While

        Dim factor As Integer = 3
        While factor * factor <= num
            If num Mod factor = 0 Then
                Yield factor
                num /= factor
            Else
                factor += 2
            End If
        End While

        If num > 1 Then
            Yield num
        End If
    End Function

    ''' <summary>
    ''' 순열
    ''' </summary>
    ''' <param name="n"></param>
    ''' <param name="r"></param>
    ''' <returns></returns>
    Public Shared Function nPr(n As Integer, r As Integer) As Integer
        Return Helper.AllMultiply(Helper.Range(n, -r))
    End Function

    ''' <summary>
    ''' 조합
    ''' </summary>
    ''' <param name="n"></param>
    ''' <param name="r"></param>
    ''' <returns></returns>
    Public Shared Function nCr(n As Integer, r As Integer) As Double
        If r > n / 2 Then r = n - r

        Dim ne As IEnumerable(Of Integer) = Helper.Range(n, -r)
        Dim de As IEnumerable(Of Integer) = Helper.Range(1, r)

        Return calc_ReductionDivision(ne, de)
    End Function

    ''' <summary>
    ''' 중복 조합
    ''' </summary>
    ''' <param name="n"></param>
    ''' <param name="r"></param>
    ''' <returns></returns>
    Public Shared Function nHr(n As Integer, r As Integer) As Double
        Return nCr(n + r - 1, r)
    End Function

    ''' <summary>
    ''' 중복 순열
    ''' </summary>
    ''' <param name="n"></param>
    ''' <param name="r"></param>
    ''' <returns></returns>
    Public Shared Function nPir(n As Integer, r As Integer) As ULong
        Return Math.Pow(n, r)
    End Function

    ''' <summary>
    ''' 두 2차원점 사이의 거리를 구합니다.
    ''' </summary>
    ''' <param name="x1"></param>
    ''' <param name="y1"></param>
    ''' <param name="x2"></param>
    ''' <param name="y2"></param>
    ''' <returns></returns>
    Public Shared Function Distance(x1 As Integer, y1 As Integer,
                                    x2 As Integer, y2 As Integer) As Double
        Return Math.Sqrt(Math.Pow(x2 - x1, 2) + Math.Pow(y2 - y1, 2))
    End Function

    ''' <summary>
    ''' 두 3차원점 사이의 거리를 구합니다.
    ''' </summary>
    ''' <param name="x1"></param>
    ''' <param name="y1"></param>
    ''' <param name="z1"></param>
    ''' <param name="x2"></param>
    ''' <param name="y2"></param>
    ''' <param name="z2"></param>
    ''' <returns></returns>
    Public Shared Function Distance(x1 As Integer, y1 As Integer, z1 As Integer,
                                    x2 As Integer, y2 As Integer, z2 As Integer) As Double
        Return Math.Sqrt(Math.Pow(x2 - x1, 2) + Math.Pow(y2 - y1, 2) + Math.Pow(z2 - z1, 2))
    End Function

    ''' <summary>
    ''' 소수를 판별합니다.
    ''' </summary>
    ''' <param name="n"></param>
    ''' <returns></returns>
    Public Shared Function IsDecimal(n As Object) As Boolean
        Dim v As Double
        Return Double.TryParse(n, v)
    End Function

    Private Class Helper
        Public Shared Iterator Function Range(start As Integer, count As Integer) As IEnumerable(Of Integer)
            For i As Integer = start To start + count + If(count < 0, 1, -1) Step If(count < 0, -1, 1)
                Yield i
            Next
        End Function

        Public Shared Function AllMultiply(arr As IEnumerable(Of Double)) As Double
            Return arr.Aggregate(Function(x As Double, y As Double) x * y)
        End Function

        Public Shared Function AllMultiply(arr As IEnumerable(Of Integer)) As Long
            Return arr.Aggregate(Function(x As Long, y As Long) x * y)
        End Function
    End Class
End Class