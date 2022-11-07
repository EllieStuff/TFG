using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BaseEnemyScript : MonoBehaviour
{
    public enum States { IDLE, MOVE_TO_TARGET, ATTACK, DAMAGE }

    const float DEFAULT_SPEED_REDUCTION = 1.4f;


    [Header("BaseEnemy")]
    [SerializeField] internal float baseRotSpeed = 4;
    [SerializeField] internal float playerDetectionDistance = 8f, playerStopDetectionDistance = 15f;
    [SerializeField] internal float enemyStartAttackDistance, enemyStopAttackDistance;
    [SerializeField] internal bool isAttacking = false;
    [SerializeField] internal float baseMoveSpeed;
    [SerializeField] internal float baseDamageTimer;

    //PROVISIONAL

    [SerializeField] Material enemyMat;
    private MeshRenderer enemyOwnMat;
    internal Material newMatDef;

    //____________________________________________________

    internal float damageTimer = 0;
    PlayerSword playerSword;
    LifeSystem playerLife;
    LifeSystem enemyLife;
    bool SwordTouching;

    readonly internal Vector3 
        baseMinVelocity = new Vector3(-10, -10, -10), 
        baseMaxVelocity = new Vector3(10, 10, 10);

    internal States state = States.IDLE;
    internal Rigidbody rb;
    internal Transform player;
    internal float actualMoveSpeed;
    internal float actualRotSpeed;
    internal float speedMultiplier = 1.0f;
    internal Vector3 actualMinVelocity, actualMaxVelocity;
    [HideInInspector] public Vector3 moveDir = Vector3.zero;
    internal bool canMove = true, canRotate = true;

    //private WeaponStats playerWeaponStats;


    // Start is called before the first frame update
    void Start()
    {
        Start_Call();
    }
    internal virtual void Start_Call()
    {
        rb = GetComponent<Rigidbody>();
        enemyLife = GetComponent<LifeSystem>();
        GameObject playerGO = GameObject.FindGameObjectWithTag("Player");
        player = playerGO.transform;
        playerLife = playerGO.GetComponent<LifeSystem>();
        playerSword = playerGO.GetComponent<PlayerSword>();

        ResetSpeed();

        //PROVISIONAL

        Material newMat = new Material(enemyMat);
        enemyOwnMat = GetComponent<MeshRenderer>();
        newMatDef = newMat;
        enemyOwnMat.material = newMatDef;

        //____________________________________________________
    }

    private void Update()
    {
        Update_Call();
    }
    internal virtual void Update_Call() { }

    void FixedUpdate()
    {
        FixedUpdate_Call();
    }
    internal virtual void FixedUpdate_Call()
    {
        UpdateStateMachine();

        LimitVelocity();

        if (canRotate)
        {
            if (moveDir != Vector3.zero)
            {
                Quaternion targetRot = Quaternion.LookRotation(rb.velocity.normalized, Vector3.up);
                transform.rotation = Quaternion.RotateTowards(transform.rotation, targetRot, actualRotSpeed * speedMultiplier * Time.deltaTime);
            }
            else
            {
                Quaternion targetRot = Quaternion.LookRotation((player.position - transform.position).normalized, Vector3.up);
                transform.rotation = Quaternion.RotateTowards(transform.rotation, targetRot, actualRotSpeed * speedMultiplier * Time.deltaTime);
            }
        }

        //if (SwordTouching && playerSword.isAttacking && state != States.DAMAGE)
        //{
        //    enemyLife.Damage(playerWeaponStats.weaponDamage, enemyLife.healthState);
        //    newMatDef.color = Color.red;
        //    damageTimer = baseDamageTimer;
        //    ChangeState(States.DAMAGE);
        //}
    }

    internal virtual void UpdateStateMachine()
    {
        switch (state)
        {
            case States.IDLE:
                //patrol
                IdleUpdate();

                break;
            case States.MOVE_TO_TARGET:
                //approach to player
                MoveToTargetUpdate();

                break;
            case States.ATTACK:
                //attack
                AttackUpdate();

                break;

            case States.DAMAGE:
                //receive damage
                DamageUpdate();

                break;

            default:
                Debug.LogWarning("State not found");
                break;
        }
    }

    internal virtual void DamageUpdate()
    {
        damageTimer -= Time.deltaTime;

        if(enemyLife.currLife <= 0)
        {
            //enemyDies animation and destroy then
            Destroy(gameObject);
        }

        if (damageTimer <= 0)
        {
            newMatDef.color = Color.white;
            damageTimer = baseDamageTimer;
            ChangeState(States.IDLE);
        }
    }
    internal virtual void IdleUpdate()
    {
        if (Vector3.Distance(transform.position, player.position) <= playerDetectionDistance)
            ChangeState(States.MOVE_TO_TARGET);
    }
    internal virtual void MoveToTargetUpdate()
    {
        Vector3 targetMoveDir = (player.position - transform.position).normalized;
        MoveRB(targetMoveDir, actualMoveSpeed * speedMultiplier);

        if (Vector3.Distance(transform.position, player.position) > playerStopDetectionDistance)
            ChangeState(States.IDLE);

        if (Vector3.Distance(transform.position, player.position) <= enemyStartAttackDistance)
            ChangeState(States.ATTACK);
    }
    internal virtual void AttackUpdate()
    {
        if (!isAttacking && Vector3.Distance(transform.position, player.position) > enemyStopAttackDistance)
            ChangeState(States.MOVE_TO_TARGET);
    }
    
    internal virtual void IdleStart() { StopRB(5.0f); }
    internal virtual void MoveToTargetStart() { }
    internal virtual void AttackStart() { }
    internal virtual void DamageStart() { }
    internal virtual void IdleExit() { }
    internal virtual void MoveToTargetExit() { }
    internal virtual void AttackExit() { }
    internal virtual void DamageExit() { }

    public virtual void ChangeState(States _state)
    {
        switch (state)
        {
            case States.IDLE:
                IdleExit();
                break;
            case States.MOVE_TO_TARGET:
                MoveToTargetExit();
                break;
            case States.ATTACK:
                AttackExit();
                break;

            default:
                Debug.LogWarning("State not found");
                break;
        }

        state = _state;

        switch (state)
        {
            case States.IDLE:
                IdleStart();
                break;
            case States.MOVE_TO_TARGET:
                MoveToTargetStart();
                break;
            case States.ATTACK:
                AttackStart();
                break;

            default:
                Debug.LogWarning("State not found");
                break;
        }
    }


    internal Vector3 ClampVector(Vector3 _originalVec, Vector3 _minVec, Vector3 _maxVec)
    {
        return new Vector3(
            Mathf.Clamp(_originalVec.x, _minVec.x, _maxVec.x),
            Mathf.Clamp(_originalVec.y, _minVec.y, _maxVec.y),
            Mathf.Clamp(_originalVec.z, _minVec.z, _maxVec.z)
        );
    }

    internal void MoveRB(Vector3 _moveDir, float _moveForce, ForceMode _forceMode = ForceMode.Force)
    {
        if (canMove)
            rb.AddForce(_moveDir * _moveForce, _forceMode);
    }
    internal void StopRB(float _speedReduction = DEFAULT_SPEED_REDUCTION)
    {
        rb.velocity = new Vector3(rb.velocity.x / _speedReduction, rb.velocity.y, rb.velocity.z / _speedReduction);
    }
    internal void SetVelocityLimit(Vector3 _minSpeed, Vector3 _maxSpeed)
    {
        actualMinVelocity = _minSpeed;
        actualMaxVelocity = _maxSpeed;
    }
    void LimitVelocity()
    {
        rb.velocity = ClampVector(rb.velocity, actualMinVelocity * speedMultiplier, actualMaxVelocity * speedMultiplier);
    }
    public void ChangeSpeed(float _moveForce, float _rotSpeed, Vector3 _minSpeed, Vector3 _maxSpeed)
    {
        actualMoveSpeed = _moveForce;
        actualRotSpeed = _rotSpeed;
        actualMinVelocity = _minSpeed;
        actualMaxVelocity = _maxSpeed;
    }
    public void ResetSpeed()
    {
        actualMoveSpeed = baseMoveSpeed;
        actualRotSpeed = baseRotSpeed;
        actualMinVelocity = baseMinVelocity;
        actualMaxVelocity = baseMaxVelocity;
    }


    internal Vector3 NormalizeDirection(Vector3 moveDir)
    {
        if (moveDir.x > 0.5f)
            moveDir = new Vector3(1, moveDir.y, moveDir.z);
        if (moveDir.x < -0.5f)
            moveDir = new Vector3(-1, moveDir.y, moveDir.z);

        if (moveDir.z > 0.5f)
            moveDir = new Vector3(moveDir.x, moveDir.y, 1);
        if (moveDir.z < -0.5f)
            moveDir = new Vector3(moveDir.x, moveDir.y, -1);

        return moveDir;
    }


    void OnCollisionEnter(Collision col)
    {
        CollisionEnterEvent(col);
    }
    internal virtual void CollisionEnterEvent(Collision col)
    {
        if (col.gameObject.CompareTag("floor"))
            rb.useGravity = false;
    }

    void OnCollisionExit(Collision col)
    {
        CollisionExitEvent(col);
    }
    internal virtual void CollisionExitEvent(Collision col)
    {
        if (col.gameObject.CompareTag("floor"))
            rb.useGravity = true;
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.tag.Equals("SwordRegion"))
        {
            SwordTouching = true;
            WeaponStats weaponStats = other.GetComponent<WeaponStats>();
            //playerWeaponStats = weaponStats;
            enemyLife.Damage(weaponStats.weaponDamage, HealthState.GetHealthStateByEffect(weaponStats.weaponEffect));
            newMatDef.color = Color.red;
            damageTimer = baseDamageTimer;
            ChangeState(States.DAMAGE);
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.tag.Equals("SwordRegion"))
            SwordTouching = false;
    }
}
