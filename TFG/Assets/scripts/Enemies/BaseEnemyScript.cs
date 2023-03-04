using System.Collections.Generic;
using UnityEngine;

public class BaseEnemyScript : MonoBehaviour
{
    public enum States { IDLE, RANDOM_MOVEMENT, MOVE_TO_TARGET, ATTACK, REST, DAMAGE, DEATH }
    public enum EnemyType { PLANT, BAT, RAT, GHOST }

    const float DEFAULT_SPEED_REDUCTION = 1.4f;
    //const float PLAYER_HIT_DISTANCE_SWORD = 3;
    const float THRESHOLD = 1f;
    const int ENEMY_LAYER = 7, ENEMYCOLPLAYER_LAYER = 9;


    [Header("BaseEnemy")]
    [SerializeField] internal EnemyType enemyType;
    [SerializeField] internal float baseRotSpeed = 4;
    [SerializeField] internal float playerDetectionDistance = 8f, playerStopDetectionDistance = 15f;
    [SerializeField] protected float stopForce = 90f;
    [SerializeField] internal float enemyStartAttackDistance, enemyStopAttackDistance;
    [SerializeField] internal bool isAttacking = false;
    [SerializeField] internal float baseMoveSpeed, playerFoundSpeed;
    [SerializeField] bool attacksTargetWOSeeingIt = false;  // WO == Without
    [SerializeField] bool movesToTargetWOSeeingIt = false;
    [SerializeField] bool stopRndMoveWhenSeeingTarget = true;
    [SerializeField] internal float baseDamageTimer;
    [SerializeField] internal float baseDeathTime;
    [SerializeField] protected bool movesToTarget = true;
    [SerializeField] bool needsToRest = false;
    [SerializeField] Vector2 idleWait = new Vector2(0.6f, 2.0f);
    [SerializeField] Vector2 restWait = new Vector2(3.0f, 3.5f);
    [SerializeField] int numOfRndMoves = 0;
    [SerializeField] protected float dmgOnTouch = 5f;
    [SerializeField] Transform enemyLightsHolder;
    [SerializeField] Transform lookPoint;

    internal ZoneScript zoneSystem;
    internal float damageTimer = 0;
    float idleWaitTimer = 0f;
    int rndMovesDone = 0;
    Vector3 rndTarget;
    float restTimer = 0f;
    float rndMoveTimer = 0f, rndMoveWait = 5f;
    LifeSystem enemyLife;
    protected DamageData touchBodyDamageData;

    readonly internal Vector3 
        baseMinVelocity = new Vector3(-10, -10, -10), 
        baseMaxVelocity = new Vector3(10, 10, 10);

    internal States state = States.IDLE;
    internal Rigidbody rb;
    internal Transform player;
    internal float actualMoveSpeed;
    internal float actualRotSpeed;
    internal float speedMultiplier = 0.5f;
    internal Vector3 actualMinVelocity, actualMaxVelocity;
    [HideInInspector] public Vector3 moveDir = Vector3.zero;
    internal bool canMove = true, canRotate = true, canAttack = true;
    internal Quaternion targetRot;
    protected bool endAttackFlag = true;
    internal bool canEnterDamageState = true;
    List<Light> enemyLights = new List<Light>();

    bool MakesRandomMoves { get { return numOfRndMoves != 0; } }
    bool HaveRandomMovesAvailable { get { return numOfRndMoves < 0 || rndMovesDone < numOfRndMoves; } }

    //PLACEHOLDER
    [SerializeField] SkinnedMeshRenderer enemyMesh;
    [SerializeField] Material transparentMat;
    //_________________________________________

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
        touchBodyDamageData = transform.Find("DamageArea").GetComponent<DamageData>();
        player = GameObject.FindGameObjectWithTag("Player").transform;
        enemyMesh.material = new Material(enemyMesh.material);

        touchBodyDamageData.damage = dmgOnTouch;
        if (enemyLightsHolder != null)
        {
            for (int i = 0; i < enemyLightsHolder.childCount; i++)
            {
                enemyLights.Add(enemyLightsHolder.GetChild(i).GetComponent<Light>());
            }
        }

        if(lookPoint = null)
        {
            lookPoint = transform;
            Debug.LogWarning("LookPoint was not set on " + transform.name + ", using its transform instead");
        }

        Physics.IgnoreLayerCollision(ENEMY_LAYER, ENEMYCOLPLAYER_LAYER);

        ResetSpeed();
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

