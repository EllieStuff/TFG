using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RoomEnemyManager : MonoBehaviour
{
    [SerializeField] ZoneScript linkedZone;
    [SerializeField] bool roomActive = false;

    PlayerAttack playerAttack;
    LevelInfo levelInfo;

    List<BaseEnemyScript> enemies = new List<BaseEnemyScript>();

    GameObject elementChoose;

    bool endRoomEventIsTriggered = false;

    // Start is called before the first frame update
    void Awake()
    {
        playerAttack = GameObject.FindGameObjectWithTag("Player").GetComponent<PlayerAttack>();
        levelInfo = playerAttack.GetComponent<LevelInfo>();

        linkedZone.assignedRoom = this;
        InitEnemies();
        linkedZone.enemiesQuantity = enemies.Count;
    }

    private void Start()
    {
        elementChoose = GameObject.Find("Canvas").transform.GetChild(10).gameObject;
        ActivateEnemies(roomActive);
        SetPlayerData();
    }

    private void Update()
    {
        SearchClosestTarget();
    }

    void InitEnemies()
    {
        for (int i = 0; i < transform.childCount; i++)
        {
            BaseEnemyScript enemy = transform.GetChild(i).GetComponent<BaseEnemyScript>();
            if (enemy == null)
                enemy = transform.GetChild(i).GetComponentInChildren<BaseEnemyScript>();

            if (enemy == null)
                Debug.LogWarning("BaseEnemyScript not found");
            else
            {
                enemy.zoneSystem = linkedZone;
                enemies.Add(enemy);
            }
        }
    }

    public bool HasEnemiesRemainging()
    {
        return enemies.Count > 0;
    }

    public Transform GetCloserEnemy(Transform _playerTransform)
    {
        if (!HasEnemiesRemainging())
        {
            if(!endRoomEventIsTriggered)
            {
                int level = levelInfo.level;

                if (level > 0 && level % 2 == 0)
                    elementChoose.SetActive(true);

                levelInfo.level++;
                endRoomEventIsTriggered = true;
            }

            return null;
        }

        Transform closerEnemy = GetFirstReachableEnemyWithWallCheck();
        if (closerEnemy == null) return enemies[0].transform;

        float closerDist = Vector3.Distance(closerEnemy.position, _playerTransform.position);

        for(int i = 0; i < enemies.Count; i++)
        {
            if(enemies[i] == null)
            {
                enemies.RemoveAt(i);
                i--;
                continue;
            }

            float newDist = Vector3.Distance(enemies[i].transform.position, _playerTransform.position);
            if(newDist < closerDist && PlayerCheck(enemies[i].transform))
            {
                closerEnemy = enemies[i].transform;
                closerDist = newDist;
            }
        }

        return closerEnemy;
    }

    private Transform GetFirstReachableEnemyWithWallCheck()
    {
        for (int i = 0; i < enemies.Count; i++)
        {
            Transform enemy = enemies[i].transform;

            if (PlayerCheck(enemy))
            {
                return enemy;
            }
        }

        return null;
    }

    private bool PlayerCheck(Transform _enemyTransform)
    {
        RaycastHit hit;
        float raycastDistance = Vector3.Distance(_enemyTransform.position, playerAttack.transform.position);
        Ray ray = new Ray(_enemyTransform.position, _enemyTransform.TransformDirection(Vector3.forward * raycastDistance));

        if (Physics.Raycast(ray, out hit))
        {
            if (hit.collider.tag.Equals("Player"))
                return true;
        }

        return false;
    }

    public void DiscardEnemy(BaseEnemyScript _enemy)
    {
        enemies.Remove(_enemy);
        playerAttack.target = GetCloserEnemy(playerAttack.transform);
    }

    public void ActivateRoom(bool _active)
    {
        roomActive = _active;
        ActivateEnemies(roomActive);
        SetPlayerData();
    }

    public void SearchClosestTarget()
    {
        if (roomActive)
            playerAttack.target = GetCloserEnemy(playerAttack.transform);
    }

    void SetPlayerData()
    {
        if (roomActive)
        {
            playerAttack.roomEnemyManager = this;
            SearchClosestTarget();
        }
    }

    void ActivateEnemies(bool _active)
    {
        foreach(BaseEnemyScript enemy in enemies)
        {
            enemy.gameObject.SetActive(_active);
        }
    }

}
