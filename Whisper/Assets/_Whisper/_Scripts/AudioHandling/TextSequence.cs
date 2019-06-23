using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class TextSequence : MonoBehaviour
{

    [System.Serializable]
    public struct TextPart
    {
        public string text;
        public float threshold;
        public int circles;
    }

    [SerializeField] private TextPart[] texts;
    [SerializeField] private float minWait;
    [SerializeField] private float fadeLength;
    [SerializeField] private float threshold;
    [SerializeField] private TMPro.TextMeshProUGUI textUI;
    [SerializeField] private AudioGetter audioGetter;
    [ReadOnlyField] public float currentVolume;

    [HideInInspector] public UnityEvent OnThresholdPassed;


    public int idx;
    private float waitClock;
    private float fadeOutClock;
    private float fadeInClock;
    private bool ready = false;



    private void Awake()
    {
        OnThresholdPassed = new UnityEvent();
    }

    private void Start()
    {
        audioGetter.OnFinishedGetting.AddListener(TextUpdate);
        textUI.text = texts[idx].text;
    }

    private void TextUpdate()
    {
        if(fadeOutClock > 0f)
        {
            fadeOutClock -= Time.deltaTime;
            if (fadeOutClock <= 0f)
            {
                fadeOutClock = 0f;
                textUI.text = texts[idx].text;
            }
            textUI.alpha = Mathf.InverseLerp(0f, fadeLength, fadeOutClock);
        }
        else if(fadeInClock < 1f)
        {
            fadeInClock += Time.deltaTime;
            if (fadeInClock > 1f)
            {
                fadeInClock = 1f;
                if (idx == texts.Length - 1)
                    enabled = false;
            }
            textUI.alpha = Mathf.InverseLerp(0f, fadeLength, fadeInClock);
        }
        else if(waitClock > 0f)
        {
            waitClock -= Time.deltaTime;
            if (waitClock <= 0f)
                fadeInClock = 0f;
        }
        currentVolume = audioGetter.AverageVolume;
        if (!ready && waitClock <= 0f && audioGetter.AverageVolume < texts[idx].threshold)
        {
            ready = true;
        }
        else if (ready && audioGetter.AverageVolume >= texts[idx].threshold)
        {
            ready = false;
            waitClock = minWait;
            ++idx;
            fadeOutClock = fadeLength;
            OnThresholdPassed.Invoke();
        }
    }

}
