using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RoomEnemyManager : MonoBehaviour
{
    public bool roomActive = false;

    PlayerAttack playerAttack;
    List<BaseEnemyScript> enemies = new List<BaseEnemyScript>();

    // Start is called before the first frame update
    void Awake()
    {
        playerAttack = GameObject.FindGameObjectWithTag("Player").GetComponent<PlayerAttack>();

        InitEnemies();
    }

    // Update is called once per frame
    void Update()
    {
        //if (!roomActive) return;

        //int destroyedEnemyIdx = enemies.FindIndex(_enemy => _enemy == null);
        //if(destroyedEnemyIdx < enemies.Count && destroyedEnemyIdx >= 0)
        //{
        //    enemies.RemoveAt(destroyedEnemyIdx);
        //    playerAttack.target = GetCloserEnemy(playerAttack.transform);
        //}
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
                enemies.Add(enemy);
        }
    }

    public bool HasEnemiesRemainging()
    {
        return enemies.Count > 0;
    }

    public Transform GetCloserEnemy(Transform _trans)
    {
        if (!HasEnemiesRemainging()) return null;

        Transform closerEnemy = enemies[0].transform;
        float closerDist = Vector3.Distance(enemies[0].transform.position, _trans.position);
        for(int i = 1; i < enemies.Count; i++)
        {
            if(enemies[i] == null)
            {
                enemies.RemoveAt(i);
                i--;
                continue;
            }

            float newDist = Vector3.Distance(enemies[i].transform.position, _trans.position);
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

}
