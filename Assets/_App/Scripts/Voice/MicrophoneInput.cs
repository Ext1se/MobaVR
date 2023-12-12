using UnityEngine;

public class MicrophoneInput : MonoBehaviour
{
    public float MicrophoneLoudness;

    private string _device;
    private AudioClip _clip;
    private int _sampleWindow = 1024;
    private bool _isInitialized;
    private bool _isRecorded = false;

    public bool IsInitialized => _isInitialized;
    public bool IsRecorded => _isRecorded;

    private void OnEnable()
    {
        InitMicrophone();
    }

    private void OnDisable()
    {
        Stop();
    }

    private void OnDestroy()
    {
        Stop();
    }

    private void Update()
    {
        //MicrophoneLoudness = LevelMax();
    }

    private void OnApplicationFocus(bool focus)
    {
        if (focus)
        {
            if (IsInitialized)
            {
                return;
            }

            InitMicrophone();
        }
        else
        {
            Stop();
        }
    }

    public void InitMicrophone()
    {
        if (Microphone.devices.Length == 0)
        {
            _isInitialized = false;
            return;
        }

        if (_device == null)
        {
            _device = Microphone.devices[0];
        }

        _isInitialized = true;
    }

    public void Start()
    {
        if (!IsInitialized)
        {
            InitMicrophone();
        }

        if (_device == null)
        {
            return;
        }

        _clip = Microphone.Start(_device, true, 999, 44100);
        _isRecorded = true;
    }

    public void Stop()
    {
        if (_device == null)
        {
            return;
        }

        Microphone.End(_device);
        _isInitialized = false;
        _isRecorded = false;
    }

    public float LevelMax()
    {
        float levelMax = 0;
        float[] waveData = new float[_sampleWindow];
        int micPosition = Microphone.GetPosition(null) - (_sampleWindow + 1); // null means the first microphone
        if (micPosition < 0)
        {
            return 0;
        }

        _clip.GetData(waveData, micPosition);
        // Getting a peak on the last 128 samples
        for (int i = 0; i < _sampleWindow; i++)
        {
            float wavePeak = waveData[i] * waveData[i];
            if (levelMax < wavePeak)
            {
                levelMax = wavePeak;
            }
        }

        return levelMax;
    }

    public void MicrophoneToAudioClip()
    {
        if (_device != null)
        {
            _clip = Microphone.Start(_device, true, 20, AudioSettings.outputSampleRate);
        }
    }

    public float GetLoundessFromMicrophone()
    {
        if (_device != null)
        {
            return GetLoudnessFromAudioClip(Microphone.GetPosition(_device), _clip);
        }

        return 0;
    }

    public float GetLoudnessFromAudioClip(int clipPosition, AudioClip clip)
    {
        int startPosition = clipPosition - _sampleWindow;
        if (startPosition < 0)
        {
            return 0f;
        }

        float[] waveData = new float[_sampleWindow];
        clip.GetData(waveData, startPosition);
        float totalLoudness = 0f;

        for (int i = 0; i < _sampleWindow; i++)
        {
            totalLoudness += Mathf.Abs(waveData[i]);
        }

        return totalLoudness / _sampleWindow;
    }
}