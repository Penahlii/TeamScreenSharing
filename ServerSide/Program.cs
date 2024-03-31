#nullable disable

using System.Net.Sockets;
using System.Net;
using System.Text;
using System.Drawing.Imaging;
using System.Drawing;
using System.Windows.Forms;
using ServerSide.Models;

class Program
{
    public static List<ConnectedClient> connectedClients = new List<ConnectedClient>();

    static async Task Main(string[] args)
    {
        string checkIP = Directory.GetCurrentDirectory().ToString() == "C:\\Users\\asus\\source\\repos\\NetworkTeamScreenSharing\\ServerSide\\bin\\Debug\\net6.0" ? "192.168.56.1" : "127.0.0.1";

        var ip = IPAddress.Parse(checkIP);
        var port = 27001;

        var listenerEP = new IPEndPoint(ip, port);
        var listener = new UdpClient(listenerEP);

        Console.WriteLine("Server started...");

        while (true)
        {
            var result = await listener.ReceiveAsync();
            if (result != null)
            {
                Console.WriteLine($"{result.RemoteEndPoint} Connected To Server.");
                var remoteEP = result.RemoteEndPoint;

                var client = new UdpClient();
                connectedClients.Add(new ConnectedClient(remoteEP, client));

                _ = Task.Run(async () =>
                {
                    while (true)
                    {
                        var screenImage = await TakeScreenShotAsync();
                        var imgBytes = await ImageToByteAsync(screenImage);
                        var chunks = imgBytes?.Chunk(ushort.MaxValue - 29);
                        foreach (var chunk in chunks!)
                        {
                            foreach (var connectedClient in connectedClients)
                            {
                                await connectedClient.Client.SendAsync(chunk, chunk.Length, connectedClient.RemoteEndPoint);
                            }
                        }
                    }
                });
            }
        }
    }

    static async Task<Image?> TakeScreenShotAsync()
    {
        var width = Screen.PrimaryScreen.Bounds.Width;
        var height = Screen.PrimaryScreen.Bounds.Height;
        Bitmap? bitmap = new Bitmap(width, height);
        using Graphics graphics = Graphics.FromImage(bitmap);
        graphics.CopyFromScreen(0, 0, 0, 0, bitmap.Size);

        return bitmap;
    }

    static async Task<byte[]?> ImageToByteAsync(Image? image)
    {
        using MemoryStream ms = new MemoryStream();
        image?.Save(ms, ImageFormat.Jpeg);

        return ms.ToArray();
    }
}