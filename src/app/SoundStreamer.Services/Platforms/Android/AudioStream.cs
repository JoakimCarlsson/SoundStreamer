using Android.Content;
using Android.Media;
using Android.OS;
using Application = Android.App.Application;
using Debug = System.Diagnostics.Debug;
using Stream = System.IO.Stream;

namespace SoundStreamer.Services;

public class AudioRecorder : IAudioRecorder
{
    public bool IsRecording => _audioRecord is { RecordingState: RecordState.Recording };
    private static readonly int _audioBufferSize = AudioRecord.GetMinBufferSize(16000, ChannelIn.Mono, Encoding.Pcm16bit);
    private byte[] _audioBuffer = new byte[_audioBufferSize];
    private AudioRecord _audioRecord;
    private Queue<byte[]> _audioQueue = new();
    
    public async Task<Queue<byte[]>> StartRecordingAsync()
    {
        var permissionStatus = await Permissions.RequestAsync<Permissions.Microphone>();
        if (permissionStatus != PermissionStatus.Granted)
            throw new Exception("Permission to access microphone was denied");

        var audioManager = (AudioManager)Application.Context.GetSystemService(Context.AudioService);
        audioManager.StartBluetoothSco();

        _audioRecord ??= new AudioRecord(
            AudioSource.Default,
            16000,
            ChannelIn.Mono,
            Encoding.Pcm16bit,
            _audioBufferSize);
        
        if (IsRecording)
            return _audioQueue;

        _audioRecord.StartRecording();

        _ = Task.Run(async () =>
        {
            while (IsRecording)
            {
                var read = await _audioRecord.ReadAsync(_audioBuffer, 0, _audioBufferSize);
                if (read > 0)
                {
                    if (_audioBuffer.All(x => x == 0))
                        continue;
                    
                    _audioQueue.Enqueue((byte[]) _audioBuffer.Clone());
                }
                else
                {
                    Debug.WriteLine("AudioRecord.Read returned 0");
                    throw new Exception("AudioRecord.Read returned 0");
                }
            }
        });

        return _audioQueue;
    }

    public void StopRecording()
    {
        if (IsRecording is false)
            return;

        _audioRecord.Stop();
        _audioRecord.Release();
        _audioRecord = null;
    }
}