using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    [SerializeField] Transform lookAtPoint;
    [SerializeField] float moveForce = 4;
    [SerializeField] Vector3 maxSpeed;

    Rigidbody rb;
    internal Vector3 LookDir { get { return (lookAtPoint.position - transform.position).normalized; } }

    // Start is called before the first frame update
    void Start()
    {
        rb = GetComponent<Rigidbody>();
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKey(KeyCode.W))
        {
            rb.AddForce(Vector3.forward * moveForce, ForceMode.Force);
            //transform.rotation = Quaternion.FromToRotation(transform.rotation.eulerAngles, transform.position + Vector3.forward);
            transform.rotation = Quaternion.Lerp(transform.rotation, Quaternion.Euler(transform.position + LookDir), moveForce);
        }
        if (Input.GetKey(KeyCode.S))
        {
            rb.AddForce(Vector3.back * moveForce, ForceMode.Force);
            transform.rotation = Quaternion.Lerp(transform.rotation, Quaternion.Inverse(Quaternion.Euler(transform.position + LookDir)), moveForce);
            //transform.rotation = Quaternion.FromToRotation(transform.rotation.eulerAngles, transform.position + Vector3.back);
        }
        if (Input.GetKey(KeyCode.A))
        {
            rb.AddForce(Vector3.left * moveForce, ForceMode.Force);
            //transform.rotation = Quaternion.FromToRotation(transform.rotation.eulerAngles, transform.position + Vector3.left);
        }
        if (Input.GetKey(KeyCode.D))
        {
            rb.AddForce(Vector3.right * moveForce, ForceMode.Force);
            //transform.rotation = Quaternion.FromToRotation(transform.rotation.eulerAngles, transform.position + Vector3.right);
        }
        rb.velocity = ClampVector(rb.velocity, -maxSpeed, maxSpeed);
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
