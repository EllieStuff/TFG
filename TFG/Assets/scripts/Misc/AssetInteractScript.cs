using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AssetInteractScript : MonoBehaviour
{
    Rigidbody rb;
    const float VERTICAL_SPEED = 4;
    const float LERP_SPEED = 2;
    Vector3 impulseVector;
    Vector3 globalFixedPos;

    void Start()
    {
        globalFixedPos = transform.position;
        impulseVector = new Vector3(0, VERTICAL_SPEED, 0);
        rb = GetComponent<Rigidbody>();
    }

    private void FixedUpdate()
    {
        if (rb.velocity.x != 0 || rb.velocity.z != 0)
            rb.velocity = new Vector3(0, rb.velocity.y, 0);

        Vector3 targetVector = new Vector3(globalFixedPos.x, transform.position.y, globalFixedPos.z);
        transform.position = Vector3.Lerp(transform.position, targetVector, Time.deltaTime * LERP_SPEED);
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.name.Contains("Projectile"))
            rb.velocity = impulseVector;
    }
}
