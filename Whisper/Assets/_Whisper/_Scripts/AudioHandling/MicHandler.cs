using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(AudioSource))]
public class MicHandler : MonoBehaviour
{

    [SerializeField] private AudioSource audioSource;



    private void Reset()
    {
        audioSource = GetComponent<AudioSource>();
    }

    private void Awake()
    {
        Debug.Log(Microphone.devices.ToString());
        //audioSource.clip = Microphone.Start()
    }

}
