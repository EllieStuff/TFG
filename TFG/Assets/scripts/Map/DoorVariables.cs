using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DoorVariables : MonoBehaviour
{
    internal bool openedDoor;
    private bool closed;
    [SerializeField] internal AnimationClip closeDoorClip;

    private void OnTriggerExit(Collider other)
    {
        if (openedDoor && !closed && other.tag.Equals("Player"))
        {
            float outDir = transform.position.z - other.transform.position.z;
            if(outDir < 0)
                CloseDoorAnimCall();
        }
    }

    void CloseDoorAnimCall()
    {
        //soroll tancar porta
        closed = true;
        Animation anim = GetComponent<Animation>();
        anim.clip = closeDoorClip;
        anim.Play();

        //AUDIO
        AudioManager.instance.PlayOneShot(FMODEvents.instance.doorClose, this.transform.position);
    }

    internal void ChangeDoorTag()
    {
        foreach(Transform child in transform)
        {
            child.tag = "BackDoor";
        }
    }
}
