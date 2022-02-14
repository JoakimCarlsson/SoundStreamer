namespace SoundStreamer.Services;

public class AudioStream : IAudioStream
{
    public bool IsRecording { get; }

    public Task<Stream> StartRecordingAsync()
    {
        throw new NotImplementedException();
    }

    public void StopRecording()
    {
        throw new NotImplementedException();
    }
}