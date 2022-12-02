using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ParticleScript : MonoBehaviour
{
    [SerializeField] float timeToDestroy;
    [SerializeField] bool randomTime;
    [SerializeField] float minTime;
    [SerializeField] float maxTime;
    float destroyTimer;

    private void Start()
    {
        if (!randomTime)
            destroyTimer = timeToDestroy;
        else
            destroyTimer = Random.Range(minTime, maxTime);
    }

    private void Update()
    {
        destroyTimer -= Time.deltaTime;
        if (destroyTimer <= 0)
            Destroy(gameObject);
    }
}
