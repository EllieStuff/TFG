using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShieldSafetyCollider : MonoBehaviour
{
    [SerializeField] Collider protectedEntityCollider;


    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("SwordRegion") || other.CompareTag("Weapon"))
        {
            Physics.IgnoreCollision(other, protectedEntityCollider, false);
        }
    }

}
