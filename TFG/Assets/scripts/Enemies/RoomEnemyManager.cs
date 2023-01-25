using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RoomEnemyManager : MonoBehaviour
{
    [SerializeField] ZoneScript linkedZone;
    [SerializeField] bool roomActive = false;

    PlayerAttack playerAttack;
    List<BaseEnemyScript> enemies = new List<BaseEnemyScript>();

    // Start is called before the first frame update
    void Awake()
    {
        playerAttack = GameObject.FindGameObjectWithTag("Player").GetComponent<PlayerAttack>();

        linkedZone.assignedRoom = this;

        InitEnemies();
    }

    private void Start()
    {
        ActivateEnemies(roomActive);
        SetPlayerData();
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
        if (!HasEnemiesRemainging()) return null;

        Transform closerEnemy = enemies[0].transform;
        float closerDist = Vector3.Distance(enemies[0].transform.position, _playerTransform.position);
        for(int i = 1; i < enemies.Count; i++)
        {
            if(enemies[i] == null)
            {
                enemies.RemoveAt(i);
                i--;
                continue;
            }

            float newDist = Vector3.Distance(enemies[i].transform.position, _playerTransform.position);
            if(newDist < closerDist)
            {
                closerEnemy = enemies[i].transform;
                closerDist = newDist;
            }
        }


        return closerEnemy;
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


    void SetPlayerData()
    {
        if (roomActive)
        {
            playerAttack.roomEnemyManager = this;
            playerAttack.target = GetCloserEnemy(playerAttack.transform);
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
