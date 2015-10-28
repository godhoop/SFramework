Imports System.Windows.Forms

Namespace Windows.Winform.Animation
    Public Class DynamicAnimation
        Public Class Animation
            Public Property Target As Object = Nothing
            Public Property From As Double = 0.0F
            Public Property [To] As Double = 0.0F
            Public Property Duration As Integer = 0
            Public Property Ease As Ease = Nothing
            Public Property DependencyProperty As System.Reflection.PropertyInfo
            Public Property BeginTime As Integer = 0

            ''' <summary>
            ''' 새로운 동적 애니메이션 효과를 생성합니다.
            ''' </summary>
            ''' <remarks></remarks>
            Sub New()
            End Sub

            ''' <summary>
            ''' 새로운 동적 애니메이션 효과를 생성합니다.
            ''' </summary>
            ''' <param name="target">애니메이션을 적용할 객체입니다.</param>
            ''' <param name="dependencyProperty">애니메이션을 적용할 객체의 속성입니다.</param>
            ''' <param name="toValue">애니메이션의 종료 값입니다.</param>
            ''' <param name="duration">애니메이션 지속 시간입니다.</param>
            ''' <param name="ease">애니메이션의 가속/감속 함수를 설정합니다.</param>
            ''' <remarks></remarks>
            Sub New(ByRef target As Object, dependencyProperty As String, toValue As Double, duration As Integer, ease As Ease)
                Me.Target = target
                Me.DependencyProperty = target.GetType.GetProperty(dependencyProperty)
                Me.From = AnimationFactory.GetValue(Me)
                Me.To = toValue
                Me.Duration = duration
                Me.Ease = ease
            End Sub

            ''' <summary>
            ''' 새로운 동적 애니메이션 효과를 생성합니다.
            ''' </summary>
            ''' <param name="target">애니메이션을 적용할 객체입니다.</param>
            ''' <param name="dependencyProperty">애니메이션을 적용할 객체의 속성입니다.</param>
            ''' <param name="fromValue">애니메이션의 시작 값입니다.</param>
            ''' <param name="toValue">애니메이션의 종료 값입니다.</param>
            ''' <param name="duration">애니메이션 지속 시간입니다.</param>
            ''' <param name="ease">애니메이션의 가속/감속 함수를 설정합니다.</param>
            ''' <remarks></remarks>
            Sub New(target As Object, dependencyProperty As String, fromValue As Double, toValue As Double, duration As Integer, ease As Ease)
                Me.Target = target
                Me.DependencyProperty = target.GetType.GetProperty(dependencyProperty)
                Me.From = fromValue
                Me.To = toValue
                Me.Duration = duration
                Me.Ease = ease
            End Sub
        End Class

        Public Class AnimationCollection
            Inherits Collections.ArrayList

            Public Overloads Function Add(value As Animation) As Integer
                Return MyBase.Add(value)
            End Function

            Public Function Items() As Animation()
                Dim itms(Me.Count - 1) As Animation

                For i As Integer = 0 To Me.Count - 1
                    itms(i) = Me.Item(i)
                Next

                Return itms
            End Function
        End Class

        Public Class AnimationBoard
            Public Event Completed(sender As AnimationBoard)
            Public Event Updated(sender As AnimationBoard, value As Double)

            Public Property Items As New AnimationCollection
            Public Property BeginTime As Integer
            Public Property Tag As Object = Nothing

            ''' <summary>
            ''' DynamicAnimation.Animation과 연결된 애니메이션을 해당 대상에 적용하여 시작합니다.
            ''' </summary>
            ''' <remarks></remarks>
            Public Sub Begin()
                Dim cCnt As Integer = 0

                For Each anim As Animation In Items
                    Dim timer As New Timer With {.Interval = 1}
                    Dim ad As New AnimationFactory.AnimationData(timer, anim)

                    AddHandler timer.Tick,
                        Sub()
                            Dim blankTime As Integer = Me.BeginTime + anim.BeginTime

                            If blankTime <= ad.StopWath.ElapsedMilliseconds Then
                                Dim bt As Integer = ad.StopWath.ElapsedMilliseconds - blankTime
                                Dim t As Integer = Math.Min(bt, ad.Animation.Duration)

                                Dim value As Double = AnimationFactory.GetNextValue(anim, t)
                                AnimationFactory.SetValue(anim, value)

                                RaiseEvent Updated(Me, value)

                                If t = ad.Animation.Duration Then
                                    timer.Stop()

                                    AnimationFactory.RestoreAnimation({anim})

                                    cCnt += 1
                                    If cCnt = Items.Count Then
                                        RaiseEvent Completed(Me)
                                        cCnt = 0
                                    End If
                                End If
                            End If
                        End Sub
                    timer.Start()
                    ad.StopWath.Start()

                    AnimationFactory.RegisterAnimation(ad)
                Next
            End Sub

            ''' <summary>
            ''' DynaminAnimation.AnimationBoard에 대해 만들어진 애니메이션을 중지합니다.
            ''' </summary>
            ''' <remarks></remarks>
            Public Sub [Stop]()
                AnimationFactory.RestoreAnimation(Me.Items.Items)
                Me.Items.Clear()
            End Sub
        End Class

        Private Class AnimationFactory
            Inherits EasingFunctions

            Public Event Completed(sender As AnimationBoard)
            Public Event Updated(sender As AnimationBoard)

            Public Structure AnimationData
                Dim Timer As Timer
                Dim Animation As Animation
                Dim StopWath As Stopwatch

                Sub New(ByRef timer As Timer, anim As Animation)
                    Me.Timer = timer
                    Me.Animation = anim
                    Me.StopWath = New Stopwatch
                End Sub
            End Structure

            Private Shared animDatas As New List(Of AnimationData)

            Public Shared Sub RestoreAnimation(animations() As Animation)
                For Each ad As AnimationData In animDatas
                    For i As Integer = 0 To animations.Length - 1
                        If animations(i) Is ad.Animation Then
                            ad.Timer.Stop()
                            ad.Timer.Dispose()
                            ad.StopWath = Nothing

                            animDatas.Remove(ad)
                            Exit For
                        End If
                    Next
                Next
            End Sub

            Public Shared Sub RegisterAnimation(ad As AnimationData)
                animDatas.Add(ad)
            End Sub

            Public Shared Function GetNextValue(anim As Animation, t As Integer) As Double
                Dim Ease As Ease = anim.Ease
                Dim inc As Double = anim.To - anim.From

                Select Case Ease.Type
                    Case DynamicAnimation.Ease.EaseType.Back
                        If Ease.Mode = DynamicAnimation.Ease.EasingMode.In Then
                            Return easeInBack(t, anim.From, inc, anim.Duration)
                        ElseIf Ease.Mode = DynamicAnimation.Ease.EasingMode.Out Then
                            Return easeOutBack(t, anim.From, inc, anim.Duration)
                        Else
                            Return easeInOutBack(t, anim.From, inc, anim.Duration)
                        End If

                    Case DynamicAnimation.Ease.EaseType.Bounce
                        If Ease.Mode = DynamicAnimation.Ease.EasingMode.In Then
                            Return easeInBounce(t, anim.From, inc, anim.Duration)
                        ElseIf Ease.Mode = DynamicAnimation.Ease.EasingMode.Out Then
                            Return easeOutBounce(t, anim.From, inc, anim.Duration)
                        Else
                            Return easeInOutBounce(t, anim.From, inc, anim.Duration)
                        End If

                    Case DynamicAnimation.Ease.EaseType.Circ
                        If Ease.Mode = DynamicAnimation.Ease.EasingMode.In Then
                            Return easeInCirc(t, anim.From, inc, anim.Duration)
                        ElseIf Ease.Mode = DynamicAnimation.Ease.EasingMode.Out Then
                            Return easeOutCirc(t, anim.From, inc, anim.Duration)
                        Else
                            Return easeInOutCirc(t, anim.From, inc, anim.Duration)
                        End If

                    Case DynamicAnimation.Ease.EaseType.Cubic
                        If Ease.Mode = DynamicAnimation.Ease.EasingMode.In Then
                            Return easeInCirc(t, anim.From, inc, anim.Duration)
                        ElseIf Ease.Mode = DynamicAnimation.Ease.EasingMode.Out Then
                            Return easeOutCirc(t, anim.From, inc, anim.Duration)
                        Else
                            Return easeInOutCirc(t, anim.From, inc, anim.Duration)
                        End If

                    Case DynamicAnimation.Ease.EaseType.Elastic
                        If Ease.Mode = DynamicAnimation.Ease.EasingMode.In Then
                            Return easeInElastic(t, anim.From, inc, anim.Duration)
                        ElseIf Ease.Mode = DynamicAnimation.Ease.EasingMode.Out Then
                            Return easeOutElastic(t, anim.From, inc, anim.Duration)
                        Else
                            Return easeInOutElastic(t, anim.From, inc, anim.Duration)
                        End If

                    Case DynamicAnimation.Ease.EaseType.Expo
                        If Ease.Mode = DynamicAnimation.Ease.EasingMode.In Then
                            Return easeInExpo(t, anim.From, inc, anim.Duration)
                        ElseIf Ease.Mode = DynamicAnimation.Ease.EasingMode.Out Then
                            Return easeOutExpo(t, anim.From, inc, anim.Duration)
                        Else
                            Return easeInOutExpo(t, anim.From, inc, anim.Duration)
                        End If

                    Case DynamicAnimation.Ease.EaseType.Quad
                        If Ease.Mode = DynamicAnimation.Ease.EasingMode.In Then
                            Return easeInQuad(t, anim.From, inc, anim.Duration)
                        ElseIf Ease.Mode = DynamicAnimation.Ease.EasingMode.Out Then
                            Return easeOutQuad(t, anim.From, inc, anim.Duration)
                        Else
                            Return easeInOutQuad(t, anim.From, inc, anim.Duration)
                        End If

                    Case DynamicAnimation.Ease.EaseType.Quart
                        If Ease.Mode = DynamicAnimation.Ease.EasingMode.In Then
                            Return easeInQuart(t, anim.From, inc, anim.Duration)
                        ElseIf Ease.Mode = DynamicAnimation.Ease.EasingMode.Out Then
                            Return easeOutQuart(t, anim.From, inc, anim.Duration)
                        Else
                            Return easeInOutQuart(t, anim.From, inc, anim.Duration)
                        End If

                    Case DynamicAnimation.Ease.EaseType.Quint
                        If Ease.Mode = DynamicAnimation.Ease.EasingMode.In Then
                            Return easeInQuint(t, anim.From, inc, anim.Duration)
                        ElseIf Ease.Mode = DynamicAnimation.Ease.EasingMode.Out Then
                            Return easeOutQuint(t, anim.From, inc, anim.Duration)
                        Else
                            Return easeInOutQuint(t, anim.From, inc, anim.Duration)
                        End If

                    Case DynamicAnimation.Ease.EaseType.Sine
                        If Ease.Mode = DynamicAnimation.Ease.EasingMode.In Then
                            Return easeInSine(t, anim.From, inc, anim.Duration)
                        ElseIf Ease.Mode = DynamicAnimation.Ease.EasingMode.Out Then
                            Return easeOutSine(t, anim.From, inc, anim.Duration)
                        Else
                            Return easeInOutSine(t, anim.From, inc, anim.Duration)
                        End If

                    Case Else
                        Return anim.From
                End Select
            End Function

            Public Shared Function GetValue(anim As Animation) As Single
                Return anim.DependencyProperty.GetValue(anim.Target, Nothing)
            End Function

            Public Shared Sub SetValue(anim As Animation, value As Single)
                anim.DependencyProperty.SetValue(anim.Target, value, Nothing)
            End Sub
        End Class
    End Class

    Partial Public Class DynamicAnimation
        Public Class Ease
            Public Enum EasingMode As Byte
                [In]
                [Out]
                [InOut]
            End Enum

            Public Enum EaseType As Byte
                [Quad]
                [Cubic]
                [Quart]
                [Quint]
                [Sine]
                [Expo]
                [Circ]
                [Elastic]
                [Back]
                [Bounce]
            End Enum

            Public ReadOnly Type As EaseType
            Public ReadOnly Mode As EasingMode

            Sub New(easeType As EaseType, easingMode As EasingMode)
                Me.Type = easeType
                Me.Mode = easingMode
            End Sub
        End Class
    End Class

    Public Class EasingFunctions
        Public Shared Function easeInQuad(t As Integer, b As Double, c As Double, d As Integer) As Double
            Return c * (t / d) ^ 2 + b
        End Function

        Public Shared Function easeOutQuad(t As Integer, b As Double, c As Double, d As Integer) As Double
            Return -c * (t / d) * (t / d - 2) + b
        End Function

        Public Shared Function easeInOutQuad(t As Integer, b As Double, c As Double, d As Integer) As Double
            Dim tempT As Double = t / (d / 2) : If tempT < 1 Then Return c / 2 * tempT * tempT + b
            tempT -= 1 : Return -c / 2 * (tempT * (tempT - 2) - 1) + b
        End Function

        Public Shared Function easeInCubic(t As Integer, b As Double, c As Double, d As Integer) As Double
            Return c * (t / d) ^ 3 + b
        End Function

        Public Shared Function easeOutCubic(t As Integer, b As Double, c As Double, d As Integer) As Double
            Return c * ((t / d - 1) ^ 3 + 1) + b
        End Function

        Public Shared Function easeInOutCubic(t As Integer, b As Double, c As Double, d As Integer) As Double
            Dim tempT As Double = t / (d / 2) : If tempT < 1 Then Return c / 2 * tempT ^ 3 + b
            tempT -= 2 : Return c / 2 * (tempT ^ 3 + 2) + b
        End Function

        Public Shared Function easeInQuart(t As Integer, b As Double, c As Double, d As Integer) As Double
            Return c * (t / d) ^ 4 + b
        End Function

        Public Shared Function easeOutQuart(t As Integer, b As Double, c As Double, d As Integer) As Double
            Return -c * ((t / d - 1) ^ 4 - 1) + b
        End Function

        Public Shared Function easeInOutQuart(t As Integer, b As Double, c As Double, d As Integer) As Double
            Dim tempT As Double = t / (d / 2) : If tempT < 1 Then Return c / 2 * tempT ^ 4 + b
            tempT -= 2 : Return -c / 2 * (tempT ^ 4 - 2) + b
        End Function

        Public Shared Function easeInQuint(t As Integer, b As Double, c As Double, d As Integer) As Double
            Return c * (t / d) ^ 5 + b
        End Function

        Public Shared Function easeOutQuint(t As Integer, b As Double, c As Double, d As Integer) As Double
            Return c * ((t / d - 1) ^ 5 + 1) + b
        End Function

        Public Shared Function easeInOutQuint(t As Integer, b As Double, c As Double, d As Integer) As Double
            Dim tempT As Double = t / (d / 2) : If tempT < 1 Then Return c / 2 * tempT ^ 5 + b
            tempT -= 2 : Return c / 2 * (tempT ^ 5 + 2) + b
        End Function

        Public Shared Function easeInSine(t As Integer, b As Double, c As Double, d As Integer) As Double
            Return -c * Math.Cos(t / d * (Math.PI / 2)) + c + b
        End Function

        Public Shared Function easeOutSine(t As Integer, b As Double, c As Double, d As Integer) As Double
            Return c * Math.Sin(t / d * (Math.PI / 2)) + b
        End Function

        Public Shared Function easeInOutSine(t As Integer, b As Double, c As Double, d As Integer) As Double
            Return -c / 2 * (Math.Cos(Math.PI * t / d) - 1) + b
        End Function

        Public Shared Function easeInExpo(t As Integer, b As Double, c As Double, d As Integer) As Double
            Return IIf(t = 0, b, c * Math.Pow(2, 10 * (t / d - 1)) + b)
        End Function

        Public Shared Function easeOutExpo(t As Integer, b As Double, c As Double, d As Integer) As Double
            Return IIf(t = d, b + c, c * (-Math.Pow(2, -10 * t / d) + 1) + b)
        End Function

        Public Shared Function easeInOutExpo(t As Integer, b As Double, c As Double, d As Integer) As Double
            If t = 0 Then Return b
            If t = d Then Return b + c
            Dim tempT As Double = t / (d / 2) : If tempT < 1 Then Return c / 2 * Math.Pow(2, 10 * (tempT - 1)) + b
            tempT -= 1 : Return c / 2 * (-Math.Pow(2, -10 * tempT) + 2) + b
        End Function

        Public Shared Function easeInCirc(t As Integer, b As Double, c As Double, d As Integer) As Double
            Dim tempT As Double = t / d
            Return -c * (Math.Sqrt(1 - tempT * tempT) - 1) + b
        End Function

        Public Shared Function easeOutCirc(t As Integer, b As Double, c As Double, d As Integer) As Double
            Dim tempT As Double = t
            Return c * Math.Sqrt(1 - (InlineAssignHelper(Of Double)(tempT, tempT / d - 1)) * tempT) + b
        End Function

        Public Shared Function easeInOutCirc(t As Integer, b As Double, c As Double, d As Integer) As Double
            Dim tempT As Double = t / (d / 2) : If tempT < 1 Then Return -c / 2 * (Math.Sqrt(1 - tempT ^ 2) - 1) + b
            tempT -= 2 : Return c / 2 * (Math.Sqrt(1 - tempT ^ 2) + 1) + b
        End Function

        Public Shared Function easeInElastic(t As Integer, b As Double, c As Double, d As Integer) As Double
            Dim s As Double = 1.70158
            Dim p As Double = 0
            Dim a As Double = c
            Dim tempT As Double = t

            If tempT = 0 Then Return b
            tempT /= d : If tempT = 1 Then Return b + c
            If Not p Then p = d * 0.3

            If a < Math.Abs(c) Then
                a = c : s = p / 4
            Else
                s = p / (2 * Math.PI) * Math.Asin(c / a)
            End If

            tempT -= 1 : Return -(a * Math.Pow(2, 10 * tempT) * Math.Sin((tempT * d - s) * (2 * Math.PI) / p)) + b
        End Function

        Public Shared Function easeOutElastic(t As Integer, b As Double, c As Double, d As Integer) As Double
            Dim s As Double = 1.70158
            Dim p As Double = 0
            Dim a As Double = c
            Dim tempT As Double = t

            If tempT = 0 Then Return b
            tempT /= d : If tempT = 1 Then Return b + c
            If Not p Then p = d * 0.3

            If a < Math.Abs(c) Then
                a = c : s = p / 4
            Else
                s = p / (2 * Math.PI) * Math.Asin(c / a)
            End If

            Return a * Math.Pow(2, -10 * tempT) * Math.Sin((tempT * d - s) * (2 * Math.PI) / p) + c + b
        End Function

        Public Shared Function easeInOutElastic(t As Integer, b As Double, c As Double, d As Integer) As Double
            Dim s As Double = 1.70158
            Dim p As Double = 0
            Dim a As Integer = c
            Dim tempT As Double = t

            If tempT = 0 Then Return b
            tempT /= d / 2
            If tempT = 2 Then Return b + c
            If Not p Then p = d * (0.3 * 1.5)

            If a < Math.Abs(c) Then
                a = c : s = p / 4
            Else
                s = p / (2 * Math.PI) * Math.Asin(c / a)
            End If

            tempT -= 1
            If tempT < 0 Then Return -0.5 * (a * Math.Pow(2, 10 * tempT) * Math.Sin((tempT * d - s) * (2 * Math.PI) / p)) + b
            Return a * Math.Pow(2, -10 * tempT) * Math.Sin((tempT * d - s) * (2 * Math.PI) / p) * 0.5 + c + b
        End Function

        Public Shared Function easeInBack(t As Integer, b As Double, c As Double, d As Integer, Optional s As Double = 1.70158) As Double
            Dim tempT As Double = t / d : Return c * tempT * tempT * ((s + 1) * tempT - s) + b
        End Function

        Public Shared Function easeOutBack(t As Integer, b As Double, c As Double, d As Integer, Optional s As Double = 1.70158) As Double
            Dim tempT As Double = t
            Return c * ((InlineAssignHelper(Of Double)(tempT, tempT / d - 1)) * tempT * ((s + 1) * tempT + s) + 1) + b
        End Function

        Public Shared Function easeInOutBack(t As Integer, b As Double, c As Double, d As Integer, Optional s As Double = 1.70158) As Double
            Dim tempT As Double = t / (d / 2)
            s *= 1.525
            If tempT < 1 Then Return c / 2 * (tempT ^ 2 * ((s + 1) * tempT - s)) + b
            tempT -= 2
            Return c / 2 * (tempT ^ 2 * ((s + 1) * tempT + s) + 2) + b
        End Function

        Public Shared Function easeInBounce(t As Integer, b As Double, c As Double, d As Integer) As Double
            Return c - easeOutBounce(d - t, 0, c, d) + b
        End Function

        Public Shared Function easeOutBounce(t As Integer, b As Double, c As Double, d As Integer) As Double
            Dim tempT As Double = t / d

            If tempT < 1 / 2.75 Then
                Return c * 7.5625 * tempT * tempT + b
            ElseIf tempT < 2 / 2.75 Then
                tempT -= 1.5 / 2.75
                Return c * (7.5625 * tempT * tempT + 0.75) + b
            ElseIf tempT < (2.5 / 2.75) Then
                tempT -= 2.25 / 2.75
                Return c * (7.5625 * tempT * tempT + 0.9375) + b
            Else
                tempT -= 2.625 / 2.75
                Return c * (7.5625 * tempT * tempT + 0.984375) + b
            End If
        End Function

        Public Shared Function easeInOutBounce(t As Integer, b As Double, c As Double, d As Integer) As Double
            If t < d / 2 Then Return easeInBounce(t * 2, 0, c, d) * 0.5 + b
            Return easeOutBounce(t * 2 - d, 0, c, d) * 0.5 + c * 0.5 + b
        End Function

        Private Shared Function InlineAssignHelper(Of T)(ByRef target As T, ByVal value As T) As T
            target = value
            Return value
        End Function
    End Class
End Namespace

