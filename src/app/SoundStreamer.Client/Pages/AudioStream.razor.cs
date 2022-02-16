using System.Net;
using System.Net.Sockets;
using Microsoft.AspNetCore.Components;
using SoundStreamer.Services;

namespace SoundStreamer.Client.Pages;

public partial class AudioStream
{
    [Inject] IAudioRecorder AudioRecorder { get; set; }
    [Inject] IAudioPlayer AudioPlayer { get; set; }
    
    private Queue<byte[]> _recievedBytesQueue = new Queue<byte[]>();
    private byte[] _buffer = new byte[5000];
    private Queue<byte[]> audioQueue = new Queue<byte[]>();

    private async Task StartStream()
    {
        //fix hardcoded values
        var ipHostInfo = await Dns.GetHostEntryAsync(Dns.GetHostName());
        var ipAddress = IPAddress.Parse("192.168.0.107");
        var remoteEndPoint = new IPEndPoint(ipAddress, 69);
        var serverClient = new Socket(ipAddress.AddressFamily, SocketType.Stream, ProtocolType.Tcp);
        
        await serverClient.ConnectAsync(remoteEndPoint);

        if (AudioRecorder.IsRecording is false)
        {

            _ = Task.Run(async () =>
            {
                while (true)
                {
                    if (audioQueue.TryDequeue(out var audioBuffer))
                    {
                        await serverClient.SendAsync(audioBuffer, SocketFlags.None); 
                    }
                }
            });

            _ = Task.Run(async () =>
            {
                while (true)
                {
                    var bytesRead = await serverClient.ReceiveAsync(_buffer, SocketFlags.None);
                    if (bytesRead == 0)
                    {
                        break;
                    }
                    else
                    {
                        var message = new byte[bytesRead];
                        Array.Copy(_buffer, message, bytesRead);
                        _recievedBytesQueue.Enqueue(message);
                    }
                }
            });

            audioQueue = await AudioRecorder.StartRecordingAsync();
            await AudioPlayer.PlayAudioAsync(_recievedBytesQueue);
        }
        else
        {
            AudioRecorder.StopRecording();
            await serverClient.DisconnectAsync(true);
        }
    }
}