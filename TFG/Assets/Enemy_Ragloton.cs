using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Enemy_Ragloton : BaseEnemyScript
{
    [Header("Ragloton")]
    [SerializeField] Transform shieldRef;
    [SerializeField] bool hasShield = true;
    [SerializeField] internal bool isAttacking = false;
    [SerializeField] float attackForce = 5.0f;
    [SerializeField] Vector3 atkVelocityLimit = new Vector3(10, 0, 10);

    
    Vector3 attackMoveDir;


    internal override void Start_Call() { base.Start_Call(); }

    internal override void Update_Call() { base.Update_Call(); }

    internal override void FixedUpdate_Call() { base.FixedUpdate_Call(); }


    internal override void IdleUpdate() { base.IdleUpdate(); }
    internal override void MoveToTargetUpdate() { base.MoveToTargetUpdate(); }
    internal override void AttackUpdate()
    { 
        base.AttackUpdate();
        rb.AddForce(attackMoveDir * attackForce, ForceMode.Force);
    }


    internal override void IdleStart() { base.IdleStart(); }
    internal override void MoveToTargetStart() { base.MoveToTargetStart(); }
    internal override void AttackStart()
    { 
        base.AttackStart();
        SetVelocityLimit(-atkVelocityLimit, atkVelocityLimit);
        attackMoveDir = (player.position - transform.position).normalized;
        isAttacking = true;
        //ToDo:
        // - Potser fer que l'escut tingui un tag default i quan acabi canviar-lo a EnemyWeapon?
    }


    internal override void IdleExit() { base.IdleExit(); }
    internal override void MoveToTargetExit() { base.MoveToTargetExit(); }
    internal override void AttackExit()
    { 
        base.AttackExit();
        SetVelocityLimit(baseMinVelocity, baseMaxVelocity);
        isAttacking = false;
    }

    
    IEnumerator AttackCoroutine(float _delay = 2.0f)
    {
        yield return new WaitForSeconds(_delay);
        ChangeState(States.IDLE);
    }

}
