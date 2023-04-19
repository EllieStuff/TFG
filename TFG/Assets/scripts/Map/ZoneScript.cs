using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ZoneScript : MonoBehaviour
{
    internal bool zoneEnabled;
    internal bool zoneDefused;
    [SerializeField] private int roomNumber;
    [SerializeField] internal int enemiesQuantity;
    [SerializeField] Animation[] doorOpenAnims;
    [SerializeField] BoxCollider[] blockedPaths;
    [SerializeField] MeshRenderer blackTile;
    [SerializeField] Vector3 roomLimits;
    [SerializeField] Transform parentTransform;
    [SerializeField] bool notSaveRoom;
    CameraFollow camSystem;

    PlayerMovement player;

    const float LERP_SPEED = 5f;
    const float DOOR_DISTANCE = 5;

    internal RoomEnemyManager assignedRoom;
    int navArrivedIndex = 0;

    internal bool showRoom;
    bool loadedRoomSave = false;

    private void Start()
    {
        blackTile.enabled = true;
        camSystem = GameObject.Find("CameraHolder").GetComponent<CameraFollow>();
        blackTile.material = new Material(blackTile.material);
        player = GameObject.Find("Player").GetComponent<PlayerMovement>();
    }

    void Update()
    {
        if(zoneEnabled && !zoneDefused && enemiesQuantity <= 0)
        {
            UnlockPaths();
            blackTile.enabled = true;
            zoneDefused = true;
        }

        if (zoneDefused)
        {
            if (doorOpenAnims == null) Debug.LogWarning("DoorOpenAnim not set");
            else PlayAnimation(doorOpenAnims);
        }

        Color tileColor = blackTile.material.color;

        if (showRoom)
            blackTile.material.color = Color.Lerp(blackTile.material.color, new Color(tileColor.r, tileColor.g, tileColor.b, 0), Time.deltaTime * LERP_SPEED);
        else
            blackTile.material.color = Color.Lerp(blackTile.material.color, new Color(tileColor.r, tileColor.g, tileColor.b, 1), Time.deltaTime * LERP_SPEED);
    }

    void GetRoomSave()
    {
        string roomName = transform.parent.name;
        string roomNameSubstring = roomName.Substring(4);

        if (roomName.Contains("."))
            roomNumber = -1;
        else
            roomNumber = Int32.Parse(roomNameSubstring);

        int savedRoomNumber = PlayerPrefs.GetInt("RoomNumber");

        if (roomNumber == savedRoomNumber)
        {
            Transform roomEnemiesManager = GameObject.FindGameObjectWithTag("EnemyManager").transform;

            int roomIndex = 0;

            foreach (Transform child in roomEnemiesManager)
            {
                RoomEnemyManager room = child.GetComponent<RoomEnemyManager>();

                if (roomIndex == 0 && savedRoomNumber > 0)
                    room.enabled = false;

                if (roomIndex == savedRoomNumber && !room.notLoadableRoom)
                {
                    room.enabled = true;
                    room.ActivateRoom(true);
                    showRoom = true;
                    break;
                }

                if(!child.name.Contains("."))
                    roomIndex++;
            }
        }
    }

    void OnTriggerEnter(Collider other)
    {
        bool isPlayer = other.tag.Equals("Player");

        if (isPlayer && !zoneEnabled)
        {
            BlockPaths();
            zoneEnabled = true;
            StartCoroutine(assignedRoom.DisableEnemiesWait());

            if(!notSaveRoom)
                PlayerPrefs.SetFloat("DeathRoomZ", other.ClosestPoint(transform.position).z);

            if(!loadedRoomSave && !notSaveRoom)
                GetRoomSave();

            if(roomNumber >= 0)
                PlayerPrefs.SetInt("RoomNumber", roomNumber);
        }

        if(isPlayer)
        {
            camSystem.camLimits = roomLimits;
            camSystem.transform.parent = parentTransform;
            showRoom = true;
            assignedRoom.ActivateRoom(true);
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
        {
            showRoom = false;
            BlockBackBoor();
            assignedRoom.ActivateRoom(false);

        }
    }

    void PlayAnimation(Animation[] items)
    {
        for (int i = 0; i < items.Length; i++)
        {
            if (items[i] == null) continue;
            if(Vector3.Distance(items[i].transform.position, player.transform.position) <= DOOR_DISTANCE && !items[i].isPlaying && items[i].enabled && !items[i].GetComponent<DoorVariables>().openedDoor)
            {
                //play sound and particles
                //items[i].GetComponent<AudioSource>().Play();
                items[i].Play();
                DoorVariables door = items[i].GetComponent<DoorVariables>();
                door.openedDoor = true;
                door.ChangeDoorTag();
            }
        }
    }

    void BlockPaths()
    {
        for (int i = 0; i < blockedPaths.Length; i++)
            blockedPaths[i].enabled = true;
    }

    void BlockBackBoor()
    {
        for (int i = 0; i < blockedPaths.Length; i++)
            if(blockedPaths[i].tag.Equals("BackDoor"))
                blockedPaths[i].enabled = true;
    }

    void UnlockPaths()
    {
        for (int i = 0; i < blockedPaths.Length; i++)
            blockedPaths[i].enabled = false;
    }


}
