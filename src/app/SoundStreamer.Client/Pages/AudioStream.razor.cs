using System.Diagnostics;
using Microsoft.AspNetCore.Components;
using SoundStreamer.Services;

namespace SoundStreamer.Client.Pages;

public partial class AudioStream
{
    [Inject] IAudioRecorder AudioRecorder { get; set; }
    [Inject] IAudioPlayer AudioPlayer { get; set; }
    
    private async Task StartStream()
    {
        if (AudioRecorder.IsRecording is false)
        {
            var audioQueue = await AudioRecorder.StartRecordingAsync();
            // await AudioPlayer.PlayAudioAsync(audioStream);
        }
        else
        {
            AudioRecorder.StopRecording();
        }

    }
}