using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerProjectileData : ProjectileData
{
    const float DESTROY_TIME = 5;

    public override void Init(Transform _origin)
    {
        base.Init(_origin);
        PlayerAttack player = _origin.GetComponent<PlayerAttack>();
        dmgData.attackElement = player.currentAttackElement;
        pierceAmount = player.projectilePierceAmount;
        if (player.target != null)
            moveDir = (player.target.position - player.transform.position).normalized;
        else
            moveDir = player.transform.forward;
        transform.rotation = Quaternion.LookRotation(moveDir, transform.up);
        Destroy(gameObject, DESTROY_TIME);
    }


    protected override void Update_Call()
    {
        base.Update_Call();
        //Do things
    }


    protected override void OnTriggerEnter_Call(Collider other)
    {
        //Do things
        base.OnTriggerEnter_Call(other);
    }

}
