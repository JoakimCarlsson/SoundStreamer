using System.Diagnostics;
using System.Net;
using System.Net.Sockets;
using Microsoft.AspNetCore.Components;
using SoundStreamer.Services;

namespace SoundStreamer.Client.Pages;

public partial class AudioStream
{
    [Inject] IAudioRecorder AudioRecorder { get; set; }
    [Inject] IAudioPlayer AudioPlayer { get; set; }
    
    private async Task StartStream()
    {
        //fix hardcoded values
        var ipHostInfo = await Dns.GetHostEntryAsync(Dns.GetHostName());
        var ipAddress = IPAddress.Parse("192.168.0.107");
        var remoteEndPoint = new IPEndPoint(ipAddress, 69);
        var serverClient = new Socket(ipAddress.AddressFamily, SocketType.Stream, ProtocolType.Tcp);
        
        await serverClient.ConnectAsync(remoteEndPoint);


        // if (AudioRecorder.IsRecording is false)
        // {
        //     var audioQueue = await AudioRecorder.StartRecordingAsync();
        //     await AudioPlayer.PlayAudioAsync(audioQueue);
        // }
        // else
        // {
        //     AudioRecorder.StopRecording();
        // }
        
        
    }
}