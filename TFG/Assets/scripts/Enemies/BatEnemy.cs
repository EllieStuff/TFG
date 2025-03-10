using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using FMOD.Studio;

public class BatEnemy : BaseEnemyScript
{
    protected enum AnimState { IDLE, ATTACKING, DEAD }

    const float RESET_ANIM_TIMER = 3;
    
    [Header("Bat Enemy")]
    [SerializeField] protected float attackAnimationTime;
    [SerializeField] protected Animator enemyAnimator;
    [SerializeField] protected GameObject projectilePrefab;
    [SerializeField] protected Transform shootPoint;
    [SerializeField] protected int numOfAttacks = 1;
    [SerializeField] protected float attackSeparationTime = 0.5f;

    protected float attackTimer;

    private bool blockAnim = false;

    AnimState currentAnim = AnimState.IDLE;


    //AUDIO
    private EventInstance batAttack;

    //function to play bat attack sound
    private void BatAttackSound()
    {
        //if (currentAnim == AnimState.ATTACKING)
        //{
            PLAYBACK_STATE playbackState;
            batAttack.getPlaybackState(out playbackState);
            if (playbackState.Equals(PLAYBACK_STATE.STOPPED))
            {
                //falta añadir el timer del proyectil
                batAttack.start();
            }
        //}
        //else
        //{
        //    batAttack.stop(STOP_MODE.ALLOWFADEOUT);
        //}
    }

    protected override void Start_Call()
    {
        base.Start_Call();

        //AUDIO
        batAttack = AudioManager.instance.CreateInstance(FMODEvents.instance.batAttack);
    }

    protected override void Update_Call() { base.Update_Call(); }

    protected override void FixedUpdate_Call() 
    { 
        base.FixedUpdate_Call();
    }

    protected void ChangeAnim(AnimState _state)
    {
        if (!blockAnim || _state.Equals(AnimState.DEAD))
        {
            enemyAnimator.SetInteger("state", (int) _state);
            currentAnim = _state;
        }
    }

    protected override void IdleUpdate()
    {
        ChangeAnim(AnimState.IDLE);

        base.IdleUpdate();
    }
    protected override void MoveToTargetUpdate()
    {
        ChangeAnim(AnimState.IDLE);

        base.MoveToTargetUpdate();
    }
    //protected override void DamageUpdate()
    //{
    //    enemyAnimator.SetFloat("state", 2);
    //    base.DamageUpdate();
    //}
    protected override void AttackUpdate()
    {
        base.AttackUpdate();

        RaycastHit hit;
        bool hitCollided = Physics.Raycast(transform.position, (playerRef.position - transform.position).normalized, out hit, Vector3.Distance(transform.position, playerRef.position), layerMask);
        if (!hitCollided || !hit.transform.CompareTag("Player"))
        {
            ChangeState(States.IDLE);

            return;
        }

        attackTimer -= Time.deltaTime;
        if (attackTimer <= 0)
        {
            StartCoroutine(ResetAnimCor());
            StartCoroutine(Attack_Cor());
            attackTimer = AttackWait + attackChargingTime;
        }

    }

    protected override void DeathStart()
    {
        base.DeathStart();
        ChangeAnim(AnimState.DEAD);
        Destroy(gameObject, baseDeathTime);
    }

    protected override void DeathUpdate()
    {
        ChangeAnim(AnimState.DEAD);
        base.DeathUpdate();
    }

    private IEnumerator ResetAnimCor()
    {
        yield return new WaitForSeconds(RESET_ANIM_TIMER);

        if (!currentAnim.Equals(AnimState.DEAD))
        {
            blockAnim = false;
            ChangeAnim(AnimState.IDLE);
        }
    }

    protected virtual IEnumerator Attack_Cor()
    {
        //place shoot animation here
        ChangeAnim(AnimState.ATTACKING);
        enemyAnimator.speed = 1.5f;
        blockAnim = true;
        //yield return new WaitForSeconds(attackChargingTime);
        yield return new WaitForSeconds(attackAnimationTime);

        //AUDIO
        BatAttackSound();

        canRotate = false;
        for (int i = 0; i < numOfAttacks; i++)
        {
            BatProjectile_Tornado projectile = Instantiate(projectilePrefab, shootPoint).GetComponent<BatProjectile_Tornado>();
            projectile.zigzagDir = i % 2 == 0 ? -1 : 1;
            //BatProjectile_Missile projectile = Instantiate(projectilePrefab, shootPoint).GetComponent<BatProjectile_Missile>();
            projectile.Init(transform);
            projectile.transform.SetParent(null);
            projectile.dmgData.damage = AttackDamage;
            
            yield return new WaitForSeconds(attackSeparationTime);
        }

        blockAnim = false;
        ChangeAnim(AnimState.IDLE);
        enemyAnimator.speed = 1f;

        canRotate = true;
    }
    protected override void EndRndMovesBehaviour()
    {
        base.EndRndMovesBehaviour();

        //float distToPlayer = Vector3.Distance(transform.position, player.position);
        if (canAttack && InAttackRange())
        {
            ChangeState(States.ATTACK);

            //RaycastHit hit;
            //bool hitCollided = Physics.Raycast(transform.position, (player.position - transform.position).normalized, out hit, distToPlayer, layerMask);
            //if (hitCollided && hit.transform.CompareTag("Player"))
            //{
            //    ChangeState(States.ATTACK);
            //}
        }
    }

    protected override void IdleStart() { base.IdleStart(); }
    protected override void MoveToTargetStart() { base.MoveToTargetStart(); }
    protected override void AttackStart()
    {
        base.AttackStart();
        attackTimer = 0f;
        moveDir = Vector3.zero;
        StopRB(stopForce);
        //StartCoroutine(Attack_Cor());
    }


    protected override void IdleExit() { base.IdleExit(); }
    protected override void MoveToTargetExit() { base.MoveToTargetExit(); }
    protected override void AttackExit() { base.AttackExit(); }

}
