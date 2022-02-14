namespace SoundStreamer.Services;

public interface IAudioRecorder
{
    bool IsRecording { get; }
    Task<Queue<byte[]>> StartRecordingAsync();
    void StopRecording();
}