        if (canRotate && !state.Equals(States.DEATH))
        {
            if (moveDir != Vector3.zero)
            {
                Vector3 rotDir = new Vector3(rb.velocity.x, 0f, rb.velocity.z).normalized;
                targetRot = Quaternion.LookRotation(rotDir, Vector3.up);
                //targetRot = Quaternion.LookRotation(rb.velocity.normalized, Vector3.up);
                transform.rotation = Quaternion.RotateTowards(transform.rotation, targetRot, actualRotSpeed * speedMultiplier * Time.deltaTime);
            }
            else
            {
                targetRot = Quaternion.LookRotation((player.position - transform.position).normalized, Vector3.up);
                transform.rotation = Quaternion.RotateTowards(transform.rotation, targetRot, actualRotSpeed * speedMultiplier * Time.deltaTime);
                Vector3 auxRot = transform.eulerAngles;
                transform.rotation = Quaternion.Euler(0, auxRot.y, 0);
            }
        }
    }


    #region StateMachine
    internal virtual void UpdateStateMachine()
    {
        switch (state)
        {
            case States.IDLE:
                IdleUpdate();
                break;

            case States.RANDOM_MOVEMENT:
                RandomMovementUpdate();
                break;

            case States.MOVE_TO_TARGET:
                MoveToTargetUpdate();
                break;

            case States.ATTACK:
                AttackUpdate();
                break;

            case States.REST:
                RestUpdate();
                break;

            case States.DAMAGE:
                DamageUpdate();
                break;

            case States.DEATH:
                DeathUpdate();
                break;

            default:
                Debug.LogWarning("State not found");
                break;
        }
    }
    public virtual void ChangeState(States _state)
    {
        if (!canEnterDamageState && _state == States.DAMAGE) return;

        switch (state)
        {
            case States.IDLE:
                IdleExit();
                break;
            case States.RANDOM_MOVEMENT:
                RandomMovementExit();
                break;
            case States.MOVE_TO_TARGET:
                MoveToTargetExit();
                break;
            case States.ATTACK:
                AttackExit();
                break;
            case States.REST:
                RestExit();
                break;
            case States.DAMAGE:
                DamageExit();
                break;
            case States.DEATH:
                DeathExit();
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
            case States.RANDOM_MOVEMENT:
                RandomMovementStart();
                break;
            case States.MOVE_TO_TARGET:
                MoveToTargetStart();
                break;
            case States.ATTACK:
                AttackStart();
                break;
            case States.REST:
                RestStart();
                break;
            case States.DAMAGE:
                DamageStart();
                break;
            case States.DEATH:
                DeathStart();
                break;

            default:
                Debug.LogWarning("State not found");
                break;
        }
    }
    #endregion StateMachine

    #region Updates
    internal virtual void IdleUpdate()
    {
        idleWaitTimer -= Time.deltaTime;
        if (idleWaitTimer > 0) return;
        else idleWaitTimer = Random.Range(idleWait.x, idleWait.y);

        //Debug.Log("Dbg Idle");
        float distToPlayer = Vector3.Distance(transform.position, player.position);
        if (movesToTarget)
        {
            if (canMove && distToPlayer <= playerDetectionDistance)
            {
                if (movesToTargetWOSeeingIt)
                    ChangeState(States.MOVE_TO_TARGET);
                else
                {
                    RaycastHit hit;
                    bool hitCollided = Physics.Raycast(transform.position, (player.position - transform.position).normalized, out hit, distToPlayer, ENEMY_LAYER);
                    if (hitCollided && hit.transform.CompareTag("Player"))
                    {
                        if (distToPlayer > enemyStartAttackDistance)
                        {
                            ChangeState(States.MOVE_TO_TARGET);
                            return;
                        }
                    }
                }
            }
        }
        
        if (canAttack && distToPlayer <= enemyStartAttackDistance)
        {
            if (attacksTargetWOSeeingIt)
                ChangeState(States.ATTACK);
            else
            {
                RaycastHit hit;
                bool hitCollided = Physics.Raycast(transform.position, (player.position - transform.position).normalized, out hit, distToPlayer, ENEMY_LAYER);
                if (hitCollided && hit.transform.CompareTag("Player"))
                {
                    if (Vector3.Angle(transform.forward, player.position - transform.position) < 1)
                        ChangeState(States.ATTACK);
                    else idleWaitTimer = -1;
                    return;
                }
            }
        }
        
        if (MakesRandomMoves && HaveRandomMovesAvailable)
        {
            ChangeState(States.RANDOM_MOVEMENT);
        }

    }
    internal virtual void RandomMovementUpdate()
    {
        rndMoveTimer += Time.deltaTime;
        moveDir = (rndTarget - transform.position).normalized;
        MoveRB(moveDir, ((actualMoveSpeed * 3f) / 4f) * speedMultiplier);


        if (stopRndMoveWhenSeeingTarget)
        {
            RaycastHit hit;
            bool hitCollided = Physics.Raycast(transform.position, (player.position - transform.position).normalized, out hit,
                Vector3.Distance(transform.position, player.position), ENEMY_LAYER);
            if (hitCollided && hit.transform.CompareTag("Player"))
            {
                ChangeState(States.MOVE_TO_TARGET);
                return;
            }
        }

        if ((lookPoint != null && Physics.Raycast(lookPoint.position, lookPoint.forward, 3f))
            || Vector3.Distance(transform.position, rndTarget) < THRESHOLD
            || rndMoveTimer >= rndMoveWait)
        {
            //Debug.Log("ExitRandomMovement");
            ChangeState(States.IDLE);
        }
    }
    internal virtual void MoveToTargetUpdate()
    {
        rb.useGravity = false;

        Vector3 targetMoveDir = (player.position - transform.position).normalized;
        MoveRB(targetMoveDir, actualMoveSpeed * speedMultiplier);


        if (Vector3.Distance(transform.position, player.position) > playerStopDetectionDistance)
            ChangeState(States.IDLE);

        if (!movesToTargetWOSeeingIt)
        {
            RaycastHit hit;
            bool hitCollided = Physics.Raycast(transform.position, (player.position - transform.position).normalized, out hit, playerStopDetectionDistance, ENEMY_LAYER);
            if (!hitCollided || !hit.transform.CompareTag("Player"))
                ChangeState(States.IDLE);
        }

        if (canAttack && Vector3.Distance(transform.position, player.position) <= enemyStartAttackDistance)
            ChangeState(States.ATTACK);
    }
    internal virtual void AttackUpdate()
    {
        if (!canAttack) { ChangeState(States.IDLE); return; }

        //Debug.Log("Dbg Attacking");
        if (endAttackFlag)
        {
            if (needsToRest) ChangeState(States.REST);
            else ChangeState(States.IDLE);
        }
    }
    internal virtual void RestUpdate()
    {
        restTimer -= Time.deltaTime;
        //Debug.Log("Dbg Resting");
        if (restTimer <= 0f) ChangeState(States.IDLE);
    }
    internal virtual void DamageUpdate()
    {
        damageTimer -= Time.deltaTime;

        if (enemyLife.isDead)
        {
            ChangeState(States.DEATH);
            return;
        }

        if (damageTimer <= 0)
        {
            float distToPlayer = Vector3.Distance(transform.position, player.position);
            if (distToPlayer <= enemyStartAttackDistance) ChangeState(States.ATTACK);
            else if (distToPlayer <= playerDetectionDistance) ChangeState(States.MOVE_TO_TARGET);
            else ChangeState(States.IDLE);
        }
    }
    internal virtual void DeathUpdate()
    {
        damageTimer -= Time.deltaTime;

        if (enemyMesh.material.color.a > 0)
        {
            enemyMesh.material.color -= new Color(0, 0, 0, Time.deltaTime);
        }
        if (enemyLights.Count > 0)
        {
            for (int i = 0; i < enemyLights.Count; i++)
            {
                enemyLights[i].intensity -= Time.deltaTime;
                if (enemyLights[i].intensity <= 0f)
                {
                    enemyLights.RemoveAt(i);
                    i--;
                }
            }
        }

        if (damageTimer <= 0)
        {
            Destroy(gameObject);
        }
    }
    #endregion Updates

    #region Starts
    internal virtual void IdleStart() { StopRB(stopForce); idleWaitTimer = Random.Range(idleWait.x, idleWait.y); }
    internal virtual void RandomMovementStart()
    {
        float rndFactor = 4f;
        bool collided = true;
        int currTrials = 0, maxTrials = 10;
        while (collided && currTrials <= maxTrials)
        {
            rndTarget = transform.position + new Vector3(Random.Range(-rndFactor, rndFactor), 0f, Random.Range(-rndFactor, rndFactor));
            collided = Physics.BoxCast(transform.position, Vector3.one, 
                (rndTarget - transform.position).normalized, Quaternion.identity, Vector3.Distance(transform.position, rndTarget), ENEMY_LAYER);
            currTrials++;
        }
        //Debug.Log("Num of Trials: " + currTrials);
        actualMoveSpeed = baseMoveSpeed;
        rndMoveTimer = 0f;
        if (collided) ChangeState(States.IDLE);
    }
    internal virtual void MoveToTargetStart() { if (numOfRndMoves > 0) rndMovesDone = 0; actualMoveSpeed = playerFoundSpeed; }
    internal virtual void AttackStart() { if (numOfRndMoves > 0) rndMovesDone = 0; }
    internal virtual void RestStart() { restTimer = Random.Range(restWait.x, restWait.y); canMove = canRotate = false; }
    internal virtual void DamageStart() { damageTimer = baseDamageTimer; }
    internal virtual void DeathStart()
    {
        GetComponent<Collider>().enabled = false;
        rb.useGravity = false;

        damageTimer = baseDeathTime;
        enemyMesh.material = transparentMat;

        if (zoneSystem != null)
        {
            zoneSystem.enemiesQuantity -= 1;
            zoneSystem = null;
        }
    }
    #endregion Starts

    #region Exits
    internal virtual void IdleExit() { }
    internal virtual void RandomMovementExit() { if (numOfRndMoves > 0) rndMovesDone++; }
    internal virtual void MoveToTargetExit() { }
    internal virtual void AttackExit() { }
    internal virtual void RestExit() { canMove = canRotate = true; }
    internal virtual void DamageExit() { }
    internal virtual void DeathExit() { }
    #endregion Exits


    #region Misc
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
            rb.velocity = _moveDir * _moveForce;
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
    #endregion Misc


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

        //if (col.gameObject.CompareTag("Player"))
        //    col.transform.GetComponent<LifeSystem>().Damage(dmgOnTouch, ElementsManager.Elements.NORMAL);
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

}
