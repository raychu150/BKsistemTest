﻿Imports System.Runtime.InteropServices

Namespace My

    ' The following events are available for MyApplication:
    ' 
    ' Startup: Raised when the application starts, before the startup form is created.
    ' Shutdown: Raised after all application forms are closed.  This event is not raised if the application terminates abnormally.
    ' UnhandledException: Raised if the application encounters an unhandled exception.
    ' StartupNextInstance: Raised when launching a single-instance application and the application is already active. 
    ' NetworkAvailabilityChanged: Raised when the network connection is connected or disconnected.
    Partial Friend Class MyApplication
        Private Sub MyApp_Shutdown(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Shutdown
            If logged Then
                'Dim noclose As New Form
                'noclose.Opacity = 0
                'noclose.Show()
                Debug.WriteLine("Shutting down...")
                If HaveInternetConnection() Then
                    Dim adapter As New DatabaseWebAdapter()
                    adapter.UpdateCommand = New DatabaseWebCommand("", New DatabaseWebConnection(My.Settings.ConnectionString))
                    adapter.Update(MainDSO)
                End If
                If My.Settings.SaveData <> "" Then
                    If Not IO.File.Exists(My.Settings.SaveData) Then
                        My.Settings.SaveData = ""
                    End If
                End If
                Dim rst As DialogResult
                If My.Settings.SaveData = "" Then
                    Dim openfileDlg As New SaveFileDialog
                    openfileDlg.Filter = "Respaldo BkSistema (*.bks)|*.bks"
                    openfileDlg.AddExtension = True
                    openfileDlg.Title = "Seleccionar el directorio donde desea respaldar la Data"
                    rst = openfileDlg.ShowDialog()
                    If rst = DialogResult.OK Then
                        My.Settings.SaveData = openfileDlg.FileName
                        My.Settings.Save()
                    End If
                End If
                If rst = DialogResult.OK And My.Settings.SaveData <> "" Then
                    Dim file As IO.StreamWriter
                    Try
                        file = My.Computer.FileSystem.OpenTextFileWriter(My.Settings.SaveData, False)
                        file.Write(JsonConvert.SerializeObject(MainDSO))
                        file.Close()
                        MessageBox.Show("Respaldo exitoso!")
                    Catch ex As Exception
                        MessageBox.Show(ex.Message)
                    End Try
                End If
                logged = False
                'noclose.Close()
            End If
        End Sub
        Private Sub StartupEvent(ByVal sender As Object, ByVal e As Microsoft.VisualBasic.ApplicationServices.StartupEventArgs) Handles Me.Startup
            'SetFileAssoc()

        End Sub
        Private Sub SetFileAssoc()
            Dim assem As Reflection.Assembly = Me.GetType.Assembly
            Dim guid As Guid = New Guid(CType(assem.GetCustomAttributes(GetType(GuidAttribute), False)(0), GuidAttribute).Value)

            ' The AssemblyName type can be used to parse the full name.
            Dim assemName As Reflection.AssemblyName = assem.GetName()
            Dim updateLocation As Uri = System.Deployment.Application.ApplicationDeployment.CurrentDeployment.UpdateLocation
            Dim AppSecurityInfo As New Security.Policy.ApplicationSecurityInfo(AppDomain.CurrentDomain.ActivationContext)
            Dim DeploymentInfo As ApplicationId = AppSecurityInfo.DeploymentId
            Dim PublicKey As Byte() = DeploymentInfo.PublicKeyToken

            Dim ProgID As String = assemName.Name
            Dim fileType As String = ".bks"
            Dim AppId As String = DeploymentInfo.Name & ", Culture=" & DeploymentInfo.Culture & ", PublicKeyToken="
            For i As Integer = 0 To (PublicKey.Length - 1)
                AppId = AppId & PublicKey(i).ToString("X")
            Next
            AppId = AppId & ", processorArchitecture=" & DeploymentInfo.ProcessorArchitecture

            'DeploymentProviderUrl = updateLocation
            Dim command As String = "rundll32.exe dfshim.dll, ShOpenVerbExtension {" & guid.ToString & "} %1"

            Dim keys As String() = My.Computer.Registry.CurrentUser.GetSubKeyNames
            'go to HKCU\Software\Classes
            'create Key ProgId
        End Sub
    End Class
End Namespace

