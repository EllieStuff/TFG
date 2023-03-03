using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraFollow : MonoBehaviour
{
    private Transform playerToFollow;
    [SerializeField] internal float camSpeed;
    [SerializeField] internal Vector3 camLimits;

    private void Start()
    {
        playerToFollow = GameObject.FindGameObjectWithTag("Player").transform;
    }

    void Update()
    {
        Vector3 posToFollow = new Vector3(playerToFollow.position.x, transform.position.y, playerToFollow.position.z);
        transform.position = Vector3.Lerp(transform.position, posToFollow, Time.deltaTime * camSpeed);
        if(transform.parent != null)
        {
            Vector3 localPos = transform.localPosition;
            transform.localPosition = new Vector3(Mathf.Clamp(localPos.x, -camLimits.x, camLimits.x), Mathf.Clamp(localPos.y, -camLimits.z, camLimits.z), localPos.z);
        }

    }
}
