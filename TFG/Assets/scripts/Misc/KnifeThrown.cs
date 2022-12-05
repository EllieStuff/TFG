using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class KnifeThrown : MonoBehaviour
{
    [SerializeField] float knifeMaxDistance;
    [SerializeField] float knifeSpeed;
    const float timerInmunnityKnife = 0.1f;
    internal Transform entityThrowingIt;
    internal Vector3 knifeDir;

    float timerKnife;
    MeshCollider knifeMesh;
    internal DamageData dmgData;

    private void Start()
    {
        knifeMesh = GetComponent<MeshCollider>();
        knifeMesh.isTrigger = true;

        timerKnife = timerInmunnityKnife;
        GetComponent<Rigidbody>().velocity = knifeDir * knifeSpeed;
        transform.parent = null;
    }

    public void SetOwnerTransform(Transform _ownerTransform)
    {
        dmgData = GetComponent<DamageData>();
        dmgData.ownerTransform = _ownerTransform;
    }

    private void Update()
    {
        if (timerKnife > 0)
            timerKnife -= Time.deltaTime;
        else if (knifeMesh.isTrigger)
            knifeMesh.isTrigger = false;

        if (dmgData.ownerTransform != null && Vector3.Distance(transform.position, dmgData.ownerTransform.position) >= knifeMaxDistance)
            Destroy(gameObject);
        else if(dmgData.ownerTransform == null)
            Destroy(gameObject);
    }

    private void OnCollisionEnter(Collision collision)
    {
        GameObject col = collision.gameObject;
        //if(col.tag.Equals("Player"))
        //{
        //    LifeSystem life = col.GetComponent<LifeSystem>();
        //    life.Damage(stats.weaponDamage, new HealthState(life));
        //}

        if(timerKnife <= 0)
            Destroy(gameObject);
    }
}
