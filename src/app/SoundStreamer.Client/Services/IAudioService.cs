namespace SoundStreamer.Client.Services;

public interface IAudioService
{
    public bool IsRecording { get; }
    public Task StartRecordingAsync();
    public void StopRecording();
    public Queue<byte[]> _audioBufferQueue { get; set; }
}