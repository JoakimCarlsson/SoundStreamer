using System.Security.Cryptography.X509Certificates;

namespace SoundStreamer.Client.Services;

public interface IAudioService
{
    public bool IsRecording { get; }
    public Task StartRecordingAsync();
}