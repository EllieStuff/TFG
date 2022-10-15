using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyScript : MonoBehaviour
{
    private enum States { IDLE, MOVE_TO_TARGET, ATTACK }

    [SerializeField] float rotSpeed = 4;
    [SerializeField] float playerDetectionDistance;
    [SerializeField] float enemyStartAttackDistance;
    [SerializeField] float moveSpeed;
    [SerializeField] Vector3 maxSpeed;

    States stats = States.IDLE;
    Rigidbody rb;
    Transform player;
    [HideInInspector] public Vector3 moveDir = Vector3.zero;

    // Start is called before the first frame update
    void Start()
    {
        rb = GetComponent<Rigidbody>();
        player = GameObject.Find("Player").transform;
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        StateMachine();

        moveDir = NormalizeDirection(new Vector3(rb.velocity.x, 0, rb.velocity.z));

        if (moveDir != Vector3.zero)
        {
            Quaternion targetRot = Quaternion.LookRotation(moveDir, Vector3.up);
            transform.rotation = Quaternion.RotateTowards(transform.rotation, targetRot, rotSpeed * Time.deltaTime);
            Debug.Log(transform.rotation);
        }
    }

    void StateMachine()
    {
        switch (stats)
        {
            case States.IDLE:
                //patrol
                if (Vector3.Distance(transform.position, player.position) <= playerDetectionDistance)
                    stats = States.MOVE_TO_TARGET;

                break;
            case States.MOVE_TO_TARGET:
                //approach to player
                rb.velocity -= (transform.position - player.position) * Time.deltaTime * moveSpeed;
                rb.velocity = ClampVector(rb.velocity, -maxSpeed, maxSpeed);

                if (Vector3.Distance(transform.position, player.position) > playerDetectionDistance)
                    stats = States.IDLE;

                if (Vector3.Distance(transform.position, player.position) <= enemyStartAttackDistance)
                    stats = States.ATTACK;

                break;
            case States.ATTACK:
                //attack

                if (Vector3.Distance(transform.position, player.position) > enemyStartAttackDistance)
                    stats = States.MOVE_TO_TARGET;

                break;
        }
    }

    Vector3 ClampVector(Vector3 _originalVec, Vector3 _minVec, Vector3 _maxVec)
    {
        return new Vector3(
            Mathf.Clamp(_originalVec.x, _minVec.x, _maxVec.x),
            Mathf.Clamp(_originalVec.y, _minVec.y, _maxVec.y),
            Mathf.Clamp(_originalVec.z, _minVec.z, _maxVec.z)
        );
    }

    Vector3 NormalizeDirection(Vector3 moveDir)
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

    void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.tag.Equals("floor"))
            rb.useGravity = false;
    }

    void OnCollisionExit(Collision collision)
    {
        if (collision.gameObject.tag.Equals("floor"))
            rb.useGravity = true;
    }
}
