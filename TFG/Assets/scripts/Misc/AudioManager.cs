using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AudioManager : MonoBehaviour
{
    [SerializeField] AudioSource audioSource;
    [SerializeField] AudioClip[] clips;
    [SerializeField] float minPitch = 0.8f;
    [SerializeField] float maxPitch = 1.2f;
    [SerializeField] bool playOnStart;
    [SerializeField] bool playOnEnable;

    private void Start()
    {
        if (playOnStart)
            PlaySound();
    }

    private void OnEnable()
    {
        if (playOnEnable)
            PlaySound();
    }

    public bool IsPlayingSound()
    {
        return audioSource.isPlaying;
    }

    public void PlaySound()
    {
        audioSource.clip = ChooseClip(clips);
        audioSource.pitch = Random.Range(minPitch, maxPitch);
        audioSource.Play();
    }

    public static AudioClip ChooseClip(AudioClip[] clips)
    {
        return clips[(int)Random.Range(0, clips.Length)];
    }

}
