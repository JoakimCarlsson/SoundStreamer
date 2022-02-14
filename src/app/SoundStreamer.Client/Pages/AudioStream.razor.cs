using System.Diagnostics;
using Microsoft.AspNetCore.Components;
using SoundStreamer.Services;

namespace SoundStreamer.Client.Pages;

public partial class AudioStream
{
    [Inject] IAudioStream AStream { get; set; }
    
    private async Task StartStream()
    {
        var audioStream = AStream.StartRecording();
        Debug.WriteLine(audioStream.Length);
    }
}