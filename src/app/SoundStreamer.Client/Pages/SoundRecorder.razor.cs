using Microsoft.AspNetCore.Components;
using SoundStreamer.Client.Services;

namespace SoundStreamer.Client.Pages;

public partial class SoundRecorder
{
    [Inject] private IAudioService AudioService { get; set; }
    private bool IsRecording => AudioService.IsRecording;

    private async Task RecordSound()
    {
        if (IsRecording)
            AudioService.StopRecording();
        else
            await AudioService.StartRecordingAsync();
    }
}