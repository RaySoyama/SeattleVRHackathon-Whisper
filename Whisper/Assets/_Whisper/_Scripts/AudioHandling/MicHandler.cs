using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(AudioSource))]
public class MicHandler : MonoBehaviour
{

    [SerializeField] private AudioSource audioSource;
    [SerializeField] private MeshRenderer volumeColorer;
    [SerializeField] private Color minColor;
    [SerializeField] private Color maxColor;
    [SerializeField] private float maxVal;

    private int channels;



    private void Reset()
    {
        audioSource = GetComponent<AudioSource>();
    }

    private void Awake()
    {
        audioSource.clip = Microphone.Start(Microphone.devices[0], true, 1, 44100);
        audioSource.Play();
        channels = audioSource.clip.channels;
    }

    private void Update()
    {
        float dt = Time.deltaTime;
        float[] samples = new float[256];//[16384];
        audioSource.GetOutputData(samples, 0);
        float sum = samples[0] + samples[samples.Length-1];
        
        for(int i = 1; i < samples.Length - 1; ++i)
        {
            sum += Mathf.Abs(samples[i]);
            Debug.DrawLine(new Vector3(i - 1, samples[i] * 1000f, 0f), new Vector3(i, samples[i + 1] * 1000f, 0f), Color.cyan);
        }

        sum /= samples.Length;
        Debug.Log(sum);
        if(volumeColorer != null)
        {
            MaterialPropertyBlock block = new MaterialPropertyBlock();
            block.SetColor("_Color", Color.Lerp(minColor, maxColor, Mathf.InverseLerp(0f, maxVal, sum)));
            Debug.Log(Color.Lerp(minColor, maxColor, Mathf.InverseLerp(0f, maxVal, sum)));
            volumeColorer.SetPropertyBlock(block);
        }


        float[] spectrum = new float[256];
        
        AudioListener.GetSpectrumData(spectrum, 0, FFTWindow.Rectangular);
        
        for (int i = 1; i < spectrum.Length - 1; i++)
        {
            Debug.DrawLine(new Vector3(i - 1, spectrum[i] * 1000f, 0f), new Vector3(i, spectrum[i + 1] * 1000f, 0f), Color.cyan);
            //Debug.DrawLine(new Vector3(i - 1, spectrum[i] + 10, 0), new Vector3(i, spectrum[i + 1] + 10, 0), Color.red);
            //Debug.DrawLine(new Vector3(i - 1, Mathf.Log(spectrum[i - 1]) + 10, 2), new Vector3(i, Mathf.Log(spectrum[i]) + 10, 2), Color.cyan);
            //Debug.DrawLine(new Vector3(Mathf.Log(i - 1), spectrum[i - 1] - 10, 1), new Vector3(Mathf.Log(i), spectrum[i] - 10, 1), Color.green);
            //Debug.DrawLine(new Vector3(Mathf.Log(i - 1), Mathf.Log(spectrum[i - 1]), 3), new Vector3(Mathf.Log(i), Mathf.Log(spectrum[i]), 3), Color.blue);
        }
    }

}
