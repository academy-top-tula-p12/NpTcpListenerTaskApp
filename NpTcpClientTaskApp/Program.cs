using System;
using System.Net;
using System.Net.Sockets;
using System.Text;

IPAddress address = IPAddress.Loopback;
int port = 5000;
IPEndPoint endPoint = new(address, port);

using TcpClient client = new();
await client.ConnectAsync(endPoint);
Console.WriteLine($"Client {client.Client.LocalEndPoint}");
var stream = client.GetStream();

List<byte> data = new();
int byteRead = 10;

while(true)
{
    Console.Write("Input message (end - stop)");
    string message = Console.ReadLine() + "\n";

    if (message == "end") break;

    byte[] buffer = Encoding.UTF8.GetBytes(message);
    await stream.WriteAsync(buffer);
    
    data.Clear();
    while ((byteRead = stream.ReadByte()) != '\n')
        data.Add((byte)byteRead);

    message = Encoding.UTF8.GetString(data.ToArray());
    Console.WriteLine($"Server {client.Client.RemoteEndPoint}: {message}");
}

await stream.WriteAsync(Encoding.UTF8.GetBytes("end"));