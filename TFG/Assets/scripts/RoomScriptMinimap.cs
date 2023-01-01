using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RoomScriptMinimap : MonoBehaviour
{
    enum RoomState { UNDISCOVERED, CURRENT, CLEANED, TRAP_ROOM_CLEANED }

    [SerializeField] RoomState roomState = RoomState.UNDISCOVERED;
    [SerializeField] ZoneScript roomReference;
    MeshRenderer roomMesh;

    private void Start()
    {
        roomMesh = GetComponent<MeshRenderer>();
        Material roomMat = new Material(roomMesh.material);
        roomMesh.material = roomMat;
        roomState = RoomState.UNDISCOVERED;
    }

    private void Update()
    {
        if (roomReference != null)
        {
            if(roomReference.zoneEnabled && !roomReference.zoneDefused)
                roomState = RoomState.CURRENT;
            else if (roomReference.zoneDefused)
            {
                roomState = RoomState.CLEANED;
                roomReference = null;
            }
        }

        switch (roomState)
        {
            case RoomState.UNDISCOVERED:
                roomMesh.material.color = Color.grey;
                break;
            case RoomState.CURRENT:
                roomMesh.material.color = Color.blue;
                break;
            case RoomState.CLEANED:
                roomMesh.material.color = Color.green;
                break;
        }
    }
}
