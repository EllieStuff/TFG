using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AudioManager : MonoBehaviour
{
    public static AudioClip ChoseClip(AudioClip[] clips)
    {
        return clips[(int)Random.Range(0, clips.Length)];
    }

}
