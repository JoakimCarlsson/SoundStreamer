namespace SoundStreamer.Services;

public interface IAudioPlayer
{
    public Task PlayAudioAsync(Stream audioStream);
}