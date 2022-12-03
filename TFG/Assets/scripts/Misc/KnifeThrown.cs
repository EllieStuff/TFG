using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class KnifeThrown : MonoBehaviour
{
    [SerializeField] float knifeMaxDistance;
    [SerializeField] float knifeSpeed;
    [SerializeField] float knifeDamage;
    const float timerInmunnityKnife = 0.1f;
    internal Transform entityThrowingIt;
    internal Vector3 knifeDir;

    float timerKnife;
    MeshCollider knifeMesh;

    private void Start()
    {
        knifeMesh = GetComponent<MeshCollider>();
        knifeMesh.isTrigger = true;
        timerKnife = timerInmunnityKnife;
        GetComponent<Rigidbody>().velocity = knifeDir * knifeSpeed;
        transform.parent = null;
    }

    private void Update()
    {
        if (timerKnife > 0)
            timerKnife -= Time.deltaTime;
        else if (knifeMesh.isTrigger)
            knifeMesh.isTrigger = false;

        if (entityThrowingIt != null && Vector3.Distance(transform.position, entityThrowingIt.position) >= knifeMaxDistance)
            Destroy(gameObject);
        else if(entityThrowingIt == null)
            Destroy(gameObject);
    }

    private void OnCollisionEnter(Collision collision)
    {
        GameObject col = collision.gameObject;
        if(col.tag.Equals("Player"))
        {
            LifeSystem life = col.GetComponent<LifeSystem>();
            life.Damage(knifeDamage, new HealthState());
        }

        if(timerKnife <= 0)
            Destroy(gameObject);
    }
}
