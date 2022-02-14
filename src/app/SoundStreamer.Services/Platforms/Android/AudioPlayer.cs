using Android.Media;
using Android.OS;
using Debug = System.Diagnostics.Debug;
using Stream = System.IO.Stream;

namespace SoundStreamer.Services;

public class AudioPlayer : IAudioPlayer
{
    private AudioTrack _audioTrack;
    private static readonly int _audioBufferSize = AudioTrack.GetMinBufferSize(16000, ChannelOut.Mono, Encoding.Pcm16bit);

    public async Task PlayAudioAsync(Queue<byte[]> audioData)
    {
        if (_audioTrack is null)
        {
            _audioTrack = new AudioTrack.Builder()
                .SetAudioAttributes(new AudioAttributes.Builder()
                    .SetUsage(AudioUsageKind.Media)
                    .SetContentType(AudioContentType.Speech)
                    .Build())
                .SetAudioFormat(new AudioFormat.Builder()
                    .SetEncoding(Encoding.Pcm16bit)
                    .SetSampleRate(16000)
                    .SetChannelMask(ChannelOut.Mono)
                    .Build())
                .SetBufferSizeInBytes(_audioBufferSize)
                .Build();
        }

        if (_audioTrack.PlayState is PlayState.Paused or PlayState.Stopped)
            _audioTrack.Play();

        _ = Task.Run(async () =>
        {
            while (true)
            {
                if (audioData.TryDequeue(out var audioBuffer))
                { 
                    await _audioTrack.WriteAsync(audioBuffer, 0, audioBuffer.Length);
                }
            }
        });

    }
}