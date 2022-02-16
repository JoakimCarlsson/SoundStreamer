namespace SoundStreamer.Services;

public class AudioRecorder : IAudioRecorder
{
    public bool IsRecording { get; }
    private Queue<byte[]> _audioQueue = new(); //can instansitate this variable in the method.

    public Task<Queue<byte[]>> StartRecordingAsync()
    {

    }

    public void StopRecording()
    {
        throw new NotImplementedException();
    }
}