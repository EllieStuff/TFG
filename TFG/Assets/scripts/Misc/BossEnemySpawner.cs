using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BossEnemySpawner : MonoBehaviour
{
    [SerializeField] LifeSystem bossLife;
    [SerializeField] BatBossEnemy boss;

    [SerializeField] GameObject[] enemiesList;
    [SerializeField] Transform[] spawnPoints;

    const float SPAWNER_TIMER = 15;
    const int MAX_SPAWNS = 3;

    float timer = SPAWNER_TIMER;

    void Update()
    {
        if(!bossLife.isDead && boss.secondPhaseEntered)
        {
            if(timer <= 0)
                SpawnEnemies();
        }
    }

    void SpawnEnemies()
    {
        int randomIterations = Random.Range(0, MAX_SPAWNS);

        for (int index = 0; index < randomIterations; index++)
        {
            int randomEnemy = Random.Range(0, enemiesList.Length);
            int spawnIndex = Random.Range(0, spawnPoints.Length);
            GameObject enemy = Instantiate(enemiesList[randomEnemy], spawnPoints[spawnIndex].parent);
        }

        timer = SPAWNER_TIMER;
    }
}
