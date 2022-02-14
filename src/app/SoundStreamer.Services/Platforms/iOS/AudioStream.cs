namespace SoundStreamer.Services;

public class AudioRecorder : IAudioRecorder
{
    public bool IsRecording { get; }

    public Task<Queue<byte[]>> StartRecordingAsync()
    {
        throw new NotImplementedException();
    }

    public void StopRecording()
    {
        throw new NotImplementedException();
    }
}