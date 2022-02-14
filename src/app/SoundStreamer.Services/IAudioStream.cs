namespace SoundStreamer.Services;

public interface IAudioStream
{
    bool IsRecording { get; }
    Stream StartRecording();
    void StopRecording();
}