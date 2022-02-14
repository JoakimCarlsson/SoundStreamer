using System.Net;
using System.Net.Sockets;

namespace SoundStreamer.Server;

public class TcpServer
{
    private Socket? _socketListener;
    private List<Socket> _clientSockets;
    
    public async Task InitializeAsync()
    {
        var ipHostInfo = await Dns.GetHostEntryAsync(Dns.GetHostName());
        var ipAddress = ipHostInfo.AddressList.First(x => x.AddressFamily == AddressFamily.InterNetwork);
        var localEndPoint = new IPEndPoint(ipAddress, 69);
        
        _socketListener = new Socket(ipAddress.AddressFamily, SocketType.Stream, ProtocolType.Tcp);
        _clientSockets = new List<Socket>();

        Console.WriteLine($"Starting server on {ipAddress}:{localEndPoint.Port}");
        _socketListener.Bind(localEndPoint);
        _socketListener.Listen(100);
        Console.WriteLine("Server started");
    }

    public void StartConnectionLoop()
    {
        if (_socketListener is null)
            throw new InvalidOperationException("Server not initialized");
        
        _ = Task.Run(async () =>
        {
            while (true)
            {
                Console.WriteLine("Waiting for a connection...");
                var clientSocket = await _socketListener.AcceptAsync();
                _clientSockets.Add(clientSocket);
                Console.WriteLine($"Client connected from {clientSocket.RemoteEndPoint}");
            }
        });
    }
    
    public void StartMessageLoop()
    {
        
    }
    
    private bool IsSocketConnected(Socket s)
    {
        // https://stackoverflow.com/questions/2661764/how-to-check-if-a-socket-is-connected-disconnected-in-c
        bool part1 = s.Poll(1000, SelectMode.SelectRead);
        bool part2 = (s.Available == 0);
        if ((part1 && part2) || !s.Connected)
            return false;
        else
            return true;
    }
}