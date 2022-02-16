using AVFoundation;
using Foundation;

namespace SoundStreamer.Services;
//https://social.msdn.microsoft.com/Forums/en-US/c24a49c2-951f-4c5d-82ec-24de2b6d2e78/how-to-record-and-play-audio-in-xamarinios-using-bytes-array?forum=xamarinios

//theory.
//https://developer.apple.com/library/ios/documentation/Audio/Conceptual/AudioSessionProgrammingGuide/Introduction/Introduction.html
//https://developer.apple.com/library/ios/documentation/Audio/Conceptual/AudioSessionProgrammingGuide/AudioSessionServices/AudioSessionServices.html
//return queue<byte[]>
//Start recording too file.
//read bytes from file.
//add bytes to queue.
// ?????
// Profit
//https://blazebin.io/qmujgtvugadc/0
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
            LinearPcmBitDepth = 16,
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