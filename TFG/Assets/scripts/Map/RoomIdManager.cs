using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class RoomIdManager : MonoBehaviour
{
    public const string SAVED_ROOM_ID_PATH = "SavedRoomId";
    const int DEFAULT_SAVED_ROOM_ID_VALUE = -1;
    const string ROOM_TEXT = "Room ";

    static int currentRoom = 0;
    static TextMeshProUGUI roomText;

    [SerializeField] bool isTutorial = false;

    public static int CurrentRoomId { get { return currentRoom; } }

    // Start is called before the first frame update
    void Start()
    {
        int savedRoomId = PlayerPrefs.GetInt(SAVED_ROOM_ID_PATH, DEFAULT_SAVED_ROOM_ID_VALUE);
        if (savedRoomId > 0)
        {
            currentRoom = PlayerPrefs.GetInt(SAVED_ROOM_ID_PATH, 0);
            PlayerPrefs.SetInt(SAVED_ROOM_ID_PATH, DEFAULT_SAVED_ROOM_ID_VALUE);
        }
        else
        {
            currentRoom = 0;
        }
        roomText = GetComponent<TextMeshProUGUI>();
        roomText.text = ROOM_TEXT + currentRoom.ToString();

        if (isTutorial) roomText.enabled = false;
    }


    public static void NextRoom()
    {
        currentRoom++;
        roomText.text = ROOM_TEXT + currentRoom.ToString();
    }

    public static void SetRoomIndex(int _index)
    {
        currentRoom = _index;
        roomText.text = ROOM_TEXT + currentRoom.ToString();
    }

}
