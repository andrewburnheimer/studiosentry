Imports System.Net
Imports System.Net.Sockets
Imports System.Text
Imports System.Threading

' Threading example found at http://stackoverflow.com/questions/27926103/vb-net-tcplistner-windows-service#question
Public Class Service
    Private serverSocket As TcpListener
    Dim listenThread As New Thread(New ThreadStart(AddressOf ListenForClients))
    Private toKeepRunning As Boolean = True

    Public Sub New()
        MyBase.New()
        InitializeComponent()

        ev = New EventLog
        If Not EventLog.SourceExists("Studio Sentry") Then
            EventLog.CreateEventSource("Studio Sentry", "Application")
        End If
        ev.Source = "Studio Sentry"
        ev.Log = "Application"
    End Sub


    Private Sub ListenForClients()
        serverSocket = New TcpListener(IPAddress.Any, 8888)
        serverSocket.Start()

        ev.WriteEntry("Listen for clients...")
        Try
            While Me.toKeepRunning 'blocks until a client has connected to the server
                Dim client As TcpClient = Me.serverSocket.AcceptTcpClient()
                Dim clientThread As New Thread(New ParameterizedThreadStart(AddressOf HandleClientComm))
                clientThread.Start(client)
            End While
        Catch ex As Exception
            ev.WriteEntry("Listener socket thread exception: " & ex.ToString, EventLogEntryType.Warning)
            Throw
        End Try
    End Sub

    Private Sub HandleClientComm(ByVal client As Object)
        Dim tcpClient As TcpClient = DirectCast(client, TcpClient)
        Dim clientStream As NetworkStream = tcpClient.GetStream()

        Dim message As Byte() = New Byte(4095) {}
        Dim bytesRead As Integer

        ev.WriteEntry("Handle client comm...")

        Try

            While Me.toKeepRunning
                bytesRead = 0
                bytesRead = clientStream.Read(message, 0, 4096) 'blocks until a client sends a message

                If bytesRead = 0 Then
                    Exit While 'the client has disconnected from the server
                End If

                'message has successfully been received

                'Dim encoder As New ASCIIEncoding()
                'Dim serverResponse As String = "Response to send"
                'Dim sendBytes As [Byte]() = Encoding.ASCII.GetBytes(serverResponse)
                'clientStream.Write(sendBytes, 0, sendBytes.Length)

                'message has successfully been received
                Dim encoder As New ASCIIEncoding()

                ' Convert the Bytes received to a string and display it on the Server Screen
                Dim msg As String = encoder.GetString(message, 0, bytesRead)
                ev.WriteEntry("Read from client: " & msg)

            End While
        Catch ex As Exception
            ev.WriteEntry("Client Comm. thread exception: " & ex.ToString, EventLogEntryType.Warning)
            Throw
        End Try
        tcpClient.Close()

    End Sub


    Private Function BytesToString(
    ByVal bytes() As Byte) As String
        Return Encoding.Default.GetString(bytes)
    End Function


    Protected Overrides Sub OnStart(ByVal args() As String)
        listenThread.Start()
        ev.WriteEntry("Starting...")
    End Sub

    Protected Overrides Sub OnStop()
        ev.WriteEntry("listenThread to stop running...")
        Me.toKeepRunning = False
    End Sub

End Class