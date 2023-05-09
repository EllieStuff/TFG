using System.Collections;
using System.Collections.Generic;
using UnityEngine.SceneManagement;
using UnityEngine;
using FMOD.Studio;

public class MusicManager : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        AudioManager.instance.SetFMODMusic(SceneManager.GetActiveScene().name);
        AudioManager.instance.PlayMusic(FMODEvents.instance.allMusic);
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
