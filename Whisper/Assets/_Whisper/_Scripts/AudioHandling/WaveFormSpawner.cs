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
    [SerializeField] private TextSequence textSequence;

    private bool ready = false;
    public int SpawnCount { get; private set; }



    private void Start()
    {
        //audioGetter.OnFinishedGetting.AddListener(CheckVolume);
        textSequence.OnThresholdPassed.AddListener(Spawn);
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
            Spawn();
        }
    }

    private void Spawn()
    {
        GameObject obj = Instantiate(waveFormPrefab);
        Vector3 pos = Random.onUnitSphere * Random.Range(minDist, maxDist);
        pos.y = Mathf.Abs(pos.y);
        obj.transform.position = pos;
        obj.transform.LookAt(Vector3.zero);
        ++SpawnCount;
        if (SpawnCount >= maxSpawns)
            audioGetter.OnFinishedGetting.RemoveListener(CheckVolume);
    }

}
