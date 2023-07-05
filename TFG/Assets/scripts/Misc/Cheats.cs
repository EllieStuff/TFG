using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class Cheats : MonoBehaviour
{
    LifeSystem playerLife;
    bool infiniteLife = false;
    bool f7;
    bool f8;
    [SerializeField] GameObject cardSelectCheat;
    [SerializeField] Transform preBossLocation;

    [SerializeField] Transform enemyRoomsParent;

    private WalkMark walkmark;

    private void Start()
    {
        playerLife = GameObject.FindGameObjectWithTag("Player").GetComponent<LifeSystem>();
        walkmark = GameObject.Find("UI_Walk").GetComponent<WalkMark>();
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.F2))
            MoneyManager.AddMoney(100000);

        if (Input.GetKeyDown(KeyCode.F3))
            infiniteLife = !infiniteLife;

        if (Input.GetKeyDown(KeyCode.F4))
            cardSelectCheat.SetActive(true);


        if (Input.GetKeyDown(KeyCode.F5))
            playerLife.Damage(10000, new ElementsManager.Elements());

        if (Input.GetKeyDown(KeyCode.F6))
            CustomSceneManager.Instance.ChangeScene(SceneManager.GetActiveScene().name);

        if (infiniteLife)
        {
            playerLife.CurrLife = playerLife.MaxLife;
        }

        if (!f7 && Input.GetKeyDown(KeyCode.F7))
        {
            playerLife.transform.position = new Vector3(preBossLocation.transform.position.x, playerLife.transform.position.y, preBossLocation.transform.position.z);
            walkmark.ResetMousePos();
            ResetAllRooms();
            RoomIdManager.SetRoomIndex(21);
            f7 = true;
        }

        if(!f8 && Input.GetKeyDown(KeyCode.F8))
        {
            playerLife.transform.position = new Vector3(0, playerLife.transform.position.y, 261.5f);
            walkmark.ResetMousePos();
            ResetAllRooms();
            RoomIdManager.SetRoomIndex(12);
            f8 = true;
        }
    }

    void ResetAllRooms()
    {
        foreach(Transform room in enemyRoomsParent)
        {
            RoomEnemyManager roomScript = room.GetComponent<RoomEnemyManager>();

            if (roomScript != null)
                roomScript.DisableRoom();
        }
    }

}
