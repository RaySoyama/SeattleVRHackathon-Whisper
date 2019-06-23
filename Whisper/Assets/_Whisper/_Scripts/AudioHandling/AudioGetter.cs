using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class AudioGetter : MonoBehaviour
{

    public UnityEvent OnFinishedGetting;

    [SerializeField] private AudioSource audioSource;
    [SerializeField] private float spectrumCooldown;
    [SerializeField] private int volumeBufferSize;
    [SerializeField] private float peakCooldown;
    
    const int spectrumSamples = 64;
    public float[] spectrum { get; private set; }
    public float[] spectrumMax { get; private set; }
    public float[] spectrumSum { get; private set; }

    public const int waveSamples = 256;
    public float[] samples { get; private set; }
    private Queue<float> volumeBuffer;
    public float AverageVolume { get; private set; }
    public float PeakAverageVolume { get; private set; }
    public float PeakVolume { get; private set; }
    public float SmoothedVolume { get; private set; }

    private bool recording = false;

    

    private void Awake()
    {
        audioSource.clip = Microphone.Start(Microphone.devices[0], true, 1, 44100);
        audioSource.Play();
        recording = true;
        //StartRecording();

        spectrum = new float[spectrumSamples];
        spectrumMax = new float[spectrumSamples];
        spectrumSum = new float[spectrumSamples];

        volumeBuffer = new Queue<float>(volumeBufferSize);
        samples = new float[waveSamples];

        OnFinishedGetting = new UnityEvent();
    }

    private void StartRecording()
    {
        if (Microphone.GetPosition(Microphone.devices[0]) > 0)
        {
            audioSource.Play();
            recording = true;
        }
        else
        {
            Invoke("StartRecording", 0.1f);
        }
    }

    private void Update()
    {
        float dt = Time.deltaTime;

        AudioListener.GetSpectrumData(spectrum, 0, FFTWindow.Rectangular);

        for (int i = 0; i < spectrumSamples; ++i)
        {
            if (spectrum[i] > spectrumMax[i])
                spectrumMax[i] = spectrum[i];

            spectrumSum[i] /= 1.2f;
            spectrumSum[i] = spectrum[i] > spectrumSum[i] ? spectrum[i] : spectrumSum[i];
        }

        audioSource.GetOutputData(samples, 0);
        float sum = 0f;
        for (int i = 0; i < samples.Length; ++i)
            sum += Mathf.Abs(samples[i]);
        sum /= samples.Length;

        volumeBuffer.Enqueue(sum);
        if (volumeBuffer.Count > volumeBufferSize)
            volumeBuffer.Dequeue();

        float[] buf = volumeBuffer.ToArray();
        AverageVolume = 0f;
        for (int i = 0; i < buf.Length; ++i)
            AverageVolume += buf[i];
        AverageVolume /= buf.Length;

        if (AverageVolume > PeakAverageVolume)
            PeakAverageVolume = AverageVolume;

        SmoothedVolume /= peakCooldown;
        if (AverageVolume > PeakVolume)
            PeakVolume = AverageVolume;
        if (AverageVolume > SmoothedVolume)
            SmoothedVolume = AverageVolume;

        //Debug.Log(AverageVolume + " " + PeakAverageVolume + " " + volumeBuffer.Count + " " + volumeBuffer.Peek());

        OnFinishedGetting.Invoke();
    }

}
