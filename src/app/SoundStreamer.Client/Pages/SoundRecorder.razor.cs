using Microsoft.AspNetCore.Components;
using SoundStreamer.Client.Services;

namespace SoundStreamer.Client.Pages;

public partial class SoundRecorder
{
    [Inject] private IAudioService AudioService { get; set; }
    
    private async Task RecordSound()
    {
        await AudioService.StartRecordingAsync();
    }
}