using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using FMODUnity;
using FMOD.Studio;

public class PlayMusicEvent : MonoBehaviour
{
    public EventReference cinem_music;
    public EventInstance playMusicInstance;

    // Start is called before the first frame update
    void Start()
    {
        PlayMusic(cinem_music);
    }

    //function to start gameplay music
    public void PlayMusic(EventReference playMusicReference)
    {
        playMusicInstance = CreateInstance(playMusicReference);
        playMusicInstance.start();
    }

    public EventInstance CreateInstance(EventReference eventReference)
    {
        EventInstance eventInstance = RuntimeManager.CreateInstance(eventReference);
        return eventInstance;
    }
}
