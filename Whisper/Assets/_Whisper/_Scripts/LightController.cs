using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LightController : MonoBehaviour
{

    [Range(0.0f, 1.0f)]
    public float volumeValue;

    public GameObject spotlight;
    [Range(0.0f, 1.0f)]
    public GameObject skylight;

    public Vector2 spotRange;
    public Vector2 skyRange;

    private MeshRenderer _skyMat;

    // Start is called before the first frame update
    void Start()
    {
        spotlight.transform.position = new Vector3(spotlight.transform.position.x, spotRange.x, spotlight.transform.position.z);
        _skyMat = skylight.GetComponent<MeshRenderer>();
        _skyMat.material.SetColor("_Color", new Color(1, 0,0,skyRange.x));
    }

    // Update is called once per frame
    void Update()
    {
        spotlight.transform.position = new Vector3(spotlight.transform.position.x, spotRange.x + ((spotRange.y - spotRange.x) * volumeValue), spotlight.transform.position.z);
        _skyMat.material.SetColor("_Color", new Color(1, 0, 0, skyRange.x + (skyRange.y - skyRange.x) * volumeValue));




    }
}
