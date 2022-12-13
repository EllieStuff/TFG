using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ExplosionAttack : MonoBehaviour
{
    [SerializeField] float dmgDuration = 2f, disappearDelay = 4;

    float timer = 0f;
    Collider col;


    private void Start()
    {
        col = GetComponent<Collider>();
        Destroy(gameObject, disappearDelay);
    }

    // Update is called once per frame
    void Update()
    {
        if (!col.enabled) return;

        timer += Time.deltaTime;
        if(timer >= dmgDuration) col.enabled = false;
    }
}
