using AVFoundation;
using Foundation;

namespace SoundStreamer.Services;

public class AudioRecorder : IAudioRecorder
{
    public bool IsRecording => _status == Status.Recording;
    private AVAudioRecorder _recorder;
    private Queue<byte[]> _audioQueue = new(); //can instansitate this variable in the method.
    private Status _status;
    
    public async Task<Queue<byte[]>> StartRecordingAsync()
    {
        var audioSession = AVAudioSession.SharedInstance();
        audioSession.RequestRecordPermission((granted) => 
        {
            if (granted is false)
                throw new Exception("No permission to record audio");
            
            audioSession.SetCategory(AVAudioSession.CategoryRecord, out NSError error);
            //if (error is null)
            //    throw new Exception("Error setting audio session category");

            var isPrepared = PrepareAudioRecording() && _recorder.Record();
            if (isPrepared)
            {
                _status = Status.Recording;
            }
            else
            {
                _status = Status.PreparingError;
            }
        });

        return _audioQueue;
    }
    
    public void StopRecording()
    {
        throw new NotImplementedException();
    }
    
    private bool PrepareAudioRecording()
    {
        var result = false;
        var audioSettings = new AudioSettings
        {
            SampleRate = 16000,
            NumberChannels = 1,
            AudioQuality = AVAudioQuality.High,
            Format = AudioToolbox.AudioFormatType.LinearPCM,
        };
        
        _recorder = AVAudioRecorder.Create(NSUrl.FromFilename("/dev/null"), audioSettings, out NSError error);
        if (error is null)
        {
            _recorder.MeteringEnabled = true;
            _recorder.PrepareToRecord();
            result = true;
        }
        else
        {
            _status = Status.PreparingError;
        }

        return result;
    }
}