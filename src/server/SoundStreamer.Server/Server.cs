using System.Net;
using System.Net.Sockets;

namespace SoundStreamer.Server;

public class Server
{
    public void Start()
    {
        var hostName = string.Empty;
        const int port = 69;
        
        var localIp = Dns.GetHostEntry(Dns.GetHostName()).AddressList.FirstOrDefault(ip => ip.AddressFamily == AddressFamily.InterNetwork);
        if (localIp != null)
            hostName = localIp.ToString();
        
        var listener = new TcpListener(IPAddress.Parse(hostName), port);
        listener.Start();

        Console.WriteLine("Server has started on {0}:{1}.{2}Waiting for a connection...", hostName, port, Environment.NewLine);
        
        var audioBuffer = new byte[1024];
        var audioBufferQueue = new Queue<byte[]>();
        
        while (true)
        {
            var client = listener.AcceptTcpClient();
            Console.WriteLine("Client connected from {0}:{1}", client.Client.RemoteEndPoint, Environment.NewLine);
            
            var stream = client.GetStream();
            var buffer = new byte[client.ReceiveBufferSize];
            var bytesRead = stream.Read(buffer, 0, client.ReceiveBufferSize);
            
            while (bytesRead > 0)
            {
                var audioData = new byte[bytesRead];
                Array.Copy(buffer, audioData, bytesRead);
                audioBufferQueue.Enqueue(audioData);
                
                bytesRead = stream.Read(buffer, 0, client.ReceiveBufferSize);
            }
            
            // while (audioBufferQueue.Count > 0)
            // {
            //     var audioData = audioBufferQueue.Dequeue();
            //     Array.Copy(audioData, audioBuffer, audioData.Length);
            //     stream.Write(audioBuffer, 0, audioData.Length);
            // }
            
            client.Close();
        }
    }
}