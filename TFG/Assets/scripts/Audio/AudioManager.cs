using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using FMODUnity;
using FMOD.Studio;

public class AudioManager : MonoBehaviour
{
    private List<EventInstance> eventInstances;

    public static AudioManager instance { get; private set; }

    public EventInstance playMusicInstance;

    private EventInstance ambientSoundInstance;

    const string
        MENU_MUSIC_STR = "Menu Music";

    const float SONG_VOLUME = 0.2f;
    const float AMBIENT_VOLUME = 0.05f;
    const float SOUND_PLAY_VOLUME = 0.5f;

    private void Awake()
    {
        //secure we only have one AudioManager on the scene
        if (instance != null)
        {
            Debug.LogError("Found more than one Audio Manager in the scene.");
        }
        instance = this;

        eventInstances = new List<EventInstance>();
    }

    private void Start()
    {
        StartAmbientSound(FMODEvents.instance.ambientSound);
        PlayMusic(FMODEvents.instance.allMusic);
    }

    //function to play Action FMOD Events
    public void PlayOneShot(EventReference sound, Vector3 worldPos)
    {
        RuntimeManager.PlayOneShot(sound, worldPos);
    }

    //function to create an instance of a timeline event
    public EventInstance CreateInstance(EventReference eventReference)
    {
        EventInstance eventInstance = RuntimeManager.CreateInstance(eventReference);
        eventInstances.Add(eventInstance);
        //global volume for every new volume
        eventInstance.setVolume(SOUND_PLAY_VOLUME);
        return eventInstance;
    }

    //function to start ambient sound
    private void StartAmbientSound(EventReference ambientSound)
    {
        ambientSoundInstance = CreateInstance(ambientSound);
        //ambient volume
        ambientSoundInstance.setVolume(AMBIENT_VOLUME);
        ambientSoundInstance.start();
    }

    //function to start gameplay music
    public void PlayMusic(EventReference playMusicReference)
    {
        playMusicInstance = CreateInstance(playMusicReference);
        SetFMODMusic(SceneManager.GetActiveScene().name);
        //song volume
        playMusicInstance.setVolume(SONG_VOLUME);
        playMusicInstance.start();
    }

    //set labeled parameter on FMOD
    public void SetFMODLabeledParameter(string parameterName, string parameterValue, EventInstance parameterInstance)
    {
        parameterInstance.setParameterByNameWithLabel(parameterName, parameterValue);
    }

    public void SetFMODMusic(string sceneName)
    {
        if (sceneName == "Main Menu")
        {
            SetFMODLabeledParameter("musicScene", "menuMusic", playMusicInstance);
        }
        else
        {
            SetFMODLabeledParameter("musicScene", "gameplayMusic", playMusicInstance);
        }
    }

    //clean up the instances
    private void CleanUp()
    {
        foreach (EventInstance eventInstance in eventInstances)
        {
            eventInstance.stop(FMOD.Studio.STOP_MODE.IMMEDIATE);
            eventInstance.release();
        }
    }

    //destroy instances when scene is destroyed
    private void OnDestroy()
    {
        CleanUp();
    }
}