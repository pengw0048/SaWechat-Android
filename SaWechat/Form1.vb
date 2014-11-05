Imports System.IO
Imports System.Text
Imports System.Security.Cryptography
Imports System.Data.SQLite

Public Class Form1

    Dim rightpath As String
    Dim passwords As New List(Of String), directories As New List(Of String)

    Private Sub RadioButton1_CheckedChanged(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles RadioButton1.CheckedChanged
        GroupBox2.Enabled = True
    End Sub

    Private Sub RadioButton2_CheckedChanged(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles RadioButton2.CheckedChanged
        GroupBox2.Enabled = True
    End Sub

    Private Sub RadioButton3_CheckedChanged(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles RadioButton3.CheckedChanged
        GroupBox2.Enabled = False
    End Sub

    Private Sub Button1_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles Button1.Click
        Button2.Enabled = False
        FolderBrowserDialog1.ShowDialog()
        TextBox1.Text = FolderBrowserDialog1.SelectedPath
        rightpath = TextBox1.Text
        If Not rightpath.EndsWith("\") Then
            rightpath = (rightpath & "\")
        End If
        ComboBox1.Items.Clear()
        ComboBox1.Items.Add("(未选择)")
        passwords.Clear()
        directories.Clear()
        passwords.Add("")
        directories.Add("")
        If Directory.Exists(rightpath + "WeiXin") AndAlso File.Exists(rightpath + "CompatibleInfo.cfg") AndAlso File.Exists(rightpath + "systemInfo.cfg") Then
            Label2.Text = "正确"
            Label2.ForeColor = Color.Green
            Try
                Dim DirInfo As New DirectoryInfo(rightpath)
                Dim FilInfo As DirectoryInfo
                For Each FilInfo In DirInfo.GetDirectories
                    Dim GotPass As Boolean = False
                    If FilInfo.Name.Length = 32 AndAlso FilInfo.Name.Replace("0", "") <> "" AndAlso File.Exists(FilInfo.FullName + "\EnMicroMsg.db") Then
                        directories.Add(FilInfo.Name)
                        Dim file1 As New FileStream(rightpath + "CompatibleInfo.cfg", FileMode.Open)
                        Dim file2 As New FileStream(rightpath + "systemInfo.cfg", FileMode.Open)
                        Dim bytes1(file1.Length) As Byte
                        Dim bytes2(file2.Length) As Byte
                        file1.Read(bytes1, 0, file1.Length)
                        file2.Read(bytes2, 0, file2.Length)
                        file1.Close()
                        file2.Close()
                        Dim IMEI As String = ""
                        Dim uid As Integer = 0
                        For i = 0 To UBound(bytes1) - 15
                            Dim fail As Boolean = False
                            For j = 0 To 14
                                If bytes1(i + j) < 48 Or bytes1(i + j) > 57 Then
                                    fail = True
                                    Exit For
                                End If
                            Next
                            If fail = False Then
                                IMEI = Encoding.ASCII.GetString(bytes1, i, 15)
                                Dim search As Byte() = New Byte(5) {&H73, &H71, &H0, &H7E, &H0, &H2}
                                For j = 0 To UBound(bytes2) - 10
                                    fail = False
                                    For k = 0 To 5
                                        If bytes2(j + k) <> search(k) Then
                                            fail = True
                                            Exit For
                                        End If
                                    Next
                                    If fail = False Then
                                        uid = Convert.ToUInt32(bytes2(j + 6)) * &H1000000UI + Convert.ToUInt32(bytes2(j + 7)) * &H10000UI + Convert.ToUInt32(bytes2(j + 8)) * &H100UI + Convert.ToUInt32(bytes2(j + 9)) * &H1UI
                                        Dim tPass As String = TryPassword(MD5(IMEI + Trim(uid), 7), FilInfo.FullName + "\EnMicroMsg.db")
                                        If tPass <> "" Then
                                            passwords.Add(tPass)
                                            GotPass = True
                                        End If
                                    End If
                                Next
                            End If
                        Next
                        If GotPass = False Then passwords.Add("")
                        ComboBox1.Items.Add(IIf(GotPass, "(已解密)", "(未解密)") + FilInfo.Name)
                    End If
                Next
            Catch ex As Exception
                MsgBox(ex.ToString)
                Exit Sub
            End Try
        End If
        ComboBox1.SelectedIndex = 0
        ComboBox1.Text = ComboBox1.Items(0).ToString
        If ComboBox1.Items.Count = 1 Then
            Label2.Text = "未找到"
            Label2.ForeColor = Color.Red
            Button2.Enabled = False
        End If
    End Sub

    Public Function MD5(ByVal strSource As String, Optional ByVal Code As Short = 32) As String
        Dim bytes As Byte() = New ASCIIEncoding().GetBytes(strSource)
        Dim buffer2 As Byte() = DirectCast(CryptoConfig.CreateFromName("MD5"), HashAlgorithm).ComputeHash(bytes)
        Dim str As String = ""
        Select Case Code
            Case 16
                For num = 4 To 11
                    str = str & Conversion.Hex(buffer2(num)).PadLeft(2, "0"c).ToLower
                Next
                Return str
            Case Else
                For num = 0 To 15
                    str = str & Conversion.Hex(buffer2(num)).PadLeft(2, "0"c).ToLower
                Next
                If Code = 7 Then Return str.Substring(0, 7)
                Return str
        End Select
    End Function

    Function TryPassword(ByVal pass As String, ByVal db As String) As String
        Dim proc As New Process
        Dim pinfo As New ProcessStartInfo("sqlcipher.exe", """" + db + """")
        pinfo.UseShellExecute = False
        pinfo.RedirectStandardInput = True
        pinfo.RedirectStandardError = True
        pinfo.WindowStyle = ProcessWindowStyle.Hidden
        pinfo.CreateNoWindow = True
        proc.StartInfo = pinfo
        proc.Start()
        Dim sw As StreamWriter = proc.StandardInput
        Dim sr As StreamReader = proc.StandardError
        sw.WriteLine("PRAGMA key='" + pass + "';")
        sw.WriteLine("PRAGMA cipher_use_hmac=off;")
        sw.WriteLine(".schema")
        sw.WriteLine(".quit")
        sw.Flush()
        Dim out As String = sr.ReadLine
        proc.Close()
        If out IsNot Nothing AndAlso out.StartsWith("Error") Then
            Return ""
        Else
            Return pass
        End If
    End Function

End Class
