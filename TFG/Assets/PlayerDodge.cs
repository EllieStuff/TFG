using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerDodge : MonoBehaviour
{
    [SerializeField] float dodgeForce = 30;

    Rigidbody rb;
    PlayerMovement mov;


    // Start is called before the first frame update
    void Start()
    {
        rb = GetComponent<Rigidbody>();
        mov = GetComponent<PlayerMovement>();
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.LeftShift))
        {
            rb.AddForce(mov.LookDir * dodgeForce, ForceMode.Impulse);
        }
    }

    private void OnDrawGizmos()
    {
        Debug.DrawLine(transform.position, mov.LookDir * 20, Color.green);
    }
}
