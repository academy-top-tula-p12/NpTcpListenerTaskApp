// TcpListener
using System.Net;
using System.Net.Sockets;
using System.Text;

string marker = "end";

IPAddress address = IPAddress.Loopback;
int port = 5000;
IPEndPoint endPoint = new(address, port);
TcpListener listener = new(endPoint);

try
{
    listener.Start();
    Console.WriteLine($"Server {listener.LocalEndpoint} start...");

    while(true)
    {
        TcpClient client = await listener.AcceptTcpClientAsync();
        Console.WriteLine($"Server accept client {client.Client.RemoteEndPoint}");

        //new Thread(async () => await ClientProcessAsync(client)).Start();
        Task.Run(async () => await ClientProcessAsync(client));
    }
}
finally
{
    listener.Stop();
}





async Task ClientProcessAsync(TcpClient client)
{
    var stream = client.GetStream();
    List<byte> data = new List<byte>();

    int byteRead = 10;

    while(true)
    {
        data.Clear();

        while ((byteRead = stream.ReadByte()) != '\n')
            data.Add((byte)byteRead);

        string message = Encoding.UTF8.GetString(data.ToArray());

        if (message == marker) break;

        Console.WriteLine($"Client {client.Client.RemoteEndPoint}: {message}");

        message = DateTime.Now.ToLongTimeString() + " " + message + "\n";
        await stream.WriteAsync(Encoding.UTF8.GetBytes(message));
    }

    client.Close();
}