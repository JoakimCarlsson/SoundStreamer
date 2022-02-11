using System.Net;
using System.Net.Sockets;
using Microsoft.AspNetCore.Components;
using SoundStreamer.Client.Services;

namespace SoundStreamer.Client.Pages;

public partial class SoundRecorder
{
    //192.168.0.107:69
    private string _hostName = "192.168.0.107";
    private int _port = 69;

    [Inject] private IAudioService AudioService { get; set; }
    private bool IsRecording => AudioService.IsRecording;

    private async Task RecordSound()
    {
        if (IsRecording)
            AudioService.StopRecording();
        else
            await AudioService.StartRecordingAsync();
    }

    private async Task Connect()
    {
        var tcpClient = new TcpClient();
        await tcpClient.ConnectAsync(_hostName, _port);
        var networkStream = tcpClient.GetStream();
        
    }
}