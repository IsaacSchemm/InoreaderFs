Imports System.IO
Imports System.Text.RegularExpressions
Imports InoreaderFs.Auth
Imports InoreaderFs.Auth.OAuth

Public Class Form1
    Implements IAutoRefreshToken

    Public ReadOnly Property App As App Implements IAutoRefreshToken.App
        Get
            Return New App(TextBox1.Text, TextBox2.Text)
        End Get
    End Property

    Public ReadOnly Property AccessToken As String Implements IAccessToken.AccessToken
        Get
            Return TextBox4.Text
        End Get
    End Property

    Public ReadOnly Property RefreshToken As String Implements IRefreshToken.RefreshToken
        Get
            Return TextBox5.Text
        End Get
    End Property

    Public Async Function UpdateTokenAsync(Param As IRefreshToken) As Task Implements IAutoRefreshToken.UpdateTokenAsync
        Dim asyncResult = BeginInvoke(
            Sub()
                TextBox4.Text = Param.AccessToken
                TextBox5.Text = Param.RefreshToken
            End Sub)
        Await Task.Factory.FromAsync(
            asyncResult,
            Sub(x)
                EndInvoke(x)
            End Sub)
    End Function

    Private Sub Button1_Click(sender As Object, e As EventArgs) Handles Button1.Click
        Dim Root As String = "HKEY_CURRENT_USER\"
        Dim Key As String = "Software\Microsoft\Internet Explorer\Main\FeatureControl\FEATURE_BROWSER_EMULATION"
        Dim ExeName = Path.GetFileName(Application.ExecutablePath)
        Dim CurrentSetting As String = CStr(Microsoft.Win32.Registry.CurrentUser.OpenSubKey(Key).GetValue(ExeName))
        If CurrentSetting Is Nothing OrElse CInt(CurrentSetting) <> 11001 Then
            Microsoft.Win32.Registry.SetValue(Root & Key, ExeName, 11001)
        End If

        Dim form As New Form With {
            .Width = 475,
            .Height = 575
        }

        Dim webBrowser As New WebBrowser With {
            .Dock = DockStyle.Fill
        }
        form.Controls.Add(webBrowser)

        AddHandler webBrowser.Navigated, Async Sub(o, x)
                                             If x.Url.GetLeftPart(UriPartial.Path) = TextBox3.Text Then
                                                 Dim match = Regex.Match(x.Url.Query, "code=([^&]+)")
                                                 Dim code = match.Groups(1).Value

                                                 Dim token = Await OAuthHandler.GetTokenAsync(App, code, New Uri(TextBox3.Text))
                                                 BeginInvoke(Sub()
                                                                 TextBox4.Text = token.access_token
                                                                 TextBox5.Text = token.refresh_token
                                                             End Sub)

                                                 form.DialogResult = DialogResult.OK
                                             End If
                                         End Sub

        AddHandler form.Shown, Sub()
                                   webBrowser.Navigate($"https://www.inoreader.com/oauth2/auth?client_id={Uri.EscapeDataString(TextBox1.Text)}&redirect_uri={Uri.EscapeDataString(TextBox3.Text)}&response_type=code&scope=read&state={Guid.NewGuid()}")
                               End Sub

        form.ShowDialog(Me)
    End Sub

    Private Async Sub Button2_Click(sender As Object, e As EventArgs) Handles Button2.Click
        Dim credentials = InoreaderFs.Auth.Credentials.NewOAuth(Me)
        Dim user = Await InoreaderFs.Endpoints.UserInfo.ExecuteAsync(credentials)
        MsgBox(user.ToString())
    End Sub
End Class
