using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlantProjectileData : ProjectileData
{
    [SerializeField] float maxHeight = 5f;

    Vector3 targetPos;
    float initialY;
    float maxDistance;

    public override void Init(Transform _origin)
    {
        base.Init(_origin);
        dmgData.attackElement = _origin.GetComponent<LifeSystem>().entityElement;
        Transform player = GameObject.FindGameObjectWithTag("Player").transform;
        if (player != null)
        {
            initialY = transform.position.y;
            targetPos = player.position;
            maxDistance = (targetPos - _origin.position).magnitude;
            moveDir = (targetPos - _origin.position).normalized;
        }
        else
            Destroy(gameObject);
        transform.rotation = Quaternion.LookRotation(moveDir, transform.up);
    }


    protected override void Update_Call()
    {
        //Do things
        //float newY = initialY + Mathf.Sin((targetPos - transform.position).magnitude / maxDistance);
        Vector3 nextPos = new Vector3(
            transform.position.x + moveDir.x * moveSpeed * Time.deltaTime,
            initialY + Mathf.Sin((targetPos - transform.position).magnitude / maxDistance), 
            transform.position.z + moveDir.y * moveSpeed * Time.deltaTime
        );
        moveDir = (nextPos - transform.position).normalized;
        
        base.Update_Call();
    }


    protected override void OnTriggerEnter_Call(Collider other)
    {
        //Do things
        
        base.OnTriggerEnter_Call(other);
    }
}
