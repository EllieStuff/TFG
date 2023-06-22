using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class BaseEnemyScript : MonoBehaviour
{
    public enum States { IDLE, RANDOM_MOVEMENT, MOVE_TO_TARGET, ATTACK, REST, /*DAMAGE,*/ DEATH }
    public enum EnemyType { PLANT, BAT, RAT, GHOST }

    const float DEFAULT_SPEED_REDUCTION = 1.4f;
    const float ATTACK_ANGLE_THRESHOLD = 15f;
    const float ATTACK_MARGIN = 0.03f;
    const float ATTACK_DISTANCE_THRESHOLD = 1f;


    [Header("Base Enemy")]
    [SerializeField] EnemyType enemyType;
    [SerializeField] protected LayerMask layerMask;
    [SerializeField] protected float baseRotSpeed = 4;
    [SerializeField] protected float playerDetectionDistance = 8f, playerStopDetectionDistance = 15f, attackRange = -1f;
    [SerializeField] protected float stopForce = 90f;
    [SerializeField] protected bool isAttacking = false;
    [SerializeField] protected float baseMoveSpeed, playerFoundSpeed;
    [SerializeField] protected bool attacksTargetWOSeeingIt = false;  // WO == Without
    [SerializeField] bool movesToTargetWOSeeingIt = false;
    [SerializeField] bool stopRndMoveWhenSeeingTarget = true;
    [SerializeField] bool endsAttackWhenTargetOutOfRange = true;
    [SerializeField] protected float baseDamageTimer;
    [SerializeField] protected float baseDeathTime;
    [SerializeField] protected bool movesToTarget = true;
    [SerializeField] bool needsToRest = false;
    [SerializeField] Vector2 idleWait = new Vector2(0.6f, 2.0f);
    [SerializeField] protected Vector2 attackWait = new Vector2(1f, 1.5f);
    [SerializeField] Vector2 restWait = new Vector2(3.0f, 3.5f);
    [SerializeField] protected float attackChargingTime = 1f;
    [SerializeField] int numOfRndMoves = 0;
    [SerializeField] float baseAttackDamage = 100f;
    [SerializeField] protected float dmgOnTouch = 20f;
    [SerializeField] Vector2Int moneyDropped;
    [SerializeField] GameObject coinPrefab;
    [SerializeField] Transform enemyLightsHolder;
    [SerializeField] Transform lookPoint;
    [SerializeField] bool disableAutoGravity;
    [SerializeField] bool test = false;

    [HideInInspector] public ZoneScript zoneSystem;
    protected float damageTimer = 0;
    float idleWaitTimer = 0f;
    int rndMovesDone = 0;
    Vector3 rndTarget;
    float restTimer = 0f;
    float rndMoveTimer = 0f, rndMoveWait = 5f;
    protected LifeSystem enemyLife;
    protected DamageData touchBodyDamageData;
    protected bool dmgActivated = false;

    readonly protected Vector3 
        baseMinVelocity = new Vector3(-10, 0, -10), 
        baseMaxVelocity = new Vector3(10, 0, 10);

    States state = States.IDLE;
    protected Rigidbody rb;
    protected Transform playerRef;
    protected float actualMoveSpeed;
    protected float actualRotSpeed;
    protected float speedMultiplier = 0.5f;
    protected Vector3 actualMinVelocity, actualMaxVelocity;
    protected Vector3 moveDir = Vector3.zero;
    [HideInInspector] public bool canMove = true, canRotate = true, canAttack = true;
    protected Quaternion targetRot;
    [HideInInspector] public bool canEnterDamageState = true;
    protected List<Light> enemyLights = new List<Light>();
    [HideInInspector] public bool waiting = true;

    public bool IsAttacking { get { return isAttacking; } }
    public States State { get { return state; } }
    bool MakesRandomMoves { get { return numOfRndMoves != 0; } }
    bool HaveRandomMovesAvailable { get { return numOfRndMoves < 0 || rndMovesDone < numOfRndMoves; } }
    protected float AttackDamage {
        get {
            switch (DifficultyManager.Difficulty)
            {
                case DifficultyMode.EASY: return baseAttackDamage * DifficultyManager.Enemies_AtkDmgMultiplier_EasyMode;
                case DifficultyMode.NORMAL: return baseAttackDamage * DifficultyManager.Enemies_AtkDmgMultiplier_NormalMode;
                case DifficultyMode.HARD: return baseAttackDamage * DifficultyManager.Enemies_AtkDmgMultiplier_HardMode;
                default: return baseAttackDamage;
            }
        }
    }
    protected float AttackWait {
        get {
            float rndWait = Random.Range(attackWait.x, attackWait.y);
            switch (DifficultyManager.Difficulty)
            {
                case DifficultyMode.EASY: return rndWait * DifficultyManager.Enemies_AtkWaitMultiplier_EasyMode;
                case DifficultyMode.NORMAL: return rndWait * DifficultyManager.Enemies_AtkWaitMultiplier_NormalMode;
                case DifficultyMode.HARD: return rndWait * DifficultyManager.Enemies_AtkWaitMultiplier_HardMode;
                default: return rndWait;
            }
        }
    }


    //PLACEHOLDER
    [SerializeField] protected SkinnedMeshRenderer enemyMesh;
    [SerializeField] protected MeshRenderer enemyMeshTmp;
    [SerializeField] protected Material transparentMat;
    //_________________________________________

    //private WeaponStats playerWeaponStats;


    // Start is called before the first frame update
    void Start()
    {
        Start_Call();
    }
    protected virtual void Start_Call()
    {
        rb = GetComponent<Rigidbody>();
        enemyLife = GetComponent<LifeSystem>();
        touchBodyDamageData = transform.Find("DamageArea").GetComponent<DamageData>();
        playerRef = GameObject.FindGameObjectWithTag("Player").transform;
        //if(enemyMesh != null) enemyMesh.material = new Material(enemyMesh.material);
        //else enemyMeshTmp.material = new Material(enemyMeshTmp.material);

        int enemiesLayer = LayerMask.NameToLayer("Enemy");
        Physics.IgnoreLayerCollision(enemiesLayer, enemiesLayer);

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

        ResetSpeed();
    }

    private void Update()
    {
        Update_Call();
    }
    protected virtual void Update_Call() { }

    void FixedUpdate()
    {
        FixedUpdate_Call();
    }
    protected virtual void FixedUpdate_Call()
    {
        if (!waiting)
        {
            UpdateStateMachine();

            LimitVelocity();
        }


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
                targetRot = Quaternion.LookRotation((playerRef.position - transform.position).normalized, Vector3.up);
                transform.rotation = Quaternion.RotateTowards(transform.rotation, targetRot, actualRotSpeed * speedMultiplier * Time.deltaTime);
                Vector3 auxRot = transform.eulerAngles;
                transform.rotation = Quaternion.Euler(0, auxRot.y, 0);
            }
        }
    }


    #region StateMachine
    protected virtual void UpdateStateMachine()
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

            //case States.DAMAGE:
            //    DamageUpdate();
            //    break;

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
        //if (!canEnterDamageState && _state == States.DAMAGE) return;

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
            //case States.DAMAGE:
            //    DamageExit();
            //    break;
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
            //case States.DAMAGE:
            //    DamageStart();
            //    break;
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
    protected virtual void IdleUpdate()
    {
        idleWaitTimer -= Time.deltaTime;
        if (idleWaitTimer > 0) return;
        else idleWaitTimer = Random.Range(idleWait.x, idleWait.y);

        //Debug.Log("Dbg Idle");
        float distToPlayer = Vector3.Distance(transform.position, playerRef.position);
        if (movesToTarget)
        {
            if (canMove && distToPlayer <= playerDetectionDistance)
            {
                if (movesToTargetWOSeeingIt)
                    ChangeState(States.MOVE_TO_TARGET);
                else
                {
                    RaycastHit hit;
                    bool hitCollided = Physics.Raycast(transform.position, (playerRef.position - transform.position).normalized, out hit, distToPlayer, layerMask);
                    if (hitCollided && hit.transform.CompareTag("Player"))
                    {
                        if (!InAttackRange())
                        {
                            ChangeState(States.MOVE_TO_TARGET);
                            return;
                        }
                    }
                }
            }
        }
        
        if (canAttack && InAttackRange())
        {
            if (attacksTargetWOSeeingIt)
                ChangeState(States.ATTACK);
            else
            {
                RaycastHit hit;
                bool hitCollided = Physics.Raycast(transform.position, (playerRef.position - transform.position).normalized, out hit, distToPlayer, layerMask);
                if (hitCollided && hit.transform.CompareTag("Player"))
                {
                    if (Vector3.Angle(transform.forward, playerRef.position - transform.position) < ATTACK_ANGLE_THRESHOLD)
                        ChangeState(States.ATTACK);
                    else idleWaitTimer = -1;
                    return;
                }
            }
        }
        
        if (MakesRandomMoves)
        {
            if (HaveRandomMovesAvailable) ChangeState(States.RANDOM_MOVEMENT);
            else EndRndMovesBehaviour();
            return;
        }

    }
    protected virtual void RandomMovementUpdate()
    {
        rndMoveTimer += Time.deltaTime;
        moveDir = (rndTarget - transform.position).normalized;
        MoveRB(moveDir, ((actualMoveSpeed * 3f) / 4f) * speedMultiplier);

        if (stopRndMoveWhenSeeingTarget)
        {
            RaycastHit hit;
            bool hitCollided = Physics.Raycast(transform.position, (playerRef.position - transform.position).normalized, out hit,
                Vector3.Distance(transform.position, playerRef.position), layerMask);
            if (hitCollided && hit.transform.CompareTag("Player"))
            {
                if (movesToTarget)
                {
                    ChangeState(States.MOVE_TO_TARGET);
                    return;
                }
                else if (InAttackRange())
                {
                    ChangeState(States.ATTACK);
                    return;
                }
            }
        }

        if ((lookPoint != null && Physics.Raycast(lookPoint.position, lookPoint.forward, 3f))
            || Vector3.Distance(transform.position, rndTarget) < ATTACK_DISTANCE_THRESHOLD
            || rndMoveTimer >= rndMoveWait)
        {
            //Debug.Log("ExitRandomMovement");
            ChangeState(States.IDLE);
        }
    }
    protected virtual void MoveToTargetUpdate()
    {
        if(!disableAutoGravity)
            rb.useGravity = false;

        Vector3 targetMoveDir = (playerRef.position - transform.position).normalized;
        MoveRB(targetMoveDir, actualMoveSpeed * speedMultiplier);


        if (Vector3.Distance(transform.position, playerRef.position) > playerStopDetectionDistance)
            ChangeState(States.IDLE);

        if (!movesToTargetWOSeeingIt)
        {
            RaycastHit hit;
            bool hitCollided = Physics.Raycast(transform.position, (playerRef.position - transform.position).normalized, out hit, playerStopDetectionDistance, layerMask);
            if (!hitCollided || !hit.transform.CompareTag("Player"))
                ChangeState(States.IDLE);
        }

        if (canAttack && InAttackRange())
            ChangeState(States.ATTACK);
    }
    protected virtual void AttackUpdate()
    {
        if (!canAttack) { ChangeState(States.IDLE); return; }

        //Debug.Log("Dbg Attacking");
        //if (endAttackFlag)
        //{
        //    if (needsToRest) ChangeState(States.REST);
        //    else ChangeState(States.IDLE);
        //}

        if (endsAttackWhenTargetOutOfRange 
            && !InAttackRange())
        {
            ChangeState(States.IDLE);
            return;
        }
    }
    protected virtual void RestUpdate()
    {
        restTimer -= Time.deltaTime;
        //Debug.Log("Dbg Resting");
        if (restTimer <= 0f) ChangeState(States.IDLE);
    }
    //protected virtual void DamageUpdate()
    //{
    //    damageTimer -= Time.deltaTime;

    //    if (enemyLife.isDead)
    //    {
    //        ChangeState(States.DEATH);
    //        return;
    //    }

    //    if (damageTimer <= 0)
    //    {
    //        float distToPlayer = Vector3.Distance(transform.position, player.position);
    //        if (InAttackRange()) ChangeState(States.ATTACK);
    //        else if (distToPlayer <= playerDetectionDistance) ChangeState(States.MOVE_TO_TARGET);
    //        else ChangeState(States.IDLE);
    //    }
    //}
    protected virtual void DeathUpdate()
    {
        damageTimer -= Time.deltaTime;

        if (enemyMesh != null)
        {
            if (enemyMesh.material.color.a > 0)
            {
                enemyMesh.material.color -= new Color(0, 0, 0, Time.deltaTime);
            }
        }
        else
        {
            if (enemyMeshTmp.material.color.a > 0)
            {
                enemyMeshTmp.material.color -= new Color(0, 0, 0, Time.deltaTime);
            }
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
            DeathExit();
        }
    }
    #endregion Updates

    #region Starts
    protected virtual void IdleStart() { StopRB(stopForce); moveDir = Vector3.zero; idleWaitTimer = Random.Range(idleWait.x, idleWait.y); }
    protected virtual void RandomMovementStart()
    {
        float rndFactor = 4f;
        bool collided = true;
        int currTrials = 0, maxTrials = 10;
        while (collided && currTrials <= maxTrials)
        {
            rndTarget = transform.position + new Vector3(Random.Range(-rndFactor, rndFactor), 0f, Random.Range(-rndFactor, rndFactor));
            collided = Physics.BoxCast(transform.position, Vector3.one, 
                (rndTarget - transform.position).normalized, Quaternion.identity, Vector3.Distance(transform.position, rndTarget), layerMask);
            currTrials++;
        }
        //Debug.Log("Num of Trials: " + currTrials);
        actualMoveSpeed = baseMoveSpeed;
        rndMoveTimer = 0f;
        if (collided) ChangeState(States.IDLE);
    }
    protected virtual void MoveToTargetStart() { if (numOfRndMoves > 0) rndMovesDone = 0; actualMoveSpeed = playerFoundSpeed; }
    protected virtual void AttackStart() { if (numOfRndMoves > 0) rndMovesDone = 0; }
    protected virtual void RestStart() { restTimer = Random.Range(restWait.x, restWait.y); canMove = canRotate = false; }
    //protected virtual void DamageStart() { damageTimer = baseDamageTimer; }
    protected virtual void DeathStart()
    {
        GetComponent<Collider>().enabled = false;
        touchBodyDamageData.GetComponent<Collider>().enabled = false;

        if (!disableAutoGravity)
            rb.useGravity = false;

        damageTimer = baseDeathTime;
        if(enemyMesh != null) enemyMesh.material = transparentMat;
        else enemyMeshTmp.material = transparentMat;

        if (zoneSystem != null)
        {
            zoneSystem.enemiesQuantity -= 1;
            zoneSystem = null;
        }

        DropMoney();
    }
    #endregion Starts

    #region Exits
    protected virtual void IdleExit() { }
    protected virtual void RandomMovementExit() { if (numOfRndMoves > 0) rndMovesDone++; }
    protected virtual void MoveToTargetExit() { }
    protected virtual void AttackExit() { }
    protected virtual void RestExit() { canMove = canRotate = true; }
    protected virtual void DamageExit() { }
    protected virtual void DeathExit() { Destroy(gameObject); }
    #endregion Exits


    #region Misc
    public void ActivateDamage()
    {
        if (enemyLife.isDead)
        {
            ChangeState(States.DEATH);
        }
        else if (canEnterDamageState && baseDamageTimer > 0f)
        {
            StartCoroutine(ActivateDamage_Cor(baseDamageTimer));
        }

    }
    protected virtual IEnumerator ActivateDamage_Cor(float _dmgTimer)
    {
        dmgActivated = true;
        canMove = false;
        StopRB(stopForce);
        yield return new WaitForSeconds(_dmgTimer);
        canMove = true;
        dmgActivated = false;
    }
    void DropMoney()
    {
        int dropAmount = Random.Range(moneyDropped.x, moneyDropped.y) / 10;
        float distRange = 0.5f;
        RoomEnemyManager roomManager = GetComponentInParent<RoomEnemyManager>();
        for(int i = 0; i < dropAmount; i++)
        {
            Vector3 rndPos = transform.position + new Vector3(Random.Range(distRange, -distRange), Random.Range(distRange, -distRange), Random.Range(distRange, -distRange));
            Rigidbody dropRb = Instantiate(coinPrefab, rndPos, Random.rotation).GetComponent<Rigidbody>();
            dropRb.AddExplosionForce(Random.Range(50f, 600f), transform.position, distRange);
            dropRb.GetComponent<CoinScript>().roomManager = roomManager;
        }

        //AUDIO
        AudioManager.instance.PlayOneShot(FMODEvents.instance.enemyDeath, this.transform.position);
    }
    protected virtual void EndRndMovesBehaviour() { rndMovesDone = 0; }

    private Vector3 ClampVector(Vector3 _originalVec, Vector3 _minVec, Vector3 _maxVec)
    {
        return new Vector3(
            Mathf.Clamp(_originalVec.x, _minVec.x, _maxVec.x),
            Mathf.Clamp(_originalVec.y, _minVec.y, _maxVec.y),
            Mathf.Clamp(_originalVec.z, _minVec.z, _maxVec.z)
        );
    }

    protected void MoveRB(Vector3 _moveDir, float _moveForce, ForceMode _forceMode = ForceMode.Force)
    {
        if (canMove)
            rb.velocity = _moveDir * _moveForce;
    }
    protected void StopRB(float _speedReduction = DEFAULT_SPEED_REDUCTION)
    {
        rb.velocity = new Vector3(rb.velocity.x / _speedReduction, rb.velocity.y, rb.velocity.z / _speedReduction);
    }
    protected void SetVelocityLimit(Vector3 _minSpeed, Vector3 _maxSpeed)
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
    protected bool InAttackRange()
    {
        Vector2 screenPosition = Camera.main.WorldToScreenPoint(transform.position);
        return screenPosition.x > -Screen.width * ATTACK_MARGIN && screenPosition.x < Screen.width * (1f + ATTACK_MARGIN) 
            && screenPosition.y > -Screen.height * ATTACK_MARGIN && screenPosition.y < Screen.height * (1f + ATTACK_MARGIN)
            && (attackRange < 0 || Vector3.Distance(transform.position, playerRef.position) <= attackRange);
    }
    #endregion Misc


    void OnCollisionEnter(Collision col)
    {
        CollisionEnterEvent(col);
    }
    protected virtual void CollisionEnterEvent(Collision col)
    {
        if (!disableAutoGravity && col.gameObject.CompareTag("floor"))
            rb.useGravity = false;

        //if (col.gameObject.CompareTag("Player"))
        //    col.transform.GetComponent<LifeSystem>().Damage(dmgOnTouch, ElementsManager.Elements.NORMAL);
    }

    void OnCollisionExit(Collision col)
    {
        CollisionExitEvent(col);
    }
    protected virtual void CollisionExitEvent(Collision col)
    {
        if (!disableAutoGravity && col.gameObject.CompareTag("floor"))
            rb.useGravity = true;

    }

}
