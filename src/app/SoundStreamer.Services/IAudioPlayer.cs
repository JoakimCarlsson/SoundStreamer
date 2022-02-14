namespace SoundStreamer.Services;

public interface IAudioPlayer
{
    public Task PlayAudioAsync(Queue<byte[]> audioData);
}