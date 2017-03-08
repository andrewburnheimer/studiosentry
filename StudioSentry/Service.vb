Imports System.Net
Imports System.Net.Sockets
Imports System.Text
Imports System.Threading
Imports Newtonsoft.Json

Public Class ServiceMetadata
    Public Shared version_num As String = "0.0.9"
End Class

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
        serverSocket = New TcpListener(IPAddress.Any, 8888) ' XXX Should be configurable
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
                Dim encoder As New ASCIIEncoding()

                ' Convert the Bytes received to a string and display it on the Server Screen
                Dim msg As String = encoder.GetString(message, 0, bytesRead)

                Dim clientRequest As New SimplifiedHttpRequest(msg)
                ev.WriteEntry("Req from client: " & clientRequest.toString())

                Dim serverResponse As SimplifiedHttpResponse
                Try
                    If clientRequest.resource = "/about" Then
                        Dim ar As New AboutResponse
                        serverResponse = New SimplifiedHttpResponse(200, JsonConvert.SerializeObject(ar))
                    ElseIf clientRequest.resource = "/usage" Then
                        Dim ur As New UsageResponse
                        serverResponse = New SimplifiedHttpResponse(200, JsonConvert.SerializeObject(ur))
                    ElseIf clientRequest.resource = "/NumOfRemoteAppUsers" Then
                        Dim rr As New RemoteAppUsersResponse
                        rr.NumOfRemoteAppUsers = NumOfRemoteAppUsers()
                        serverResponse = New SimplifiedHttpResponse(200, JsonConvert.SerializeObject(rr))
                    ElseIf clientRequest.resource = "/NumOfRemoteDesktopUsers" Then
                        Dim rr As New RemoteDesktopUsersResponse
                        rr.NumOfRemoteDesktopUsers = NumOfRemoteDesktopUsers()
                        serverResponse = New SimplifiedHttpResponse(200, JsonConvert.SerializeObject(rr))
                    ElseIf clientRequest.resource = "/KickRemoteAppUser" And clientRequest.method = "PUT" Then
                        KickRemoteAppUser()
                        serverResponse = New SimplifiedHttpResponse(202)
                    Else
                        Dim ur As New UsageResponse(False)
                        serverResponse = New SimplifiedHttpResponse(404, JsonConvert.SerializeObject(ur))
                    End If

                Catch ex As Exception
                    Dim code As Integer = 500
                    If TypeOf ex Is NotImplementedException Then
                        code = 501
                    End If
                    Dim er As New ErrorResponse(code, ex.Message, ex.StackTrace)
                    serverResponse = New SimplifiedHttpResponse(code, JsonConvert.SerializeObject(er))
                End Try


                ev.WriteEntry("Res to client: " & serverResponse.toString())

                Dim sendBytes As [Byte]() = Encoding.ASCII.GetBytes(serverResponse.payload)
                clientStream.Write(sendBytes, 0, sendBytes.Length)
            End While
        Catch ex As Exception
            ev.WriteEntry("Client Comm. thread exception: " & ex.ToString, EventLogEntryType.Warning)
            Throw
        End Try
        tcpClient.Close()

    End Sub


    Private Function BytesToString(ByVal bytes() As Byte) As String
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

    Private Function NumOfRemoteAppUsers() As Integer
        Return ListOfRemoteAppUsers().Count
    End Function

    Private Function ListOfRemoteAppUsers() As String()
        Throw New NotImplementedException()
        Return New String(0) {}
    End Function

    Private Function NumOfRemoteDesktopUsers() As Integer
        Return ListOfRemoteDesktopUsers().Count
    End Function

    Private Function ListOfRemoteDesktopUsers() As String()
        Throw New NotImplementedException()
        Return New String(0) {}
    End Function

    Private Sub KickRemoteAppUser()
        Throw New NotImplementedException()
    End Sub
End Class

Public Class UsageResponse
    Public status As Integer = 200
    Public message As String = "OK"

    Public validEndpoints(,) As String = {{"GET", "/about"}, {"GET", "/usage"}, {"GET", "/NumOfRemoteAppUsers"},
        {"GET", "/NumOfRemoteDesktopUsers"}, {"PUT", "/KickRemoteAppUser"}}
    Public Sub New(Optional intentionalCall As Boolean = True)
        If Not intentionalCall Then
            message = "Resource unknown, validEndpoints provided below"
            status = 404
        End If

    End Sub
End Class

Public Class ErrorResponse
    Public status As Integer
    Public message As String
    Public strace As String

    Public Sub New(s As Integer, m As String, Optional st As String = "")
        status = s
        message = m
        strace = st
    End Sub
End Class

Public Class AboutResponse
    Public name As String = "StudioSentry"
    Public version As String = ServiceMetadata.version_num
End Class

Public Class RemoteAppUsersResponse
    Public NumOfRemoteAppUsers As Integer
End Class

Public Class RemoteDesktopUsersResponse
    Public NumOfRemoteDesktopUsers As Integer
End Class
