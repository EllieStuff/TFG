using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Enemy_Ragloton : BaseEnemyScript
{
    [Header("Ragloton")]
    [SerializeField] Transform shieldPivotRef; 
    [SerializeField] Transform idleShieldPoint, attackingShieldPoint;
    [SerializeField] internal bool hasShield = true;
    [SerializeField] float attackForce = 10.0f, attackDuration = 1.0f;
    [SerializeField] Vector3 atkVelocityLimit = new Vector3(20, 0, 20);

    
    Vector3 attackMoveDir = Vector3.zero;


    internal override void Start_Call() { base.Start_Call(); }

    internal override void Update_Call() { base.Update_Call(); }

    internal override void FixedUpdate_Call() { base.FixedUpdate_Call(); }


    internal override void IdleUpdate() { base.IdleUpdate(); }
    internal override void MoveToTargetUpdate() { base.MoveToTargetUpdate(); }
    internal override void AttackUpdate()
    { 
        base.AttackUpdate();

        if (isAttacking)
        {
            MoveRB(attackMoveDir, attackForce);
        }
        else
        {

        }
    }


    internal override void IdleStart() { base.IdleStart(); }
    internal override void MoveToTargetStart() { base.MoveToTargetStart(); }
    internal override void AttackStart()
    { 
        base.AttackStart();
        SetVelocityLimit(-atkVelocityLimit, atkVelocityLimit);
        StartCoroutine(AttackCoroutine());
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
    
    IEnumerator AttackCoroutine()
    {
        // Prepares For Attack
        canMove = false;
        StopRB(2.0f);
        yield return new WaitForSeconds(0.1f);
        if (hasShield)
            yield return LerpTransformsPosition(shieldPivotRef, idleShieldPoint, attackingShieldPoint);
        else
            yield return new WaitForSeconds(0.3f);
        yield return new WaitForSeconds(0.1f);

        // Attacks
        canMove = isAttacking = true;
        canRotate = false;
        attackMoveDir = (player.position - transform.position).normalized;
        yield return new WaitForSeconds(attackDuration);

        // Ends Attack
        canMove = isAttacking = false;
        StopRB(4.0f);
        yield return new WaitForSeconds(1.0f);
        if(hasShield)
            yield return LerpTransformsPosition(shieldPivotRef, attackingShieldPoint, idleShieldPoint);
        else
            yield return new WaitForSeconds(0.3f);
        canMove = canRotate = true;

        if(state.Equals(States.DAMAGE))
        {
            base.newMatDef.color = Color.white;
            base.damageTimer = baseDamageTimer;
        }

        ChangeState(States.IDLE);
    }
    IEnumerator LerpTransformsPosition(Transform _affectedTrans, Transform _initPointTrans, Transform _targetPointTrans, float _lerpDuration = 0.5f)
    {
        float timer = 0, maxTime = _lerpDuration;
        while (timer < maxTime)
        {
            yield return new WaitForEndOfFrame();
            timer += Time.deltaTime;
            _affectedTrans.position = Vector3.Lerp(_initPointTrans.position, _targetPointTrans.position, timer / maxTime);
        }
        yield return new WaitForEndOfFrame();
        _affectedTrans.position = _targetPointTrans.position;
    }

}
