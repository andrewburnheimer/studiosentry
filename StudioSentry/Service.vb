Public Class Service
    Public Sub New()
        MyBase.New()
        ' This call is required by the designer.
        InitializeComponent()

        ' Add any initialization after the InitializeComponent() call.
        Me.EventLog = New System.Diagnostics.EventLog
        If Not System.Diagnostics.EventLog.SourceExists("Studio Sentry") Then
            System.Diagnostics.EventLog.CreateEventSource("Studio Sentry", "Application")
        End If
        EventLog.Source = "Studio Sentry"
        EventLog.Log = "Application"
    End Sub

    Protected Overrides Sub OnStart(ByVal args() As String)
        ' Add code here to start your service. This method should set things
        ' in motion so your service can do its work.
        EventLog.WriteEntry("In OnStart.")

    End Sub

    Protected Overrides Sub OnStop()
        ' Add code here to perform any tear-down necessary to stop your service.
        EventLog.WriteEntry("In OnStop.")
    End Sub

End Class
