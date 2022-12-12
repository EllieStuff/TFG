using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MusicJukebox : MonoBehaviour
{
    [SerializeField] float maxVolume;
    [SerializeField] AudioSource bgMusic;
    [SerializeField] AudioSource battleMusic;
    [SerializeField] float timerExitBattle;

    const float VOLUME_CHANGE_SPEED = 0.8f;

    float timer;
    internal bool battleMode;

    void Update()
    {
        if(battleMode)
        {
            bgMusic.volume = Mathf.Lerp(bgMusic.volume, 0, Time.deltaTime * VOLUME_CHANGE_SPEED);
            battleMusic.volume = Mathf.Lerp(battleMusic.volume, maxVolume, Time.deltaTime * VOLUME_CHANGE_SPEED);
            if (timer > 0)
                timer -= Time.deltaTime;
            else
                battleMode = false;
        }  
        else
        {
            bgMusic.volume = Mathf.Lerp(bgMusic.volume, maxVolume, Time.deltaTime * VOLUME_CHANGE_SPEED);
            battleMusic.volume = Mathf.Lerp(battleMusic.volume, 0, Time.deltaTime * VOLUME_CHANGE_SPEED);
        }
    }

    public void EnableBattleMode()
    {
        timer = timerExitBattle;
        battleMode = true;
    }
}
