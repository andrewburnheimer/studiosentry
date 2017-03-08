Public Class SimplifiedHttpResponse
    Private protocol As String = "HTTP"
    Private version As String = "1.1"
    Private code As Integer
    Private reasonPhrase As String
    Private headers As New Dictionary(Of String, String) From {{"Connection", "close"}}

    Private body As String

    Public Sub New(status As Integer, Optional responseBody As String = "", Optional contentType As String = "application/json")
        body = responseBody
        code = status
        reasonPhrase = setReasonPhrase(status)
        setHeader("Content-Type", contentType)
        setHeader("Server", "StudioSentry/" & ServiceMetadata.version_num & " (.NET 4)")
        setContentLengthHeader()
        setDateHeader()
    End Sub

    Overrides Function toString() As String
        Return "<SimplifiedHttpResponse " & protocol & "/" & version & " " & code & " " & reasonPhrase & ", " & body.Length & " bytes, " & headers.Count & " hdrs>"
    End Function

    Function payload() As String
        Dim data As String = ""
        data += protocol & "/" & version & " " & code & " " & reasonPhrase & vbCrLf
        For Each kvp As KeyValuePair(Of String, String) In headers
            Dim k As String = kvp.Key
            Dim v As String = kvp.Value
            data += k & ": " & v & vbCrLf
        Next
        data += vbCrLf ' empty line feed separating headers from body
        data += body
        Return data
    End Function

    Public Sub setHeader(title As String, value As String)
        deleteHeader(title)
        addHeader(title, value)
    End Sub

    Private Sub setDateHeader()
        setHeader("Date", DateTime.UtcNow.ToString("ddd, dd MMM yyyy HH:mm:ss") & " GMT")
    End Sub

    Private Function setReasonPhrase(status As Integer) As String
        Dim message As String = ""
        If status = 200 Then
            message = "OK"
        ElseIf status = 202 Then
            message = "ACCEPTED"
        ElseIf status = 404 Then
            message = "NOT FOUND"
        ElseIf status = 501 Then
            message = "NOT IMPLEMENTED"
        ElseIf status = 500 Then
            message = "INTERNAL SERVER ERROR"
        Else
            message = "UNKNOWN STATUS"
        End If
        Return message
    End Function

    Private Sub addHeader(title As String, value As String)
        headers.Add(title, value)
    End Sub

    Private Sub deleteHeader(title As String)
        headers.Remove(title)
    End Sub

    Private Function setContentLengthHeader() As Integer
        setHeader("Content-Length", body.Length)
        Return body.Length
    End Function
End Class
