Public Class SimplifiedHttpRequest
    Public method As String
    Public resource As String
    Public protocol As String
    Public version As String
    Public headers As New List(Of String)

    Public Sub New(requestDataFromClient As String)
        parseReqFromClient(requestDataFromClient)
    End Sub

    Overrides Function toString() As String
        Return "<SimplifiedHttpRequest " & method & " " & resource & " " & protocol & "/" & version & ", " & headers.Count & " hdrs>"
    End Function

    Private Sub parseReqFromClient(requestDataFromClient As String)
        Dim stringArray As String() = requestDataFromClient.Split(New String() {Environment.NewLine},
                                       StringSplitOptions.None)
        Dim allRequestLines As New List(Of String)
        allRequestLines = stringArray.ToList()

        Dim firstLine = allRequestLines.First
        allRequestLines.RemoveAt(0)
        headers = allRequestLines

        parseFirstReqLine(firstLine)
    End Sub

    Private Sub parseFirstReqLine(firstLine As String)
        Dim stringArray As String() = firstLine.Split(" "c)
        method = stringArray(0)
        resource = stringArray(1)

        Dim protocolArray As String() = stringArray(2).Split("/"c)
        protocol = protocolArray(0)
        version = protocolArray(1)
    End Sub
End Class
