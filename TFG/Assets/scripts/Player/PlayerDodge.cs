using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerDodge : MonoBehaviour
{
    [SerializeField] float dodgeForce = 30;
    [SerializeField] internal float dodgeRechargeDelay = 1f;
    [SerializeField] PlayerHUD sprintHUD;

    Rigidbody rb;
    internal float dodgeRechargeTimer = 0f;

    // Start is called before the first frame update
    void Start()
    {
        rb = GetComponent<Rigidbody>();
    }

    // Update is called once per frame
    void Update()
    {
        Debug.DrawLine(transform.position, transform.forward * 20, Color.green);
        if (Input.GetKeyDown(KeyCode.LeftShift) && dodgeRechargeTimer <= 0)
        {
            sprintHUD.ShakeBar();
            rb.AddForce(transform.forward * dodgeForce, ForceMode.Impulse);
            dodgeRechargeTimer = dodgeRechargeDelay;
        }
        else if(dodgeRechargeTimer > 0)
        {
            dodgeRechargeTimer -= Time.deltaTime;
        }
    }

    //private void OnDrawGizmos()
    //{
    //    Debug.DrawLine(transform.position, mov.LookDir * 20, Color.green);
    //}
}
