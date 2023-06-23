using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MeleeEnemy : BaseEnemyScript
{
    enum AnimState { IDLE, MOVING, ATTACKING, RESTING, DEAD }

    [Header("MeleeEnemy")]
    [SerializeField] float attackForce = 10.0f;
    [SerializeField] float attackDuration = 1.0f;
    [SerializeField] Vector3 atkVelocityLimit = new Vector3(20, 0, 20);
    [SerializeField] Animator enemyAnimator;

    [SerializeField] ParticleSystem restTears;
    [SerializeField] ParticleSystem fastRatVFX;
    [SerializeField] ParticleSystem objectionVFX;
    
    Vector3 attackMoveDir = Vector3.zero;


    protected override void Start_Call() { base.Start_Call(); }

    protected override void Update_Call() { base.Update_Call(); }

    protected override void FixedUpdate_Call() { base.FixedUpdate_Call(); }


    protected override void IdleUpdate() 
    {
        base.IdleUpdate(); 
    }
    protected override void MoveToTargetUpdate() 
    {
        base.MoveToTargetUpdate(); 
    }
    protected override void AttackUpdate()
    {
        base.AttackUpdate();

        if (canRotate && isAttacking && Vector3.Angle(transform.forward, attackMoveDir) < 1f) canRotate = false;
        if (isAttacking)
        {
            attackMoveDir = new Vector3(attackMoveDir.x, 0, attackMoveDir.z);
            MoveRB(attackMoveDir, attackForce);
        }

    }


    protected override void IdleStart()
    {
        base.IdleStart();
        enemyAnimator.SetInteger("state", (int)AnimState.IDLE);
    }
    protected override void RandomMovementStart()
    {
        base.RandomMovementStart();
        enemyAnimator.SetInteger("state", (int)AnimState.MOVING);
    }
    protected override void MoveToTargetStart()
    {
        base.MoveToTargetStart();
        enemyAnimator.SetInteger("state", (int)AnimState.MOVING);
    }
    protected override void AttackStart()
    {
        base.AttackStart();
        SetVelocityLimit(-atkVelocityLimit, atkVelocityLimit);
        canEnterDamageState = false;
        moveDir = Vector3.zero;
        StartCoroutine(Attack_Cor());
    }
    protected override void DeathStart()
    {
        enemyAnimator.SetInteger("state", (int)AnimState.DEAD);
        fastRatVFX.Stop();
        base.DeathStart();
    }


    protected override void IdleExit() { base.IdleExit(); }
    protected override void MoveToTargetExit() { base.MoveToTargetExit(); }
    protected override void AttackExit()
    { 
        base.AttackExit();
        SetVelocityLimit(baseMinVelocity, baseMaxVelocity);
        isAttacking = false;
    }

    protected override void RestStart()
    {
        base.RestStart();
        //Activar particulas sudor
        restTears.Play();
    }

    protected override void RestExit()
    {
        base.RestExit();
        //Desactivar particulas sudor
        restTears.Stop();
    }

    IEnumerator Attack_Cor()
    {
        // Prepares For Attack
        canMove = false;
        StopRB(2.0f);
        yield return new WaitForSeconds(0.2f);
        //Feedback
        enemyAnimator.SetInteger("state", (int)AnimState.IDLE);
        objectionVFX.Play();
        yield return new WaitForSeconds(attackChargingTime);
        //Feedback
        yield return new WaitForSeconds(0.2f);

        //AUDIO
        AudioManager.instance.PlayOneShot(FMODEvents.instance.ratAttack, this.transform.position);

        // Attacks
        //Justo aqui activar feedback viento vientoso ataque rata particulas
        fastRatVFX.Play();
        touchBodyDamageData.StopAllCoroutines();
        touchBodyDamageData.damage = AttackDamage;
        touchBodyDamageData.disabled = false;
        canMove = isAttacking = true;
        yield return null;
        touchBodyDamageData.dmgBehaviour = DamageData.DamageBehaviour.ON_ENTER;
        //canRotate = false;
        //attackMoveDir = (player.position - transform.position).normalized;
        attackMoveDir = transform.forward;
        moveDir = attackMoveDir;
        enemyAnimator.SetInteger("state", (int)AnimState.ATTACKING);
        yield return new WaitForSeconds(attackDuration);

        // Ends Attack
        fastRatVFX.Stop();
        enemyAnimator.SetInteger("state", (int)AnimState.RESTING);
        canMove = isAttacking = false;
        StopRB(stopForce);
        yield return new WaitForSeconds(0.2f);
        touchBodyDamageData.damage = dmgOnTouch;
        touchBodyDamageData.dmgBehaviour = DamageData.DamageBehaviour.ON_STAY;
        canEnterDamageState = true;
        ChangeState(States.REST);
        
        //canMove = canRotate = true;

        //if(state.Equals(States.DAMAGE))
        //{
        //    base.damageTimer = baseDamageTimer;
        //}

    }


}
