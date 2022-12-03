using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WalkMark : MonoBehaviour
{
    Camera cameraMain;

    Vector3 worldPosition = Vector3.zero;
    Plane plane = new Plane(Vector3.up, 0);
    PlayerMovement playerScript;

    private void Start()
    {
        cameraMain = Camera.main;
        playerScript = GameObject.Find("Player").GetComponent<PlayerMovement>();
    }

    // Update is called once per frame
    void Update()
    {
        SetMarkPositionWithRaycast();

        if(Input.GetKey(KeyCode.Mouse1))
        {
            playerScript.targetMousePos = worldPosition;
        }
    }

    void SetMarkPositionWithRaycast()
    {
        float distance;
        Ray ray = cameraMain.ScreenPointToRay(Input.mousePosition);
        if (plane.Raycast(ray, out distance))
            worldPosition = ray.GetPoint(distance);

        transform.position = worldPosition;
    }
}
