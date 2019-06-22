using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LerpCube : MonoBehaviour
{
    

    public GameObject pos1;
    public GameObject pos2;
    public float speed;

    private float n;
    private bool k;
    // Start is called before the first frame update
    void Start()
    {
        this.transform.position = pos1.transform.position;
        n = 0.0f;
    }

    // Update is called once per frame
    void Update()
    {
        n += speed / 10 * Time.deltaTime;

        if (k)
        {
            transform.position = Vector3.Lerp(pos1.transform.position, pos2.transform.position, n);

            if (transform.position == pos2.transform.position || n > 1.0f)
            {
                k = false;
                n = 0;
                transform.position = pos2.transform.position;
            }
        }
        else
        {
            transform.position = Vector3.Lerp(pos2.transform.position, pos1.transform.position, n);
            
            if (transform.position == pos1.transform.position || n > 1.0f)
            {
                k = true;
                n = 0;
                transform.position = pos1.transform.position;
            }
        }


        
    }
}
