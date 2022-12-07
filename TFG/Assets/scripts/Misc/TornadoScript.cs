using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TornadoScript : MonoBehaviour
{
    [SerializeField] float tornadoDamage;
    [SerializeField] float suctionSpeed;
    HealthState state;

    private void Start()
    {
        state = new HealthState();
    }

    private void OnTriggerStay(Collider other)
    {
        if(other.CompareTag("Enemy"))
        {
            Transform otherTransform = other.transform;
            otherTransform.position = Vector3.Lerp(otherTransform.position, transform.position, Time.deltaTime * suctionSpeed);
            otherTransform.gameObject.GetComponent<LifeSystem>().Damage(tornadoDamage, state);
        }
    }
}
