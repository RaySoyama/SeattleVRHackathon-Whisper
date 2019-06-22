using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SkyboxColorer : MonoBehaviour
{

    [SerializeField] private AudioGetter audioGetter;
    [SerializeField] private Color lowColor;
    [SerializeField] private Color highColor;



    private void Start()
    {
        audioGetter.OnFinishedGetting.AddListener(ChangeColor);
    }

    private void ChangeColor()
    {
        RenderSettings.skybox.SetColor("_Tint", Color.Lerp(lowColor, highColor, Mathf.InverseLerp(0f, audioGetter.PeakVolume, audioGetter.SmoothedVolume)));
    }

}
