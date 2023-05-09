using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class VolumeManager : MonoBehaviour
{
    const string MUSIC_VOLUME_PATH = "MusicVolume", SFX_VOLUME_PATH = "SfxVolume";

    [SerializeField] Slider musicSlider, sfxSlider;

    AudioManager audioManager;

    private void Start()
    {
        audioManager = FindObjectOfType<AudioManager>();

        musicSlider.value = PlayerPrefs.GetFloat(MUSIC_VOLUME_PATH, 1f);
        audioManager.SetMusicVolume(musicSlider.value);
        
        sfxSlider.value = PlayerPrefs.GetFloat(SFX_VOLUME_PATH, 1f);
        audioManager.SetSfxVolume(sfxSlider.value);
    }

    public void SetMusicVolume()
    {
        audioManager.SetMusicVolume(musicSlider.value);
        PlayerPrefs.SetFloat(MUSIC_VOLUME_PATH, musicSlider.value);
    }

    public void SetSfxVolume()
    {
        audioManager.SetSfxVolume(sfxSlider.value);
        PlayerPrefs.SetFloat(SFX_VOLUME_PATH, sfxSlider.value);
    }

}
