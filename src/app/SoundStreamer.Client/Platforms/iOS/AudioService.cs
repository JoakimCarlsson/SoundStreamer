using SoundStreamer.Client.Services;

namespace SoundStreamer.Client;

public class AudioService : IAudioService
{
    public bool IsRecording { get; }
    public Task StartRecordingAsync()
    {
        throw new NotImplementedException();
    }

    public void StopRecording()
    {
        throw new NotImplementedException();
    }

    public byte[] _audioBuffer { get; set; }
}