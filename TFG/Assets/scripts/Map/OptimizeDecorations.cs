using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OptimizeDecorations : MonoBehaviour
{
    const int ACTIVATE_MARGIN = 1;

    GameObject[] decorationRooms;
    int lastSavedRoomId = -1;

    private void Start()
    {
        decorationRooms = new GameObject[transform.childCount];
        for(int i = 0; i < decorationRooms.Length; i++)
        {
            decorationRooms[i] = transform.GetChild(i).gameObject;
        }
    }

    private void Update()
    {
        if(lastSavedRoomId != RoomIdManager.CurrentRoomId)
        {
            lastSavedRoomId = RoomIdManager.CurrentRoomId;
            StartCoroutine(CheckRoomsActive());
        }
    }


    IEnumerator CheckRoomsActive()
    {
        Vector2Int roomActiveMargin = new Vector2Int(lastSavedRoomId - ACTIVATE_MARGIN, lastSavedRoomId + ACTIVATE_MARGIN);
        for(int i = 0; i < decorationRooms.Length; i++)
        {
            yield return null;
            if(i >= roomActiveMargin.x && i <= roomActiveMargin.y)
            {
                if (!decorationRooms[i].activeInHierarchy) 
                    decorationRooms[i].SetActive(true);
            }
            else
            {
                if (decorationRooms[i].activeInHierarchy) 
                    decorationRooms[i].gameObject.SetActive(false);
            }
        }
    }

}
