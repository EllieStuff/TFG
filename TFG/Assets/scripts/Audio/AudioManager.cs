using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using FMODUnity;
using FMOD.Studio;

public class AudioManager : MonoBehaviour
{
    private List<EventInstance> eventInstances;

    public static AudioManager instance { get; private set; }

    private EventInstance gameplayMusicInstance;

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
        StartGamePlayMusic(FMODEvents.instance.gameplayMusic);
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

    //function to start gameplay music
    private void StartGamePlayMusic(EventReference gameplayMusic)
    {
        gameplayMusicInstance = CreateInstance(gameplayMusic);
        gameplayMusicInstance.start();

    }

    //set labeled parameter on FMOD
    public void SetFMODLabeledParameter(string parameterName, string parameterValue, EventInstance parameterInstance)
    {
        parameterInstance.setParameterByNameWithLabel(parameterName, parameterValue);
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