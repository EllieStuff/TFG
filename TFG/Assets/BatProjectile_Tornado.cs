using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BatProjectile_Tornado : ProjectileData
{
    [SerializeField] float zigzagSpeed = 6f, zigzagFreq = 10f;
    [SerializeField] TrailRenderer trail;
    [SerializeField] ParticleSystemRenderer particles;
    //[SerializeField] bool testing = false;

    internal int zigzagDir = 1;
    float startTime;

    public override void Init(Transform _origin)
    {
        base.Init(_origin);
        affectedByObstacles = true; //"false" temporal for the build, "true" in the future for the next feature
        dmgData.attackElement = _origin.GetComponent<LifeSystem>().entityElement;
        Transform player = GameObject.FindGameObjectWithTag("Player").transform;
        if (player != null)
        {
            moveDir = (player.position - _origin.position).normalized;
        }
        else
            Destroy(gameObject);
        transform.rotation = Quaternion.LookRotation(moveDir, transform.up);
        startTime = Time.time;
    }


    protected override void Update_Call()
    {
        //base.Update_Call();
        Vector3 movementVec = moveDir * moveSpeed + Mathf.Sin((Time.time - startTime) * zigzagFreq) * transform.right * zigzagDir * zigzagSpeed;
        rb.MovePosition(transform.position + movementVec * Time.deltaTime);
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
