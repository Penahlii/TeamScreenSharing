using System.IO;
using System.Net.Sockets;
using System.Net;
using System.Windows;
using System.Windows.Media.Imaging;

namespace ClientSide;

public partial class MainWindow : Window
{
    string checkIP = Directory.GetCurrentDirectory().ToString() == "C:\\Users\\asus\\source\\repos\\NetworkTeamScreenSharing\\ServerSide\\bin\\Debug\\net6.0" ? "192.168.56.1" : "127.0.0.1";
    public UdpClient client;
    public IPAddress remoteIP;
    public int remotePort;
    public IPEndPoint remoteEP;


    public MainWindow()
    {
        InitializeComponent();

        remoteIP = IPAddress.Parse("192.168.56.1");
        remotePort = 27001;
        remoteEP = new IPEndPoint(remoteIP, remotePort);

        client = new UdpClient();
    }




    public async Task<BitmapImage?> ByteToImageAsync(byte[]? imageData)
    {
        var image = new BitmapImage();

        image.BeginInit();

        image.StreamSource = new MemoryStream(imageData);
        image.CacheOption = BitmapCacheOption.OnLoad;

        image.EndInit();

        return image;
    }

    private async void StartScreenSharing(object sender, RoutedEventArgs e)
    {
        var maxLen = ushort.MaxValue - 29;
        var len = 0;
        var buffer = new byte[maxLen];
        await client.SendAsync(buffer, buffer.Length, remoteEP);

        var list = new List<byte>();
        while (true)
        {
            do
            {
                try
                {
                    var result = await client.ReceiveAsync();

                    buffer = result.Buffer;
                    len = buffer.Length;
                    list.AddRange(buffer);
                }
                catch (Exception ex)
                {
                    MessageBox.Show(ex.Message);
                }
            } while (len == maxLen);

            var image = await ByteToImageAsync(list.ToArray());

            if (image is not null)
                ScreenShare.Source = image;

            list.Clear();
        }
    }
}