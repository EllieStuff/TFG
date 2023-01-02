using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RoomScriptMinimap : MonoBehaviour
{
    enum RoomState { UNDISCOVERED, CURRENT, CLEANED, TRAP_ROOM_CURRENT }

    [SerializeField] RoomState roomState = RoomState.UNDISCOVERED;
    [SerializeField] Transform camTransform;
    [SerializeField] ZoneScript roomReference;
    [SerializeField] bool trapRoom;
    MeshRenderer roomMesh;

    const float LERP_SPEED = 2;

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
            if((roomReference.zoneEnabled && !roomReference.zoneDefused) || roomReference.showRoom)
            {
                roomState = RoomState.CURRENT;
                camTransform.position = Vector3.Lerp(camTransform.position, new Vector3(transform.position.x, camTransform.position.y, transform.position.z), Time.deltaTime * LERP_SPEED);
            }
            else if (roomReference.zoneDefused)
                roomState = RoomState.CLEANED;
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
