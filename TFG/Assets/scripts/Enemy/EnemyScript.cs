using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyScript : MonoBehaviour
{
    private enum States { IDLE, MOVE_TO_TARGET, ATTACK }

    [SerializeField] float rotSpeed = 4;

    States stats = States.IDLE;
    Rigidbody rb;
    [HideInInspector] public Vector3 moveDir = Vector3.zero;

    // Start is called before the first frame update
    void Start()
    {
        rb = GetComponent<Rigidbody>();
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        moveDir = NormalizeDirection(rb.velocity);

        switch(stats)
        {
            case States.IDLE:

                break;
            case States.MOVE_TO_TARGET:

                break;
            case States.ATTACK:

                break;
        }

        if (moveDir != Vector3.zero)
        {
            Quaternion targetRot = Quaternion.LookRotation(moveDir, Vector3.up);
            transform.rotation = Quaternion.RotateTowards(transform.rotation, targetRot, rotSpeed * Time.deltaTime);
        }
    }

    Vector3 NormalizeDirection(Vector3 moveDir)
    {
        if (moveDir.x > 0)
            moveDir = new Vector3(1, moveDir.y, moveDir.z);
        if (moveDir.x < 0)
            moveDir = new Vector3(-1, moveDir.y, moveDir.z);

        if (moveDir.z > 0)
            moveDir = new Vector3(moveDir.x, moveDir.y, 1);
        if (moveDir.z < 0)
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
