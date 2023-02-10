using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ParticleScript : MonoBehaviour
{
    [SerializeField] float timeToDestroy;
    [SerializeField] bool randomTime;
    [SerializeField] float minTime;
    [SerializeField] float maxTime;
    [SerializeField] bool removeParent;
    [SerializeField] bool followPlayer;
    [SerializeField] bool permanent;
    [SerializeField] bool playerParticle;
    [SerializeField] Rigidbody playerRB;

    int originalMaxParticles;
    float destroyTimer;

    ParticleSystem particles;

    private void Start()
    {
        if (removeParent)
            transform.parent = null;
        if (followPlayer)
            transform.parent = GameObject.FindGameObjectWithTag("Player").transform;

        if (!randomTime)
            destroyTimer = timeToDestroy;
        else
            destroyTimer = Random.Range(minTime, maxTime);

        if (playerParticle)
        {
            particles = GetComponent<ParticleSystem>();
            var main = particles.main;
            originalMaxParticles = main.maxParticles;
        }
    }

    private void Update()
    {
        if(!permanent)
        {
            destroyTimer -= Time.deltaTime;
            if (destroyTimer <= 0)
                Destroy(gameObject);
        }
        if(playerParticle)
        {
            var main = particles.main;
            if (playerRB.velocity.magnitude <= 1)
                main.maxParticles = originalMaxParticles / 100;
            else
                main.maxParticles = originalMaxParticles;
        }
    }
}
