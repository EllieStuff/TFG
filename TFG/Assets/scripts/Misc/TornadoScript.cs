using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TornadoScript : MonoBehaviour
{
    [SerializeField] float tornadoDamage;
    [SerializeField] float suctionSpeed;
    [SerializeField] AudioManager audioManager;
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
            GameObject otherGO = otherTransform.gameObject;
            LifeSystem life = otherGO.GetComponent<LifeSystem>();

            if (!audioManager.IsPlayingSound() && life.currLife > 0)
                audioManager.PlaySound();

            life.Damage(tornadoDamage, state);
            otherGO.GetComponent<BaseEnemyScript>().ChangeState(BaseEnemyScript.States.DAMAGE);
        }
    }
}
