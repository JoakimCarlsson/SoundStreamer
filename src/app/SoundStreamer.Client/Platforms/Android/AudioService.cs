using System.Diagnostics;
using Android.Media;
using SoundStreamer.Client.Services;

namespace SoundStreamer.Client;

public class AudioService : IAudioService
{
    private Action<bool> RecordingStateChanged;
    public bool IsRecording { get; }
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
    byte[] audioBuffer = null;
    AudioRecord audioRecord = null;
    bool endRecording = false;
    bool isRecording = false;

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
                if (endRecording)
                {
                    endRecording = false;
                    break;
                }

                try
                {
                    // Keep reading the buffer while there is audio input.
                    int numBytes = await audioRecord.ReadAsync(audioBuffer, 0, audioBuffer.Length);
                    await fileStream.WriteAsync(audioBuffer, 0, numBytes);
                    // Do something with the audio input.
                }
                catch (Exception ex)
                {
                    Debug.WriteLine(ex.Message);
                    break;
                }
            }

            fileStream.Close();
        }

        audioRecord.Stop();
        audioRecord.Release();
        isRecording = false;
        await StartAsyncTest();
    }

    private void RaiseRecordingStateChangedEvent()
    {
        if (RecordingStateChanged != null)
            RecordingStateChanged(isRecording);
    }

    protected async Task StartRecorderAsync()
    {
        endRecording = false;
        isRecording = true;

        RaiseRecordingStateChangedEvent();

        audioBuffer = new byte[1024];
        audioRecord = new AudioRecord(
            AudioSource.Mic,
            16000,
            ChannelIn.Mono,
            Encoding.Pcm16bit,
            audioBuffer.Length
        );

        audioRecord.StartRecording();

        await ReadAudioAsync();
    }

    public async Task StartAsync()
    {
        await StartRecorderAsync();
    }

    public void Stop()
    {
        endRecording = true;
        Thread.Sleep(500); // Give it time to drop out.
    }

    byte[] buffer = null;
    AudioTrack audioTrack = null;
    public async Task PlaybackAsync()
    {
        FileStream fileStream = new FileStream(_tempFilePath, FileMode.Open, FileAccess.Read);
        BinaryReader binaryReader = new BinaryReader(fileStream);
        long totalBytes = new FileInfo(_tempFilePath).Length;
        buffer = binaryReader.ReadBytes((Int32)totalBytes);
        fileStream.Close();
        fileStream.Dispose();
        binaryReader.Close();
        await PlayAudioTrackAsync();
    }

    protected async Task PlayAudioTrackAsync()
    {
        audioTrack = new AudioTrack(
            Android.Media.Stream.Music,
            16000,
            ChannelOut.Mono,
            Encoding.Pcm16bit,
            buffer.Length,
            AudioTrackMode.Stream);

        audioTrack.Play();

        await audioTrack.WriteAsync(buffer, 0, buffer.Length);
    }

    public async Task StartAsyncTest()
    {
        await PlaybackAsync();
    }
}