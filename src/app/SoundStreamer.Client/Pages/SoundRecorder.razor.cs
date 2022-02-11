using System.Diagnostics;
using System.Net;
using System.Net.Sockets;
using Microsoft.AspNetCore.Components;
using SoundStreamer.Client.Services;

namespace SoundStreamer.Client.Pages;

public partial class SoundRecorder
{
    //192.168.0.107:69
    [Inject] private IAudioService AudioService { get; set; }
    private bool IsRecording => AudioService.IsRecording;
    public string IpAddress { get; set; }
    private async Task RecordSound()
    {
        if (IsRecording)
            AudioService.StopRecording();
        else
            await AudioService.StartRecordingAsync();
    }

    private async Task Connect()
    {
        try
        {
            if (IPEndPoint.TryParse(IpAddress, out IPEndPoint endpoint))
            {
                //var tcpClient = new TcpClient();
                //await tcpClient.ConnectAsync(endpoint);
                //var networkStream = tcpClient.GetStream();

                //while (AudioService._audioBufferQueue.TryDequeue(out var buffer))
                //{
                //    await networkStream.WriteAsync(buffer, 0, buffer.Length);
                //}

                //networkStream.Close();
                //tcpClient.Close();
            }
        }
        catch (Exception e)
        {
            Debug.WriteLine(e);
        }
    }
}