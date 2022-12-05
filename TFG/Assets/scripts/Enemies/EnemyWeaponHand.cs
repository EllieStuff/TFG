using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyWeaponHand : MonoBehaviour
{
    [SerializeField] internal bool isTouchingPlayer;

    private void Start()
    {
        
    }


    private void OnTriggerEnter(Collider other)
    {
        if (other.tag.Equals("Player"))
            isTouchingPlayer = true;
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.tag.Equals("Player"))
            isTouchingPlayer = false;
    }
}
