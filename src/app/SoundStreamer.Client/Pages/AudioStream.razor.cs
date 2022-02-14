using Microsoft.AspNetCore.Components;
using SoundStreamer.Services;

namespace SoundStreamer.Client.Pages;

public partial class AudioStream
{
    [Inject] IAudioStream Stream { get; set; }
    
    private async Task StartStream()
    {
        
    }
}