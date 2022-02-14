namespace SoundStreamer.Services;

public class AudioStream : IAudioStream
{
    public bool IsRecording { get; }
    public Stream StartRecording()
    {
        throw new NotImplementedException();
    }

    public void StopRecording()
    {
        throw new NotImplementedException();
    }
}