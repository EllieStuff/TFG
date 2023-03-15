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
            CloseDoorAnimCall();
    }

    void CloseDoorAnimCall()
    {
        closed = true;
        Animation anim = GetComponent<Animation>();
        anim.clip = closeDoorClip;
        anim.Play();
    }
}
