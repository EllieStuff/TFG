using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RespawnEnemy : MonoBehaviour
{
    [SerializeField] Transform enemyRef;
    [SerializeField] GameObject enemyPrefab;
    [SerializeField] float respawnDist = 40;

    Transform playerRef;
    Vector3 initialPosition;
    Vector3 initialScale;
    Quaternion initialRotation;

    // Start is called before the first frame update
    void Start()
    {
        playerRef = GameObject.FindGameObjectWithTag("Player").transform;
        enemyRef = transform.GetChild(0);
        initialPosition = enemyRef.position;
        initialScale = enemyRef.localScale;
        initialRotation = enemyRef.rotation;
    }

    // Update is called once per frame
    void Update()
    {
        if (enemyRef == null && Vector3.Distance(playerRef.position, initialPosition) > respawnDist)
        {
            enemyRef = Instantiate(enemyPrefab, transform).transform;
            enemyRef.position = initialPosition;
            enemyRef.localScale = initialScale;
            enemyRef.rotation = initialRotation;
        }
    }
}
