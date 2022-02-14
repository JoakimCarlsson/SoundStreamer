namespace SoundStreamer.Services;

public interface IAudioStream
{
    bool IsRecording { get; }
    Task<Stream> StartRecordingAsync();
    void StopRecording();
}