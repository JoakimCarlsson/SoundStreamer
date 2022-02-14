using System.Net;
using System.Net.Sockets;
using System.Text;

namespace SoundStreamer.Server;

public class TcpServer
{
    private Socket? _socketListener;
    private List<Socket> _clientSockets;
    private IPEndPoint _ipEndPoint;
    private readonly IPHostEntry _ipHostInfo;

    private Queue<byte[]> _messageQueue;
    private byte[] _buffer;
    private IPAddress _ipAddress;
    private const int BufferSize = 1024;


    public TcpServer()
    {
        _ipHostInfo = Dns.GetHostEntry(Dns.GetHostName());
        _ipAddress = _ipHostInfo.AddressList.First(x => x.AddressFamily == AddressFamily.InterNetwork);
        _ipEndPoint = new IPEndPoint(_ipAddress, 69);

        _clientSockets = new List<Socket>();
        _messageQueue = new Queue<byte[]>();
    }


    public void Initialize()
    {
        _socketListener = new Socket(_ipAddress.AddressFamily, SocketType.Stream, ProtocolType.Tcp);
        _buffer = new byte[BufferSize];

        Console.WriteLine($"Starting server on {_ipAddress}:{_ipEndPoint.Port}");
        _socketListener.Bind(_ipEndPoint);
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
                //process client socket
                _ = ProcessClientAsync(clientSocket);
                _ = Bajs(clientSocket);
                //_clientSockets.Add(clientSocket);
                Console.WriteLine($"Client connected from {clientSocket.RemoteEndPoint}");
            }
        });

    }

    private object Bajs(Socket clientSocket)
    {
        return Task.Run(async () =>
        {
            while (true)
            {
                if (_messageQueue.TryDequeue(out var buffer))
                {
                    Console.WriteLine($"sent: {buffer}");
                    _ =  clientSocket.SendAsync(buffer, SocketFlags.None);
                }
            }
        });
    }

    private Task ProcessClientAsync(Socket socket)
    {
        _clientSockets.Add(socket);

        return Task.Run(async () =>
        {
            while (true)
            {
                var bytesRead = await socket.ReceiveAsync(_buffer, SocketFlags.None);
                if (bytesRead == 0)
                {
                    socket.Shutdown(SocketShutdown.Both);
                    socket.Close();
                    _clientSockets.Remove(socket);
                    break;
                }
                else
                {
                    var message = new byte[bytesRead];
                    Array.Copy(_buffer, message, bytesRead);
                    _messageQueue.Enqueue(message);
                    Console.WriteLine($"Current queue: {_messageQueue.Count}");
                }
            }
        });
    }

    public void StartMessageLoop()
    {
        //if (_socketListener is null)
        //    throw new InvalidOperationException("Server not initialized");

        //Console.WriteLine("Starting message loop");

        //_ = Task.Run(async () =>
        //{
        //    while (true)
        //    {
        //        var res = await _socketListener.ReceiveMessageFromAsync(_buffer, SocketFlags.None, _clientSockets.First().RemoteEndPoint!);
        //        Console.WriteLine(res.RemoteEndPoint);
        //    }
        //});
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