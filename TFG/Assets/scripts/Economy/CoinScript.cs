using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CoinScript : MonoBehaviour
{
    const int COIN_VALUE = 10;
    const float PLAYER_ABSORB_DISTANCE = 2f;
    const float END_MOVEMENT_TIMER = 0.5f, GO_TO_PLAYER_TIMER = 2f, ROOM_EMPTY_TIMER = 0.1f, DESTROY_DIST = 200f;

    public static int CoinsInScene = 0;

    float timer = 0;
    float moveSpeed = 15f;
    Vector3 lastPos = Vector3.zero;
    bool rbActive = true;
    Transform playerRef;
    internal RoomEnemyManager roomManager;

    // Start is called before the first frame update
    void Start()
    {
        playerRef = GameObject.FindGameObjectWithTag("Player").transform;
        CoinsInScene++;
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
                if (timer >= END_MOVEMENT_TIMER)
                {
                    DeactivateRb();
                }
            }
        }


        timer += Time.deltaTime;
        if (timer > GO_TO_PLAYER_TIMER || (!roomManager.IsBossRoom && !roomManager.HasEnemiesRemainging() && timer > ROOM_EMPTY_TIMER) 
            || Vector3.Distance(transform.position, playerRef.position) < PLAYER_ABSORB_DISTANCE)
        {
            if (rbActive) DeactivateRb();
            Vector3 moveDir = (playerRef.position - transform.position).normalized;
            transform.position += moveDir * moveSpeed * Time.deltaTime;
        }

        if(Vector3.Distance(transform.position, playerRef.position) > DESTROY_DIST)
        {
            CoinsInScene--;
            if (CoinsInScene < 0) CoinsInScene = 0;
            Destroy(gameObject);
        }

    }


    void DeactivateRb()
    {
        rbActive = false;
        timer = 0f;
        Destroy(GetComponent<Rigidbody>());
        GetComponent<Collider>().isTrigger = true;
    }


    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            MoneyManager.AddMoney(COIN_VALUE);

            //AUDIO
            AudioManager.instance.PlayOneShot(FMODEvents.instance.uiGetCoin, this.transform.position);

            CoinsInScene--;
            if (CoinsInScene < 0) CoinsInScene = 0;
            Destroy(gameObject);
        }
    }
}
