using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(LineRenderer))]
public class WaveFormDrawer : MonoBehaviour
{

    [SerializeField] private AudioGetter audioGetter;
    [SerializeField] private LineRenderer lineRenderer;
    [SerializeField] private float sampleHeightMultiplier;
    [SerializeField] private Transform startPos;
    [SerializeField] private Transform endPos;
    //[SerializeField] private Color visibleColor;
    //[SerializeField] private Color invisibleColor;
    [SerializeField] private Color lowColor;
    [SerializeField] private Color highColor;
    [SerializeField] private float highRange;
    [SerializeField] private float opacityThreshold;
    [SerializeField] private float visibleTime;

    private float[] samples;
    private float visibleClock;



    private void Reset()
    {
        lineRenderer = GetComponent<LineRenderer>();
    }

    private void Start()
    {
        audioGetter.OnFinishedGetting.AddListener(DrawLine);
        lineRenderer.positionCount = AudioGetter.waveSamples;
        //lineRenderer.startColor = invisibleColor;
        //lineRenderer.endColor = invisibleColor;
    }

    private void DrawLine()
    {
        visibleClock = Mathf.Clamp(visibleClock - Time.deltaTime, 0f, float.MaxValue);

        if (audioGetter.AverageVolume > opacityThreshold)
            visibleClock = visibleTime;
        //Debug.Log(visibleClock + " " + audioGetter.AverageVolume);
        if(visibleClock > 0f)
        {
            samples = audioGetter.samples;
            Vector3 start = startPos.position;
            Vector3 inc = (endPos.position - start).normalized * (Vector3.Distance(start, endPos.position) / AudioGetter.waveSamples);

            for (int i = 0; i < samples.Length; ++i)
            {
                Vector3 point = start + inc * i;
                point.y += samples[i] * sampleHeightMultiplier;
                lineRenderer.SetPosition(i, point);
            }

            //Color c = Color.Lerp(invisibleColor, visibleColor, Mathf.InverseLerp(0f, visibleTime, visibleClock));

            //float[] spectrum = audioGetter.spectrumSum;
            //float sum = 0f;
            //for (int i = 0; i < spectrum.Length; ++i)
            //    sum += spectrum[i];
            //
            //Color c = Color.black;
            //int highPoint = (int)(spectrum.Length * highRange);
            //for(int i = 0; i < spectrum.Length; ++i)
            //{
            //    float lerp = Mathf.InverseLerp(0f, highPoint, i);
            //    c += Color.Lerp(lowColor, highColor, lerp) * (spectrum[i] / sum);
            //    //Debug.Log(lerp + " " + Color.Lerp(lowColor, highColor, lerp) + " " + (spectrum[i] / sum) + " " + spectrum[i] + " " + sum);
            //}
            //c.a = Mathf.InverseLerp(0f, visibleTime, visibleClock);
            Color c = Color.Lerp(lowColor, highColor, Mathf.InverseLerp(0f, audioGetter.PeakAverageVolume, audioGetter.AverageVolume));
            c.a = Mathf.InverseLerp(0f, visibleTime, visibleClock);

            lineRenderer.startColor = c;
            lineRenderer.endColor = c;
        }
    }

}
