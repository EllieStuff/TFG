using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraFollow : MonoBehaviour
{
    private Transform playerToFollow;
    [SerializeField] private float camSpeed;
    [SerializeField] Vector3 camMargin = new Vector3(0, 9, -8);

    private void Start()
    {
        playerToFollow = GameObject.FindGameObjectWithTag("Player").transform;
    }

    void Update()
    {
        Vector3 posToFollow = new Vector3(playerToFollow.position.x + camMargin.x, playerToFollow.position.y + camMargin.y, playerToFollow.position.z + camMargin.z);
        transform.position = Vector3.Lerp(transform.position, posToFollow, Time.deltaTime * camSpeed);
    }
}
