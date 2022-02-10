using System.Diagnostics;
using Android.Media;
using SoundStreamer.Client.Services;

namespace SoundStreamer.Client;

public class AudioService : IAudioService
{
    private Action<bool> _recordingStateChanged;
    public bool IsRecording => _audioRecord is {RecordingState: RecordState.Recording};
    private bool _test;
    public async Task StartRecordingAsync()
    {
        if (_test is false)
        {
            _test = true;
            await StartAsync();
        }
        else
        {
            _test = false;
            Stop();
        }
    }

    private string _tempFilePath;
    byte[] _audioBuffer = null;
    AudioRecord _audioRecord = null;
    bool _endRecording = false;
    bool _isRecording = false;

    public async Task ReadAudioAsync()
    {
        var permissionStatus = await Permissions.RequestAsync<Permissions.Microphone>();
        if (permissionStatus != PermissionStatus.Granted)
            throw new Exception("Permission to access microphone was denied");

        _tempFilePath ??= Path.Combine(Path.GetTempPath(), "SoundStreamer.wav");

        using (var fileStream = new FileStream(_tempFilePath, FileMode.Create, FileAccess.Write))
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
        _isRecording = false;
        await StartAsyncTest();
    }

    private void RaiseRecordingStateChangedEvent()
    {
        if (_recordingStateChanged != null)
            _recordingStateChanged(_isRecording);
    }

    protected async Task StartRecorderAsync()
    {
        _endRecording = false;
        _isRecording = true;

        RaiseRecordingStateChangedEvent();

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

    public async Task StartAsync()
    {
        await StartRecorderAsync();
    }

    public void Stop()
    {
        _endRecording = true;
        Thread.Sleep(500); // Give it time to drop out.
    }

    byte[] _buffer = null;
    AudioTrack _audioTrack = null;
    public async Task PlaybackAsync()
    {
        FileStream fileStream = new FileStream(_tempFilePath, FileMode.Open, FileAccess.Read);
        BinaryReader binaryReader = new BinaryReader(fileStream);
        long totalBytes = new FileInfo(_tempFilePath).Length;
        _buffer = binaryReader.ReadBytes((Int32)totalBytes);
        fileStream.Close();
        fileStream.Dispose();
        binaryReader.Close();
        await PlayAudioTrackAsync();
    }

    protected async Task PlayAudioTrackAsync()
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

    public async Task StartAsyncTest()
    {
        await PlaybackAsync();
    }
}