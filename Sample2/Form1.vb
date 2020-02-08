Imports InoreaderFs.Auth
Imports InoreaderFs.Auth.OAuth

Public Class Form1
    Implements IAutoRefreshToken

    Public ReadOnly Property App As App Implements IAutoRefreshToken.App
        Get
            Return New App(TextBox1.Text, TextBox2.Text)
        End Get
    End Property

    Public ReadOnly Property RefreshToken As String Implements IRefreshToken.RefreshToken
        Get
            Return TextBox4.Text
        End Get
    End Property

    Public ReadOnly Property AccessToken As String Implements IAccessToken.AccessToken
        Get
            Return TextBox3.Text
        End Get
    End Property

    Public Function UpdateTokenAsync(Param As IRefreshToken) As Task Implements IAutoRefreshToken.UpdateTokenAsync
        TextBox3.Text = Param.AccessToken
        TextBox4.Text = Param.RefreshToken
        Return Task.CompletedTask
    End Function

    Private Async Sub Button1_Click(sender As Object, e As EventArgs) Handles Button1.Click
        Dim credentials = InoreaderFs.Auth.Credentials.NewOAuth(Me)
        Dim user = Await InoreaderFs.Endpoints.UserInfo.ExecuteAsync(credentials)
        MsgBox(user.userName)
    End Sub
End Class
