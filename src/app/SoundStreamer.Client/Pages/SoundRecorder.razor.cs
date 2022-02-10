using Microsoft.AspNetCore.Components;
using SoundStreamer.Client.Services;

namespace SoundStreamer.Client.Pages;

public partial class SoundRecorder
{
    [Inject] private IAudioService AudioService { get; set; }
    private bool _isRecording => AudioService.IsRecording;

    private async Task RecordSound()
    {
        if (_isRecording)
            AudioService.StopRecording();
        else
            await AudioService.StartRecordingAsync();
    }
}