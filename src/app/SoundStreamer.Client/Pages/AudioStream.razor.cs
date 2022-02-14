using System.Diagnostics;
using Microsoft.AspNetCore.Components;
using SoundStreamer.Services;

namespace SoundStreamer.Client.Pages;

public partial class AudioStream
{
    [Inject] IAudioRecorder ARecorder { get; set; }
    
    private async Task StartStream()
    {
        if (ARecorder.IsRecording is false)
        {
            var audioStream = await ARecorder.StartRecordingAsync();
        }
        else
        {
            ARecorder.StopRecording();
        }

    }
}