using Android.Media;
using Android.OS;
using Debug = System.Diagnostics.Debug;
using Stream = System.IO.Stream;

namespace SoundStreamer.Services;

public class AudioStream : IAudioStream
{
    public bool IsRecording  => _audioRecord is {RecordingState: RecordState.Recording};
    private static readonly int _audioBufferSize = AudioRecord.GetMinBufferSize(16000, ChannelIn.Mono, Encoding.Pcm16bit);
    private byte[] _audioBuffer = new byte[_audioBufferSize];
    private AudioRecord _audioRecord;
    private MemoryStream _audioStream;
    public async Task<Stream> StartRecordingAsync()
    {
        var permissionStatus = await Permissions.RequestAsync<Permissions.Microphone>();
        if (permissionStatus != PermissionStatus.Granted)
            throw new Exception("Permission to access microphone was denied");

        _audioRecord ??= new AudioRecord(
            AudioSource.Mic,
            16000,
            ChannelIn.Mono,
            Encoding.Pcm16bit,
            _audioBufferSize);

        _audioStream ??= new MemoryStream();
        
        _audioRecord.StartRecording();

        _ = Task.Run(async () =>
        {
            while (IsRecording)
            {
                var read = await _audioRecord.ReadAsync(_audioBuffer, 0, _audioBufferSize);
                if (read > 0)
                {
                    await _audioStream.WriteAsync(_audioBuffer, 0, read);
                }
                else
                {
                    Debug.WriteLine("AudioRecord.Read returned 0");
                    break;
                }
            }
        });

        return _audioStream;
    }

    public void StopRecording()
    {
        throw new NotImplementedException();
    }
}