using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerDodge : MonoBehaviour
{
    [SerializeField] float dodgeForce = 30;

    Rigidbody rb;


    // Start is called before the first frame update
    void Start()
    {
        rb = GetComponent<Rigidbody>();
    }

    // Update is called once per frame
    void Update()
    {
        Debug.DrawLine(transform.position, transform.forward * 20, Color.green);
        if (Input.GetKeyDown(KeyCode.LeftShift))
        {
            rb.AddForce(transform.forward * dodgeForce, ForceMode.Impulse);
        }
    }

    //private void OnDrawGizmos()
    //{
    //    Debug.DrawLine(transform.position, mov.LookDir * 20, Color.green);
    //}
}
