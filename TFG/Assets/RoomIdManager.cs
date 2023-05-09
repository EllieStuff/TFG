using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class RoomIdManager : MonoBehaviour
{
    const string ROOM_TEXT = "Room ";

    static int currentRoom = 0;
    static TextMeshProUGUI roomText;

    [SerializeField] bool isTutorial = false;

    public static int CurrentRoomId { get { return currentRoom; } }

    // Start is called before the first frame update
    void Start()
    {
        currentRoom = 0;
        roomText = GetComponent<TextMeshProUGUI>();

        if (isTutorial) roomText.enabled = false;
    }


    public static void NextRoom()
    {
        currentRoom++;
        roomText.text = ROOM_TEXT + currentRoom.ToString();
    }

}
