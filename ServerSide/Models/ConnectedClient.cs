using System.Net.Sockets;
using System.Net;

namespace ServerSide.Models;

public class ConnectedClient
{
    public IPEndPoint RemoteEndPoint { get; }
    public UdpClient Client { get; }

    public ConnectedClient(IPEndPoint remoteEndPoint, UdpClient client)
    {
        RemoteEndPoint = remoteEndPoint;
        Client = client;
    }
}