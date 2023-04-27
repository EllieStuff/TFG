using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MenuCamScript : MonoBehaviour
{
    [SerializeField] Animation doorAnim;
    internal bool moveCamForward;
    const float CAM_FORWARD_SPEED = 5;
    Rigidbody camRB;

    public void EnableCamForward()
    {
        moveCamForward = true;
        doorAnim.Play();
        //doorAnim.gameObject.GetComponent<AudioSource>().Play();
    }

    private void Start()
    {
        camRB = GetComponent<Rigidbody>();
    }

    void FixedUpdate()
    {
        if (moveCamForward)
            camRB.velocity = Vector3.forward * CAM_FORWARD_SPEED;
    }
}
