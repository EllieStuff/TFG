using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Camera : MonoBehaviour
{
    private Transform playerToFollow;
    [SerializeField] private float camSpeed;

    private void Start()
    {
        playerToFollow = GameObject.Find("Player").transform;
    }

    void Update()
    {
        Vector3 posToFollow = new Vector3(playerToFollow.position.x, playerToFollow.position.y + 9, playerToFollow.position.z - 8);
        transform.position = Vector3.Lerp(transform.position, posToFollow, Time.deltaTime * camSpeed);
    }
}
