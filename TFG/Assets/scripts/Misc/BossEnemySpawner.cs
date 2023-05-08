using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BossEnemySpawner : MonoBehaviour
{
    [SerializeField] LifeSystem bossLife;
    [SerializeField] BatBossEnemy boss;

    [SerializeField] GameObject[] enemiesList;
    [SerializeField] Transform[] spawnPoints;
    [SerializeField] GameObject spawnParticles;
    [SerializeField] Transform roomEnemyListPivot;

    const float SPAWNER_TIMER = 15;
    const float DESTROY_PARTICLES_TIMER = 5;
    const int MAX_SPAWNS = 3;
    const float UP_MULTIPLIER = 0;

    float timer = SPAWNER_TIMER / 2;
    bool[] spawnedPlaces;

    bool enemiesCleared = false;
    List<LifeSystem> enemies;

    void Update()
    {
        if(!bossLife.isDead && boss.secondPhaseEntered)
        {
            timer -= Time.deltaTime;

            if(timer <= 0)
                SpawnEnemies();
        }

        if(!enemiesCleared && bossLife.isDead)
        {
            DestroyEnemies();
            enemiesCleared = true;
        }
    }

    void SpawnEnemies()
    {
        int randomIterations = Random.Range(0, MAX_SPAWNS);

        spawnedPlaces = new bool[spawnPoints.Length];

        for (int index = 0; index < randomIterations; index++)
        {
            int randomEnemy = Random.Range(0, enemiesList.Length);
            int spawnIndex = Random.Range(0, spawnPoints.Length);

            if (spawnedPlaces[spawnIndex])
                continue;
            else
                spawnedPlaces[spawnIndex] = true;

            GameObject enemy = Instantiate(enemiesList[randomEnemy], spawnPoints[spawnIndex].position, spawnPoints[spawnIndex].rotation);
            enemy.transform.parent = roomEnemyListPivot;
            roomEnemyListPivot.GetComponent<RoomEnemyManager>().UpdateRoomEnemies();
            enemy.GetComponent<BaseEnemyScript>().waiting = false;

            enemies.Add(enemy.GetComponent<LifeSystem>());

            ParticleSystem particles = Instantiate(spawnParticles, spawnPoints[spawnIndex].position + (Vector3.up * UP_MULTIPLIER), spawnParticles.transform.rotation).GetComponent<ParticleSystem>();
            particles.Play();
            Destroy(particles.gameObject, DESTROY_PARTICLES_TIMER);
        }

        timer = SPAWNER_TIMER;
    }

    void DestroyEnemies()
    {
        foreach(LifeSystem enemyLife in enemies) 
        {
            enemyLife.currLife = 0;
        }
    }
}
