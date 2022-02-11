using System.Diagnostics;
using Android.Media;
using SoundStreamer.Client.Services;
using Stream = Android.Media.Stream;

namespace SoundStreamer.Client;

public class AudioService : IAudioService
{
    public bool IsRecording => _audioRecord is {RecordingState: RecordState.Recording};

    private AudioRecord _audioRecord;
    private AudioTrack _tempAudioTrack;
    public Queue<byte[]> _audioBufferQueue { get; set; } = new();

    // private readonly Queue<byte[]> _audioBufferQueue = new();
    private int _bufferSize;
    private byte[] _audioBuffer;

    public async Task StartRecordingAsync()
    {
        var permissionStatus = await Permissions.RequestAsync<Permissions.Microphone>();
        if (permissionStatus != PermissionStatus.Granted)
            throw new Exception("Permission to access microphone was denied");

        await StartRecorderAsync();
    }

    public void StopRecording()
    {
        _audioRecord?.Stop();
    }

    private async Task StartRecorderAsync()
    {
        _bufferSize = AudioTrack.GetMinBufferSize(16000, ChannelOut.Mono, Encoding.Pcm16bit);
        _audioBuffer = new byte[_bufferSize];
        _audioRecord = new AudioRecord(
            AudioSource.Mic,
            16000,
            ChannelIn.Mono,
            Encoding.Pcm16bit,
            AudioTrack.GetMinBufferSize(16000, ChannelOut.Mono, Encoding.Pcm16bit)
        );

        _audioRecord.StartRecording();
        
        while (IsRecording)
        {
            try
            {
                await _audioRecord.ReadAsync(_audioBuffer, 0, _audioBuffer.Length);
                _audioBufferQueue.Enqueue((byte[]) _audioBuffer.Clone());
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex.Message);
                break;
            }
        }
        
        _audioRecord.Release();

        while (_audioBufferQueue.TryDequeue(out var audioBuffer))
            await PlayAudioBuffer(audioBuffer);
    }

    private async Task PlayAudioBuffer(byte[] buffer)
    {
        _tempAudioTrack ??= new AudioTrack(
            Stream.Music,
            16000,
            ChannelOut.Mono,
            Encoding.Pcm16bit,
            buffer.Length,
            AudioTrackMode.Stream);

        if (_tempAudioTrack.PlayState is PlayState.Paused or PlayState.Stopped)
            _tempAudioTrack.Play();

        _tempAudioTrack.Flush();
        await _tempAudioTrack.WriteAsync(buffer, 0, buffer.Length);
    }
}