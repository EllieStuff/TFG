using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BaseEnemyScript : MonoBehaviour
{
    public enum States { IDLE, MOVE_TO_TARGET, ATTACK }

    [SerializeField] float rotSpeed = 4;
    [SerializeField] float playerDetectionDistance;
    [SerializeField] float enemyStartAttackDistance;
    [SerializeField] float moveSpeed;
    [SerializeField] Vector3 maxSpeed;

    States state = States.IDLE;
    Rigidbody rb;
    Transform player;
    [HideInInspector] public Vector3 moveDir = Vector3.zero;

    // Start is called before the first frame update
    void Start()
    {
        Start_Call();
    }
    internal virtual void Start_Call()
    {
        rb = GetComponent<Rigidbody>();
        player = GameObject.FindGameObjectWithTag("Player").transform;
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

        moveDir = NormalizeDirection(new Vector3(rb.velocity.x, 0, rb.velocity.z));

        if (moveDir != Vector3.zero)
        {
            Quaternion targetRot = Quaternion.LookRotation(moveDir, Vector3.up);
            transform.rotation = Quaternion.RotateTowards(transform.rotation, targetRot, rotSpeed * Time.deltaTime);
            //Debug.Log(transform.rotation);
        }
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

            default:
                Debug.LogWarning("State not found");
                break;
        }
    }

    internal virtual void IdleUpdate()
    {
        if (Vector3.Distance(transform.position, player.position) <= playerDetectionDistance)
            state = States.MOVE_TO_TARGET;
    }
    internal virtual void MoveToTargetUpdate()
    {
        rb.velocity -= (transform.position - player.position) * Time.deltaTime * moveSpeed;
        rb.velocity = ClampVector(rb.velocity, -maxSpeed, maxSpeed);

        if (Vector3.Distance(transform.position, player.position) > playerDetectionDistance)
            state = States.IDLE;

        if (Vector3.Distance(transform.position, player.position) <= enemyStartAttackDistance)
            state = States.ATTACK;
    }
    internal virtual void AttackUpdate()
    {
        if (Vector3.Distance(transform.position, player.position) > enemyStartAttackDistance)
            ChangeState(States.MOVE_TO_TARGET);
    }
    
    internal virtual void IdleStart() { }
    internal virtual void MoveToTargetStart() { }
    internal virtual void AttackStart() { }
    internal virtual void IdleExit() { }
    internal virtual void MoveToTargetExit() { }
    internal virtual void AttackExit() { }

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

}
