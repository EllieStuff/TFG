using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using FMODUnity;
using FMOD.Studio;

public class AudioManager : MonoBehaviour
{

    private Bus musicBus, sfxBus;

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

        musicBus = RuntimeManager.GetBus("bus:/music");
        sfxBus = RuntimeManager.GetBus("bus:/sfx");
    }

    private void Start()
    {
        StartAmbientSound(FMODEvents.instance.ambientSound);
        PlayMusic(FMODEvents.instance.allMusic);
    }

    public void SetMusicVolume(float _musicVolume)
    {
        musicBus.setVolume(_musicVolume);
    }

    public void SetSfxVolume(float _sfxVolume)
    {
        sfxBus.setVolume(_sfxVolume);
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
        return eventInstance;
    }

    //function to start ambient sound
    private void StartAmbientSound(EventReference ambientSound)
    {
        ambientSoundInstance = CreateInstance(ambientSound);
        ambientSoundInstance.start();
    }

    //function to start gameplay music
    public void PlayMusic(EventReference playMusicReference)
    {
        playMusicInstance = CreateInstance(playMusicReference);
        SetFMODMusic(SceneManager.GetActiveScene().name);
        playMusicInstance.start();
    }

    public void StopMusic(EventInstance playMusicInstance)
    {
        playMusicInstance.stop(FMOD.Studio.STOP_MODE.ALLOWFADEOUT);
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