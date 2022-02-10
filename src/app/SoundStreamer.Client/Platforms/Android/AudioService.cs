using System.Diagnostics;
using Android.Media;
using SoundStreamer.Client.Services;

namespace SoundStreamer.Client;

public class AudioService : IAudioService
{
    private Action<bool> _recordingStateChanged;
    public bool IsRecording => _audioRecord is {RecordingState: RecordState.Recording};
    private string _tempFilePath;
    private byte[] _audioBuffer;
    private AudioRecord _audioRecord;
    private bool _endRecording;
    private byte[] _buffer;
    private AudioTrack _audioTrack;
    
    public async Task StartRecordingAsync()
    {
        var permissionStatus = await Permissions.RequestAsync<Permissions.Microphone>();
        if (permissionStatus != PermissionStatus.Granted)
            throw new Exception("Permission to access microphone was denied");

        _tempFilePath ??= Path.Combine(Path.GetTempPath(), "SoundStreamer.wav");
        
        await StartRecorderAsync();
    }
    
    public void StopRecording()
    {
        _endRecording = true;
    }
    
    private async Task StartRecorderAsync()
    {
        _endRecording = false;

        _audioBuffer = new byte[1024];
        _audioRecord = new AudioRecord(
            AudioSource.Mic,
            16000,
            ChannelIn.Mono,
            Encoding.Pcm16bit,
            _audioBuffer.Length
        );

        _audioRecord.StartRecording();

        await ReadAudioAsync();
    }
    
    private async Task ReadAudioAsync()
    {
        await using (var fileStream = new FileStream(_tempFilePath, FileMode.Create, FileAccess.Write))
        {
            while (true)
            {
                if (_endRecording)
                {
                    _endRecording = false;
                    break;
                }

                try
                {
                    int numBytes = await _audioRecord.ReadAsync(_audioBuffer, 0, _audioBuffer.Length);
                    await fileStream.WriteAsync(_audioBuffer, 0, numBytes);
                }
                catch (Exception ex)
                {
                    Debug.WriteLine(ex.Message);
                    break;
                }
            }

            fileStream.Close();
        }

        _audioRecord.Stop();
        _audioRecord.Release();
        await PlaybackAsync();
    }

    private async Task PlaybackAsync()
    {
        var fileStream = new FileStream(_tempFilePath, FileMode.Open, FileAccess.Read);
        var binaryReader = new BinaryReader(fileStream);
        var totalBytes = new FileInfo(_tempFilePath).Length;
        _buffer = binaryReader.ReadBytes((int)totalBytes);
        fileStream.Close();
        await fileStream.DisposeAsync();
        binaryReader.Close();
        await PlayAudioTrackAsync();
    }

    private async Task PlayAudioTrackAsync()
    {
        _audioTrack = new AudioTrack(
            Android.Media.Stream.Music,
            16000,
            ChannelOut.Mono,
            Encoding.Pcm16bit,
            _buffer.Length,
            AudioTrackMode.Stream);

        _audioTrack.Play();

        await _audioTrack.WriteAsync(_buffer, 0, _buffer.Length);
    }
}