using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ZoneScript : MonoBehaviour
{
    internal bool zoneEnabled;
    bool zoneDefused;
    [SerializeField] internal int enemiesQuantity;
    [SerializeField] Animation[] doorOpenAnims;
    [SerializeField] BoxCollider[] blockedPaths;
    [SerializeField] MeshRenderer blackTile;
    [SerializeField] Vector3 roomLimits;
    [SerializeField] Transform parentTransform;
    CameraFollow camSystem;

    PlayerMovement player;

    const float LERP_SPEED = 5f;

    int navArrivedIndex = 0;

    bool showRoom;

    private void Start()
    {
        camSystem = GameObject.Find("CameraHolder").transform.GetChild(0).GetComponent<CameraFollow>();
        blackTile.material = new Material(blackTile.material);
        player = GameObject.Find("Player").GetComponent<PlayerMovement>();
    }

    void Update()
    {
        if(zoneEnabled && !zoneDefused && enemiesQuantity <= 0)
        {
            PlayAnimation(doorOpenAnims);
            UnlockPaths();
            blackTile.enabled = true;
            zoneDefused = true;
        }

        Color tileColor = blackTile.material.color;

        if (showRoom)
            blackTile.material.color = Color.Lerp(blackTile.material.color, new Color(tileColor.r, tileColor.g, tileColor.b, 0), Time.deltaTime * LERP_SPEED);
        else
            blackTile.material.color = Color.Lerp(blackTile.material.color, new Color(tileColor.r, tileColor.g, tileColor.b, 1), Time.deltaTime * LERP_SPEED);
    }

    private void OnTriggerEnter(Collider other)
    {
        bool isPlayer = other.tag.Equals("Player");

        if (isPlayer && !zoneEnabled)
        {
            BlockPaths();
            zoneEnabled = true;
        }

        if(isPlayer)
        {
            camSystem.camLimits = roomLimits;
            camSystem.transform.parent.parent = parentTransform;
            showRoom = true;
        }
    }

    void OnTriggerStay(Collider other)
    {
        if(other.tag.Equals("Player") && !camSystem.transform.parent.parent.Equals(parentTransform))
        {
            camSystem.camLimits = roomLimits;
            camSystem.transform.parent.parent = parentTransform;
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.tag.Equals("Player") && zoneDefused)
            showRoom = false;
    }

    void PlayAnimation(Animation[] items)
    {
        for (int i = 0; i < items.Length; i++)
        {
            if(!items[i].isPlaying && items[i].enabled)
            {
                //play sound and particles
                items[i].GetComponent<AudioSource>().Play();
                items[i].Play();
            }
        }
    }

    void BlockPaths()
    {
        for (int i = 0; i < blockedPaths.Length; i++)
            blockedPaths[i].enabled = true;
    }

    void UnlockPaths()
    {
        for (int i = 0; i < blockedPaths.Length; i++)
            blockedPaths[i].enabled = false;
    }
}
