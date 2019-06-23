using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneReloader : MonoBehaviour
{

    [SerializeField] private Valve.VR.SteamVR_Fade fader;

    private void Update()
    {
        if(Input.GetKeyDown(KeyCode.Space))
        {
            Valve.VR.SteamVR_Fade.Start(Color.black, 1f);
            SceneManager.LoadSceneAsync(SceneManager.GetActiveScene().name);
        }
    }

}
