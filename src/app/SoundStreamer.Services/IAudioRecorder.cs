namespace SoundStreamer.Services;

public interface IAudioRecorder
{
    bool IsRecording { get; }
    Task<Stream> StartRecordingAsync();
    void StopRecording();
}