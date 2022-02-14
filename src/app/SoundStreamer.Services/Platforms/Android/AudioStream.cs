using Android.Media;
using Stream = System.IO.Stream;

namespace SoundStreamer.Services;

public class AudioStream : IAudioStream
{
    public bool IsRecording  => _audioRecord is {RecordingState: RecordState.Recording};
    private static readonly int _audioBufferSize = AudioRecord.GetMinBufferSize(16000, ChannelIn.Mono, Encoding.Pcm16bit);
    private byte[] _audioBuffer = new byte[_audioBufferSize];
    private AudioRecord _audioRecord;
    private MemoryStream _audioStream;
    public Stream StartRecording()
    {
        _audioRecord ??= new AudioRecord(
            AudioSource.Mic,
            16000,
            ChannelIn.Mono,
            Encoding.Pcm16bit,
            _audioBufferSize);

        _audioStream ??= new MemoryStream();
        
        _audioRecord.StartRecording();

        while (IsRecording)
        {
            int bytesRead = _audioRecord.Read(_audioBuffer, 0, _audioBufferSize);
            _audioStream.Write(_audioBuffer, 0, bytesRead);
        }

        return _audioStream;
    }

    public void StopRecording()
    {
        throw new NotImplementedException();
    }
}