using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BatProjectile_Missile : ProjectileData
{
    const float BASE_SPEED_INC = 1f;
    const float CLOSE_RANGE_SPEED_INC = 3.5f;
    const float CLOSE_RANGE_THRESHOLD = 3f;

    [SerializeField] Vector3 maxVelocity = new Vector3(10, 0, 10);
    [SerializeField] float destroyTime = 4f;
    [SerializeField] TrailRenderer trail;
    [SerializeField] ParticleSystemRenderer particles;
    [SerializeField] LifeSystem playerLife;
    //[SerializeField] bool testing = false;

    Transform playerRef;
    Vector3 ancorePos;
    float speedInc = 1f;

    public override void Init(Transform _origin)
    {
        base.Init(_origin);
        affectedByObstacles = false;
        dmgData.attackElement = _origin.GetComponent<LifeSystem>().entityElement;
        playerRef = GameObject.FindGameObjectWithTag("Player").transform;
        playerLife = playerRef.GetComponent<LifeSystem>();
        if (playerRef != null)
        {
            moveDir = (playerRef.position - _origin.position).normalized;
            ancorePos = _origin.position;
        }
        else
            Destroy(gameObject);
        transform.rotation = Quaternion.LookRotation(moveDir, transform.up);
        DestroyObject(destroyTime);
    }


    protected override void Update_Call()
    {
        if(playerLife != null && playerLife.isDead)
            Destroy(gameObject);

        if (Vector3.Distance(transform.position, playerRef.position) > CLOSE_RANGE_THRESHOLD) 
            { ancorePos = transform.position; speedInc = BASE_SPEED_INC; }
        else 
            speedInc = CLOSE_RANGE_SPEED_INC;
        moveDir = (playerRef.position - ancorePos).normalized;
        rb.velocity += moveDir * moveSpeed * speedInc * Time.deltaTime;
        rb.velocity = ClampVector(rb.velocity, -maxVelocity, maxVelocity);
    }


    internal Vector3 ClampVector(Vector3 _originalVec, Vector3 _minVec, Vector3 _maxVec)
    {
        return new Vector3(
            Mathf.Clamp(_originalVec.x, _minVec.x, _maxVec.x),
            Mathf.Clamp(_originalVec.y, _minVec.y, _maxVec.y),
            Mathf.Clamp(_originalVec.z, _minVec.z, _maxVec.z)
        );
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
