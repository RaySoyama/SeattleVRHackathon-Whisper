using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WaveFormSpawner : MonoBehaviour
{

    [SerializeField] private GameObject waveFormPrefab;
    [SerializeField] private float minDist;
    [SerializeField] private float maxDist;
    [SerializeField] private float threshold;
    [SerializeField] private int maxSpawns;
    [SerializeField] private AudioGetter audioGetter;

    private bool ready = false;
    public int SpawnCount { get; private set; }



    private void Start()
    {
        audioGetter.OnFinishedGetting.AddListener(CheckVolume);
    }

    private void CheckVolume()
    {
        if(!ready && audioGetter.AverageVolume < threshold)
        {
            ready = true;
        }
        else if(ready && audioGetter.AverageVolume >= threshold)
        {
            ready = false;
            GameObject obj = Instantiate(waveFormPrefab);
            obj.transform.position = Random.onUnitSphere * Random.Range(minDist, maxDist);
            obj.transform.LookAt(Vector3.zero);
            ++SpawnCount;
            if (SpawnCount >= maxSpawns)
                audioGetter.OnFinishedGetting.RemoveListener(CheckVolume);
        }
    }

}
