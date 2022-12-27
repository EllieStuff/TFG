using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ZoneScript : MonoBehaviour
{
    internal bool zoneEnabled;
    bool zoneDefused;
    [SerializeField] internal int enemiesQuantity;
    [SerializeField] Animation doorOpenAnim;
    [SerializeField] BoxCollider blockedPath;
    [SerializeField] MeshRenderer blackTile;
    [SerializeField] Transform[] endPivots;
    CameraFollow camLimitSystem;

    PlayerMovement player;
    CameraFollow camScript;
    WalkMark walkMarkScript;

    float old_cam_speed;
    const float MAX_TILE_DISTANCE = 20;

    int navArrivedIndex = 0;

    private void Start()
    {
        camLimitSystem = GameObject.Find("CameraHolder").transform.GetChild(0).GetComponent<CameraFollow>();
        blackTile.material = new Material(blackTile.material);
        camScript = GameObject.Find("CameraHolder").transform.GetChild(0).GetComponent<CameraFollow>();
        old_cam_speed = camScript.camSpeed;
        player = GameObject.Find("Player").GetComponent<PlayerMovement>();
        walkMarkScript = GameObject.Find("UI_Walk").GetComponent<WalkMark>();
    }

    void Update()
    {
        if(zoneEnabled && !zoneDefused && enemiesQuantity <= 0)
        {
            camLimitSystem.UpdateCamLimit();
            PlayAnimation(doorOpenAnim);
            blackTile.enabled = true;
            walkMarkScript.transition = true;
            zoneDefused = true;
        }

        Color tileColor = blackTile.material.color;

        if (zoneDefused)
        {
            float distance = Vector3.Distance(endPivots[navArrivedIndex].position, player.transform.position);
            float distance2 = (Mathf.Clamp((endPivots[1].position.z - player.transform.position.z), 0, MAX_TILE_DISTANCE) / MAX_TILE_DISTANCE);

            blackTile.material.color = new Color(tileColor.r, tileColor.g, tileColor.b, 1 - distance2);

            camScript.camSpeed = 0.5f;

            player.targetMousePos = new Vector3(endPivots[navArrivedIndex].position.x, player.transform.position.y, endPivots[navArrivedIndex].position.z);

            if (navArrivedIndex == 1 && distance <= 2)
            {
                walkMarkScript.transition = false;
                camScript.camSpeed = old_cam_speed;
                Destroy(this);
            }
            else if (navArrivedIndex == 0 && distance <= 2)
                navArrivedIndex++;

        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if(other.tag.Equals("Player") && !zoneEnabled)
        {
            blockedPath.enabled = true;
            zoneEnabled = true;
        }
    }


    void PlayAnimation(Animation item)
    {
        if(!item.isPlaying)
        {
            //play sound and particles
            item.GetComponent<AudioSource>().Play();
            item.Play();
        }
    }
}
