using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraFollow : MonoBehaviour
{
    private Transform playerToFollow;
    [SerializeField] private float camSpeed;
    [SerializeField] private float limitY;

    float limitForEveryTile;

    private void Start()
    {
        limitForEveryTile = limitY;
        playerToFollow = GameObject.FindGameObjectWithTag("Player").transform;
    }

    public void UpdateCamLimit()
    {
        limitY += limitForEveryTile * limitForEveryTile;
    }

    void Update()
    {
        Vector3 posToFollow = new Vector3(0, transform.position.y, Mathf.Clamp(playerToFollow.position.z, -limitY, limitY));
        transform.parent.position = Vector3.Lerp(transform.position, posToFollow, Time.deltaTime * camSpeed);
    }
}
