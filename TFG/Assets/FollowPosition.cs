using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FollowPosition : MonoBehaviour
{
    [SerializeField] Transform toFollow;
    [SerializeField] Vector3 margin;


    // Update is called once per frame
    void Update()
    {
        transform.position = toFollow.position + margin;
    }
}
