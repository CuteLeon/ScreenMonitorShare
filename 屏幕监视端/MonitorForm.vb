Imports System.IO
Imports System.Net
Imports System.Net.Sockets
Imports System.Threading

Public Class MonitorForm
    Dim ConnectPort As Integer = 12345
    Dim UDPClient As New UdpClient(ConnectPort)
    Dim MonitorThread As Thread

    Private Sub MonitorForm_Load(sender As Object, e As EventArgs) Handles MyBase.Load
        MonitorThread = New Thread(AddressOf ReceiveScreen)
        MonitorThread.Start()
    End Sub

    Private Sub ReceiveScreen()
        While True
            Dim RemoteIpEndPoint As New IPEndPoint(IPAddress.Any, ConnectPort)
            Dim BitmapStream As MemoryStream = New MemoryStream()
            Dim DataLengthBytes(7) As Byte '用于接收图像数据的长度
            DataLengthBytes = UDPClient.Receive(RemoteIpEndPoint)
            Dim DataLength As Long = BitConverter.ToInt64(DataLengthBytes, 0)
            Debug.Print("+ 接收端：即将接收的数据长度：" & DataLength)

            While True
                Dim TempBytes() As Byte = UDPClient.Receive(RemoteIpEndPoint)
                Dim TempLength As Integer = TempBytes.Length
                If TempLength <= 0 Then Exit While
                Debug.Print("收到分组长度：" & TempLength)
                BitmapStream.Write(TempBytes, 0, TempLength)
            End While

            Debug.Print("+ 接收端：成功接收长度：" & DataLength)
            Me.BackgroundImage = Bitmap.FromStream(BitmapStream)
            Debug.Print("+ 接收端：完成图像显示。")

            'MsgBox(vbNullString,, RemoteIpEndPoint.Address.ToString() & ":" & RemoteIpEndPoint.Port)

            UDPClient.Close()
        End While
    End Sub

End Class
