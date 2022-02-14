using Android.Media;
using Android.OS;
using Debug = System.Diagnostics.Debug;
using Stream = System.IO.Stream;

namespace SoundStreamer.Services;

public class AudioPlayer : IAudioPlayer
{
    private AudioTrack _audioTrack;
    private static readonly int _audioBufferSize = AudioTrack.GetMinBufferSize(16000, ChannelOut.Mono, Encoding.Pcm16bit);
    private byte[] _audioBuffer = new byte[_audioBufferSize];
    private int offset = 0;
    public async Task PlayAudioAsync(Stream audioStream)
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

        //while ((bytesRead = await audioStream.ReadAsync(_audioBuffer, 0, _audioBuffer.Length)) > 0)
        while (true)
        {
            //await audioStream.ReadExactlyAsync(_audioBuffer, 0, _audioBuffer.Length);
            int bytesRead = await audioStream.ReadAsync(_audioBuffer, 0, _audioBuffer.Length);
            Debug.WriteLine($"Bytes read: {bytesRead}");
            await _audioTrack.WriteAsync(_audioBuffer, 0, bytesRead);
        }
    }
}