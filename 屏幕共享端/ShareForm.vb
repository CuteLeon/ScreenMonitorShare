Imports System.Drawing.Imaging
Imports System.Net.Sockets
Imports System.Threading

Public Class ShareForm
    Dim ConnectPort As Integer = 12345
    Dim RemoteIP As String = "localhost"
    Dim UDPClient As New UdpClient()
    Dim ShareThread As Thread

    Private Sub ShareForm_Load(sender As Object, e As EventArgs) Handles MyBase.Load
        ShareThread = New Thread(AddressOf ShareScreen)
        'ShareThread.Start()
    End Sub

    Private Sub ShareScreen()
        Dim ScreenBitmap As Bitmap = New Bitmap(Screen.PrimaryScreen.Bounds.Width, Screen.PrimaryScreen.Bounds.Height)
        Dim ScreenGraphics As Graphics = Graphics.FromImage(ScreenBitmap)
        Dim EmptyPoint As Point = New Point(0, 0)
        Dim BitmapStream As IO.MemoryStream = New IO.MemoryStream()
        Dim TempBytes(255) As Byte
        While True
            ScreenGraphics.CopyFromScreen(EmptyPoint, EmptyPoint, Screen.PrimaryScreen.Bounds.Size)
            ScreenBitmap.Save(BitmapStream, ImageFormat.Jpeg)
            Dim BitmapByte() As Byte = BitmapStream.GetBuffer
            Debug.Print("- 发送端：即将发送的数据长度：" & BitmapStream.Length)
            UDPClient.Send(BitmapByte, BitmapByte.Length, RemoteIP, ConnectPort)
            Debug.Print("- 发送端：成功发送。")
            Thread.Sleep(1000)
        End While
    End Sub

    Private Sub Button1_Click(sender As Object, e As EventArgs) Handles Button1.Click
        Dim ScreenBitmap As Bitmap = New Bitmap(Screen.PrimaryScreen.Bounds.Width, Screen.PrimaryScreen.Bounds.Height)
        Dim ScreenGraphics As Graphics = Graphics.FromImage(ScreenBitmap)
        Dim EmptyPoint As Point = New Point(0, 0)
        Dim BitmapStream As IO.MemoryStream = New IO.MemoryStream()
        ScreenGraphics.CopyFromScreen(EmptyPoint, EmptyPoint, Screen.PrimaryScreen.Bounds.Size)
        ScreenBitmap.Save(BitmapStream, ImageFormat.Jpeg)
        Dim BitmapByte() As Byte = BitmapStream.GetBuffer
        Dim TempBytes(255) As Byte

        Debug.Print("- 发送端：即将发送的数据长度：" & BitmapStream.Length)
        UDPClient.Send(BitConverter.GetBytes(BitmapStream.Length), 8, RemoteIP, ConnectPort)

        While True
            Dim TempLength As Integer
            TempLength = BitmapStream.Read(TempBytes, 0, TempBytes.Length)
            If TempLength <= 0 Then Exit While
            UDPClient.Send(TempBytes, TempLength, RemoteIP, ConnectPort)
        End While

        Debug.Print("- 发送端：成功发送。")
        UDPClient.Close()
    End Sub
End Class
