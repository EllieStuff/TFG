using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WalkMark : MonoBehaviour
{
    Camera cameraMain;

    Vector3 worldPosition = Vector3.zero;
    Plane plane = new Plane(Vector3.up, 0);
    PlayerMovement playerScript;
    [SerializeField] GameObject walkMark;
    internal bool transition;

    const float DISTANCE_TO_DISABLE_MARK = 2;

    private void Start()
    {
        walkMark.transform.parent = null;
        cameraMain = Camera.main;
        playerScript = GameObject.FindGameObjectWithTag("Player").GetComponent<PlayerMovement>();
    }

    void Update()
    {
        SetMarkPositionWithRaycast();

        if(!transition && Input.GetKey(KeyCode.Mouse1))
        {
            //CheckCollidingWall(true);

            playerScript.targetMousePos = worldPosition;

            if(walkMark.activeSelf)
                walkMark.SetActive(false);
        }

        if(Input.GetKey(KeyCode.LeftShift) && walkMark.activeSelf)
            walkMark.SetActive(false);

        if(Input.GetKeyUp(KeyCode.Mouse1))
        {
            ResetMousePos();

            //CheckCollidingWall(true);
        }

        //CheckCollidingWall(false);

        if (walkMark.activeSelf && Vector3.Distance(playerScript.transform.position, walkMark.transform.position) <= DISTANCE_TO_DISABLE_MARK)
            walkMark.SetActive(false);
    }

    public void ResetMousePos()
    {
        worldPosition = playerScript.transform.position;
        playerScript.targetMousePos = worldPosition;
        walkMark.transform.position = worldPosition;
    }

    void CheckCollidingWall(bool disableWallCheck)
    {
        if (playerScript.isCollidingWall)
        {
            ResetMousePos();

            if (disableWallCheck)
                playerScript.isCollidingWall = false;
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

    public void SetWalkMarkActive(bool _active)
    {
        walkMark.SetActive(_active);
    }

}
