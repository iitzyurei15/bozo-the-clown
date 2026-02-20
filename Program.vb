Imports System
Imports System.Collections.Generic
Imports System.Diagnostics
Imports System.Threading

Module Program
    Private Enum Turn
        Straight
        Left
        Right
    End Enum

    Private ReadOnly Rng As New Random()

    Sub Main()
        Console.CursorVisible = False
        Console.Title = "A Dance of Fire and Ice (Console VB.NET)"

        Dim beatIntervalMs As Integer = 600
        Dim timingWindowMs As Integer = 120
        Dim trail As List(Of Turn) = GenerateTrail(80)
        Dim score As Integer = 0
        Dim streak As Integer = 0
        Dim bestStreak As Integer = 0
        Dim activePlanetIsFire As Boolean = True

        DrawHelp()
        WaitForStart()

        Dim clock As Stopwatch = Stopwatch.StartNew()

        For beatIndex As Integer = 0 To trail.Count - 1
            Dim beatTime As Long = CLng(beatIndex) * beatIntervalMs
            DrawFrame(trail, beatIndex, activePlanetIsFire, score, streak, bestStreak, beatIntervalMs, timingWindowMs)

            Do While clock.ElapsedMilliseconds < beatTime - timingWindowMs
                Thread.Sleep(1)
            Loop

            Dim hit As Boolean = WaitForHit(clock, beatTime, timingWindowMs)

            If hit Then
                score += 100 + Math.Min(streak, 30) * 5
                streak += 1
                bestStreak = Math.Max(bestStreak, streak)
                activePlanetIsFire = Not activePlanetIsFire
            Else
                streak = 0
                score = Math.Max(0, score - 80)
            End If
        Next

        DrawGameOver(score, bestStreak)
    End Sub

    Private Sub DrawHelp()
        Console.Clear()
        Console.WriteLine("A DANCE OF FIRE AND ICE (VB.NET Console Edition)")
        Console.WriteLine(New String("="c, 48))
        Console.WriteLine("Follow the trail rhythm. Press [SPACE] on every beat.")
        Console.WriteLine("Two planets orbit: Fire (F) and Ice (I). They alternate each hit.")
        Console.WriteLine("If you miss the beat window, your streak resets.")
        Console.WriteLine("Press Q during gameplay to quit.")
        Console.WriteLine()
        Console.WriteLine("Trail legend: ─ straight, ╱ left, ╲ right")
        Console.WriteLine()
        Console.WriteLine("Press ENTER to start...")
    End Sub

    Private Sub WaitForStart()
        Do
            Dim key As ConsoleKeyInfo = Console.ReadKey(True)
            If key.Key = ConsoleKey.Enter Then
                Exit Do
            End If
        Loop
    End Sub

    Private Function GenerateTrail(length As Integer) As List(Of Turn)
        Dim turns As New List(Of Turn)(length)
        Dim last As Turn = Turn.Straight

        For i As Integer = 0 To length - 1
            Dim roll As Integer = Rng.Next(100)
            Dim candidate As Turn

            If roll < 55 Then
                candidate = Turn.Straight
            ElseIf roll < 78 Then
                candidate = Turn.Left
            Else
                candidate = Turn.Right
            End If

            If i > 1 AndAlso candidate <> Turn.Straight AndAlso candidate = last Then
                candidate = Turn.Straight
            End If

            turns.Add(candidate)
            last = candidate
        Next

        Return turns
    End Function

    Private Function WaitForHit(clock As Stopwatch, beatTime As Long, windowMs As Integer) As Boolean
        Dim startWindow As Long = beatTime - windowMs
        Dim endWindow As Long = beatTime + windowMs

        Do
            Dim now As Long = clock.ElapsedMilliseconds

            If Console.KeyAvailable Then
                Dim key As ConsoleKeyInfo = Console.ReadKey(True)
                If key.Key = ConsoleKey.Q Then
                    Environment.[Exit](0)
                End If

                If key.Key = ConsoleKey.Spacebar Then
                    Return now >= startWindow AndAlso now <= endWindow
                End If
            End If

            If now > endWindow Then
                Return False
            End If

            Thread.Sleep(1)
        Loop
    End Function

    Private Sub DrawFrame(trail As List(Of Turn), beatIndex As Integer, fireTurn As Boolean, score As Integer, streak As Integer, bestStreak As Integer, bpmMs As Integer, windowMs As Integer)
        Console.SetCursorPosition(0, 0)
        Console.WriteLine("A DANCE OF FIRE AND ICE (VB.NET Console Edition)      ")
        Console.WriteLine($"Score: {score,-6}  Streak: {streak,-3}  Best: {bestStreak,-3}           ")
        Console.WriteLine($"Beat Interval: {bpmMs}ms  Timing Window: ±{windowMs}ms              ")
        Console.WriteLine($"Active Planet: {(If(fireTurn, "FIRE (F)", "ICE (I)"))}                          ")
        Console.WriteLine("Press SPACE on beat. Press Q to quit.                    ")
        Console.WriteLine(New String("-"c, 58))

        Dim startIdx As Integer = Math.Max(0, beatIndex - 20)
        Dim endIdx As Integer = Math.Min(trail.Count - 1, beatIndex + 20)

        Dim line As String = ""
        For i As Integer = startIdx To endIdx
            Dim token As String = TurnSymbol(trail(i))
            If i = beatIndex Then
                line &= "[" & token & "]"
            Else
                line &= " " & token & " "
            End If
        Next

        Console.WriteLine(line.PadRight(58))
        Console.WriteLine(New String("-"c, 58))
        Console.WriteLine("Keep the orbit stable. Every beat alternates planets.")
    End Sub

    Private Function TurnSymbol(t As Turn) As String
        Select Case t
            Case Turn.Left
                Return "╱"
            Case Turn.Right
                Return "╲"
            Case Else
                Return "─"
        End Select
    End Function

    Private Sub DrawGameOver(score As Integer, bestStreak As Integer)
        Console.Clear()
        Console.WriteLine("Run complete!")
        Console.WriteLine($"Final Score: {score}")
        Console.WriteLine($"Best Streak: {bestStreak}")
        Console.WriteLine()
        Console.WriteLine("Thanks for playing A Dance of Fire and Ice (VB.NET).")
    End Sub
End Module
