using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraFollow : MonoBehaviour
{
    private Transform playerToFollow;
    [SerializeField] internal float camSpeed;
    [SerializeField] private float limitY;

    float limitForEveryTile;

    const float MARGIN_PLUS_BACK = 6;

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
        Vector3 posToFollow = new Vector3(0, transform.parent.position.y, Mathf.Clamp(playerToFollow.position.z, limitY - (limitForEveryTile + MARGIN_PLUS_BACK), limitY));
        transform.parent.position = Vector3.Lerp(transform.parent.position, posToFollow, Time.deltaTime * camSpeed);
    }
}
