using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CoinScript : MonoBehaviour
{
    const int COIN_VALUE = 10;

    float timer = 0, endMovementTime = 0.5f, goToPlayerTimer = 3f;
    float moveSpeed = 10f;
    Vector3 lastPos = Vector3.zero;
    bool rbActive = true;
    Transform playerRef;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if (rbActive)
        {
            if (lastPos != transform.position)
            {
                timer = 0f;
                lastPos = transform.position;
            }
            else
            {
                timer += Time.deltaTime;
                if(timer >= endMovementTime)
                {
                    rbActive = false;
                    timer = 0f;
                    playerRef = GameObject.FindGameObjectWithTag("Player").transform;
                    Destroy(GetComponent<Rigidbody>());
                    GetComponent<Collider>().isTrigger = true;
                }
            }
        }
        else
        {
            if(timer < goToPlayerTimer) timer += Time.deltaTime;
            else
            {
                Vector3 moveDir = (playerRef.position - transform.position).normalized;
                transform.position += moveDir * moveSpeed * Time.deltaTime;
            }
        }

    }


    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            other.GetComponent<MoneyManager>().AddMoney(COIN_VALUE);
            Destroy(gameObject);
        }
    }
}
