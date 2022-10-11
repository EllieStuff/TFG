using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    const float INPUT_THRESHOLD = 0.3f;

    [SerializeField] float moveForce = 4;
    [SerializeField] float rotSpeed = 4;
    [SerializeField] Vector3 maxSpeed;

    Rigidbody rb;
    [HideInInspector] public Vector3 moveDir = Vector3.zero;
    bool moving = false;

    // Start is called before the first frame update
    void Start()
    {
        rb = GetComponent<Rigidbody>();
    }

    // Update is called once per frame
    void Update()
    {
        float horizontalInput = Input.GetAxis("Horizontal");
        float verticalInput = Input.GetAxis("Vertical");
        moveDir = new Vector3(horizontalInput, 0, verticalInput).normalized;
        //transform.Translate(moveDir * moveSpeed * Time.deltaTime, Space.World);

        if (Mathf.Abs(verticalInput) > INPUT_THRESHOLD || Mathf.Abs(horizontalInput) > INPUT_THRESHOLD)
        {
            moving = true;
            rb.AddForce(moveDir * moveForce /** Time.deltaTime*/, ForceMode.Force);
            rb.velocity = ClampVector(rb.velocity, -maxSpeed, maxSpeed);
        }
        else if(moving)
        {
            moving = false;
            rb.velocity = rb.velocity / 2f;
        }

        if (moveDir != Vector3.zero)
        {
            Quaternion targetRot = Quaternion.LookRotation(moveDir, Vector3.up);
            transform.rotation = Quaternion.RotateTowards(transform.rotation, targetRot, rotSpeed * Time.deltaTime);
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

}
