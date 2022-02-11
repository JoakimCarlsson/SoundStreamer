using System.Net;
using System.Net.Sockets;

namespace SoundStreamer.Server;

public class Server
{
    public void Start()
    {
        var hostName = string.Empty;
        const int port = 69;
        var bytes = new byte[1024];
        string? data = null;
        
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
            
            data = null;
            var networkStream = client.GetStream();
            int i;
            while((i = networkStream.Read(bytes, 0, bytes.Length))!=0)
            {
                data = System.Text.Encoding.ASCII.GetString(bytes, 0, i);
                Console.WriteLine("Received: {0}", data);

                audioBufferQueue.Enqueue((byte[]) bytes.Clone());

                //data = data.ToUpper();

                //var msg = System.Text.Encoding.ASCII.GetBytes(data);
                //networkStream.Write(msg, 0, msg.Length);
                //Console.WriteLine("Sent: {0}", data);
            }
            
            // while (audioBufferQueue.Count > 0)
            // {
            //     var audioData = audioBufferQueue.Dequeue();
            //     Array.Copy(audioData, audioBuffer, audioData.Length);
            //     stream.Write(audioBuffer, 0, audioData.Length);
            // }
            
            
            client.Close();
            listener.Stop();
            
            var memoryStream = new MemoryStream();

            while (audioBufferQueue.TryDequeue(out var buffer))
            {
                memoryStream.Write(buffer, 0, buffer.Length);

            }

            using (FileStream fs = File.Create("myFile.wav"))
            {
                fs.Write(memoryStream.GetBuffer(), 0, memoryStream.GetBuffer().Length);
            }
            
            File.WriteAllBytes("test.wav", memoryStream.ToArray());
        }
        
        
    }
}