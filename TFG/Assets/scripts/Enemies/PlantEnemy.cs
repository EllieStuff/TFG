using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlantEnemy : BaseEnemyScript
{
    enum AnimState { IDLE, ATTACKING, DEAD }
    enum AttackType { NORMAL_THROW, CIRCLE_ATTACK, FOUR_PROJECTILES, THREE_PROJECTILES }

    const int CIRCLE_ITERATIONS = 24;
    const int CIRCLE_MULTIPLIER = 50;
    const float RESET_ANIM_TIMER = 1.5f;

    [Header("PlantEnemy")]
    [SerializeField] float attackAnimationTime;
    [SerializeField] bool shootWithObstacles = true;
    [SerializeField] Animator enemyAnimator;
    [SerializeField] GameObject projectilePrefab;
    [SerializeField] Transform shootPoint;
    [SerializeField] AttackType attackStyle;
    [SerializeField] int numOfAttacks = 1;
    [SerializeField] float attackSeparationTime = 0.2f;

    float attackTimer;

    private bool blockAnim = false;

    AnimState currentAnim = AnimState.IDLE;

    protected override void Start_Call() { base.Start_Call(); }

    protected override void Update_Call() { base.Update_Call(); }

    protected override void FixedUpdate_Call() { base.FixedUpdate_Call(); }

    private void ChangeAnim(AnimState _state)
    {
        if (!blockAnim || _state.Equals(AnimState.DEAD))
        {
            enemyAnimator.SetInteger("state", (int)_state);
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
        base.MoveToTargetUpdate();
    }
    //protected override void DamageUpdate()
    //{
    //    //enemyAnimator.SetFloat("state", (int)AnimState.IDLE);
    //    base.DamageUpdate();
    //}
    protected override void AttackUpdate()
    {
        base.AttackUpdate();

        if (!dmgActivated)
            rb.velocity = new Vector3(0, rb.velocity.y, 0);

        attackTimer -= Time.fixedDeltaTime;

        if (attackTimer <= 0)
        {
            StartCoroutine(Attack_Cor());
            StartCoroutine(ResetAnimCor());
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

    IEnumerator Attack_Cor()
    {
        yield return new WaitForSeconds(attackChargingTime);

        //place shoot animation here
        isAttacking = true;
        ChangeAnim(AnimState.ATTACKING);

        blockAnim = true;
        yield return new WaitForSeconds(attackAnimationTime);

        //AUDIO
        AudioManager.instance.PlayOneShot(FMODEvents.instance.plantAttack, this.transform.position);

        RaycastHit hit;
        if (Physics.Raycast(shootPoint.position, (playerRef.position - shootPoint.position).normalized, out hit))
        {
            if (!hit.transform.CompareTag("Wall"))
            {
                if (shootWithObstacles || (!shootWithObstacles && !hit.transform.CompareTag("Obstacle")))
                {
                    for (int i = 0; i < numOfAttacks; i++)
                    {
                        yield return new WaitForSeconds(attackAnimationTime);
                        AttackStateMachine(attackStyle);
                        yield return new WaitForSeconds(attackSeparationTime);
                    }
                }
            }
        }
        isAttacking = false;
        blockAnim = false;
    }

    void AttackStateMachine(AttackType type)
    {
        ProjectileData projectile;
        switch (type)
        {
            case AttackType.FOUR_PROJECTILES:
                for (int knifeDirState = 0; knifeDirState < 4; knifeDirState++)
                {
                    projectile = Instantiate(projectilePrefab, transform).GetComponent<ProjectileData>();
                    projectile.Init(transform);
                    projectile.transform.SetParent(null);
                    switch (knifeDirState)
                    {
                        case 0:
                            projectile.moveDir = new Vector3(1, 0, 0);
                            break;
                        case 1:
                            projectile.moveDir = new Vector3(-1, 0, 0);
                            break;
                        case 2:
                            projectile.moveDir = new Vector3(0, 0, 1);
                            break;
                        case 3:
                            projectile.moveDir = new Vector3(0, 0, -1);
                            break;
                    }
                }
                break;
            case AttackType.CIRCLE_ATTACK:
                float circleY = 0.1f;
                float circleX = 0.1f;
                float auxTimer = 0;
                for (int i = 0; i < CIRCLE_ITERATIONS; i++)
                {
                    circleY = Mathf.Sin(auxTimer);
                    circleX = Mathf.Cos(auxTimer);
                    auxTimer += Time.deltaTime * CIRCLE_MULTIPLIER;
                    KnifeThrown knife_2 = Instantiate(projectilePrefab, transform).GetComponent<KnifeThrown>();
                    knife_2.knifeDir = new Vector3(circleX, 0, circleY);
                    knife_2.SetOwnerTransform(transform);
                }
                break;
            case AttackType.THREE_PROJECTILES:
                for (int direction = 0; direction < 3; direction++)
                {
                    KnifeThrown knife_3 = Instantiate(projectilePrefab, transform).GetComponent<KnifeThrown>();
                    switch (direction)
                    {
                        case 0:
                            knife_3.knifeDir = new Vector3(1, 0, 1);
                            break;
                        case 1:
                            knife_3.knifeDir = new Vector3(-1, 0, 1);
                            break;
                        case 2:
                            knife_3.knifeDir = new Vector3(0, 0, 1);
                            break;
                    }
                    knife_3.SetOwnerTransform(transform);
                    knife_3.localDir = true;
                    knife_3.entityThrowingIt = transform;
                }
                break;
            case AttackType.NORMAL_THROW:
                projectile = Instantiate(projectilePrefab, shootPoint).GetComponent<ProjectileData>();
                projectile.Init(transform);
                projectile.transform.SetParent(null);
                projectile.dmgData.damage = AttackDamage;
                //projectile.moveDir = (player.position - transform.position).normalized;
                break;
        }
    }

    protected override void IdleStart() { base.IdleStart(); enemyAnimator.SetFloat("state", (int)AnimState.IDLE); }
    protected override void MoveToTargetStart() { base.MoveToTargetStart(); }
    protected override void AttackStart() { base.AttackStart(); attackTimer = 0f; }


    protected override void IdleExit() { base.IdleExit(); }
    protected override void MoveToTargetExit() { base.MoveToTargetExit(); }
    protected override void AttackExit() { base.AttackExit(); }


}
