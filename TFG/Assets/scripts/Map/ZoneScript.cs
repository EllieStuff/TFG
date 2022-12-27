using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ZoneScript : MonoBehaviour
{
    bool zoneEnabled;
    bool zoneDefused;
    [SerializeField] internal int enemiesQuantity;
    [SerializeField] Animation doorOpenAnim;
    [SerializeField] BoxCollider blockedPath;
    CameraFollow camLimitSystem;

    private void Start()
    {
        camLimitSystem = GameObject.Find("CameraHolder").transform.GetChild(0).GetComponent<CameraFollow>();
    }

    void Update()
    {
        if(zoneEnabled && !zoneDefused && enemiesQuantity <= 0)
        {
            camLimitSystem.UpdateCamLimit();
            PlayAnimation(doorOpenAnim);
            blockedPath.enabled = false;
            zoneDefused = true;
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if(other.tag.Equals("Player") && !zoneEnabled)
        {
            blockedPath.enabled = true;
            zoneEnabled = true;
        }
    }


    void PlayAnimation(Animation item)
    {
        if(!item.isPlaying)
        {
            //play sound and particles
            //item.GetComponent<AudioSource>().Play();
            item.Play();
        }
    }
}
