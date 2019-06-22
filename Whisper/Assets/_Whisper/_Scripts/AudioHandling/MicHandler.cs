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
    [SerializeField] private GameObject visualizerObjPrefab;
    [SerializeField] private LineRenderer waveFormRenderer;

    private int channels;
    private float maxRecordedSum = 0f;
    const int spectrumSamples = 64;
    const int waveSamples = 256;
    private float[] spectrum;
    private float[] spectrumMax;
    private float[] spectrumSum;
    private Transform[] vis;
    const float visHeight = 20f;

    private bool recording = false;



    private void Reset()
    {
        audioSource = GetComponent<AudioSource>();
    }

    private void Awake()
    {
        audioSource.clip = Microphone.Start(Microphone.devices[0], true, 1, 44100);
        audioSource.Play();
        channels = audioSource.clip.channels;
        recording = true;
        //StartRecording();

        spectrum = new float[spectrumSamples];
        spectrumMax = new float[spectrumSamples];
        spectrumSum = new float[spectrumSamples];
        vis = new Transform[spectrumSamples];
        for(int i = 0; i < spectrumSamples; ++i)
        {
            GameObject obj = Instantiate(visualizerObjPrefab);
            vis[i] = obj.transform;
            vis[i].position = new Vector3(i, 0f, 0f);
        }

        if(waveFormRenderer != null)
        {
            waveFormRenderer.positionCount = waveSamples;
        }
    }

    private void StartRecording()
    {
        if (Microphone.GetPosition(Microphone.devices[0]) > 0)
        {
            audioSource.Play();
            channels = audioSource.clip.channels;
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
        float[] samples = new float[waveSamples];//[16384];
        audioSource.GetOutputData(samples, 0);
        float sum = samples[0] + samples[samples.Length-1];
        
        for(int i = 9; i < samples.Length; ++i)
        {
            waveFormRenderer.SetPosition(i, new Vector3(i, samples[i] * 1000f, 0f));
            //sum += Mathf.Abs(samples[i]);
            //Debug.DrawLine(new Vector3(i - 1, samples[i] * 1000f, 0f), new Vector3(i, samples[i + 1] * 1000f, 0f), Color.cyan);
        }
        
        sum /= samples.Length;
        if (sum > maxRecordedSum)
            maxRecordedSum = sum;
        
        if(volumeColorer != null)
        {
            MaterialPropertyBlock block = new MaterialPropertyBlock();
            Color c = Color.Lerp(minColor, maxColor, Mathf.InverseLerp(0f, maxRecordedSum, sum));
            block.SetColor("_Color", c);
            volumeColorer.SetPropertyBlock(block);
            RenderSettings.skybox.SetColor("_Tint", c);
            waveFormRenderer.startColor = c;
            waveFormRenderer.endColor = c;
        }


        //float[] spectrum = new float[64];
        
        AudioListener.GetSpectrumData(spectrum, 0, FFTWindow.Rectangular);

        StartCoroutine(Visualize());

        //for(int i = 0; i < spectrumSamples; ++i)
        //{
        //    //Debug.Log(-Mathf.Log(spectrum[i], 2f));
        //    if (spectrum[i] > spectrumMax[i])
        //        spectrumMax[i] = spectrum[i];
        //
        //    spectrumSum[i] /= 2f;
        //    spectrumSum[i] += spectrum[i];
        //    vis[i].localScale = new Vector3(1f, Mathf.Lerp(0f, visHeight, Mathf.InverseLerp(0f, spectrumMax[i], spectrumSum[i])), 1f);
        //    //vis[i].localScale = new Vector3(1f, Mathf.Lerp(0f, visHeight, Mathf.InverseLerp(0f, spectrumMax[i], spectrum[i])), 1f);
        //    vis[i].position = new Vector3(i, vis[i].localScale.y / 2f, 0f);
        //}
        
        //for (int i = 1; i < spectrum.Length - 1; i++)
        //{
        //    Debug.DrawLine(new Vector3(i - 1, Mathf.Log(spectrum[i], 2f), 0f), new Vector3(i, Mathf.Log(spectrum[i + 1], 2f), 0f), Color.cyan);
        //    //Debug.DrawLine(new Vector3(i - 1, spectrum[i] + 10, 0), new Vector3(i, spectrum[i + 1] + 10, 0), Color.red);
        //    //Debug.DrawLine(new Vector3(i - 1, Mathf.Log(spectrum[i - 1]) + 10, 2), new Vector3(i, Mathf.Log(spectrum[i]) + 10, 2), Color.cyan);
        //    //Debug.DrawLine(new Vector3(Mathf.Log(i - 1), spectrum[i - 1] - 10, 1), new Vector3(Mathf.Log(i), spectrum[i] - 10, 1), Color.green);
        //    //Debug.DrawLine(new Vector3(Mathf.Log(i - 1), Mathf.Log(spectrum[i - 1]), 3), new Vector3(Mathf.Log(i), Mathf.Log(spectrum[i]), 3), Color.blue);
        //}
    }

    private IEnumerator Visualize()
    {
        for (int i = 0; i < spectrumSamples; ++i)
        {
            //Debug.Log(-Mathf.Log(spectrum[i], 2f));
            if (spectrum[i] > spectrumMax[i])
                spectrumMax[i] = spectrum[i];

            spectrumSum[i] /= 1.2f;
            //spectrumSum[i] += spectrum[i];
            spectrumSum[i] = spectrum[i] > spectrumSum[i] ? spectrum[i] : spectrumSum[i];
            vis[i].localScale = new Vector3(1f, Mathf.Lerp(0f, visHeight, Mathf.InverseLerp(0f, spectrumMax[i], spectrumSum[i])), 1f);
            //vis[i].localScale = new Vector3(1f, Mathf.Lerp(0f, visHeight, Mathf.InverseLerp(0f, spectrumMax[i], spectrum[i])), 1f);
            vis[i].position = new Vector3(i, vis[i].localScale.y / 2f, 0f);
        }

        yield return null;
    }

}
