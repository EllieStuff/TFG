using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BatProjectile : ProjectileData
{
    const float DISTANCE_SPEED_RELATION = 0.1f;

    [SerializeField] float maxHeight = 5f;
    [SerializeField] TrailRenderer trail;
    [SerializeField] ParticleSystemRenderer particles;
    //[SerializeField] bool testing = false;


    public override void Init(Transform _origin)
    {
        base.Init(_origin);
        affectedByObstacles = false;
        dmgData.attackElement = _origin.GetComponent<LifeSystem>().entityElement;
        Transform player = GameObject.FindGameObjectWithTag("Player").transform;
        if (player != null)
        {
            moveDir = (player.position - _origin.position).normalized;
        }
        else
            Destroy(gameObject);
        transform.rotation = Quaternion.LookRotation(moveDir, transform.up);
    }


    protected override void Update_Call()
    {
        base.Update_Call();


    }



    public override void DestroyObject(float _timer = -1)
    {
        base.DestroyObject(_timer);
    }


    protected override void OnTriggerEnter_Call(Collider other)
    {
        base.OnTriggerEnter_Call(other);
    }
}
