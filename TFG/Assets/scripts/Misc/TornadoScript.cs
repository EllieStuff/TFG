using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TornadoScript : MonoBehaviour
{
    [SerializeField] float tornadoDuration = 4f;
    [SerializeField] float tornadoDamage = 0.5f;
    [SerializeField] float dmgFrequency = 0.5f;
    [SerializeField] float suctionSpeed;
    [SerializeField] AudioManager audioManager;
    //HealthState windState = new Wind_HealthState();

    private void Start()
    {

    }


    private void OnTriggerEnter(Collider other)
    {
        if(other.CompareTag("Enemy"))
        {
            other.GetComponent<BaseEnemyScript>().StartCoroutine(TornadoCoroutine(other.transform));
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Enemy"))
        {
            other.GetComponent<BaseEnemyScript>().StopCoroutine(TornadoCoroutine(other.transform));
        }
    }


    IEnumerator TornadoCoroutine(Transform _enemy)
    {
        float endTornadoTimeStamp = Time.realtimeSinceStartup + tornadoDuration;
        float dmgTimer = dmgFrequency;

        LifeSystem life = _enemy.gameObject.GetComponent<LifeSystem>();
        HealthState windState = new Wind_HealthState();
        windState.Init(life);
        windState.effectDuration = tornadoDuration - 0.1f;
        life.Damage(tornadoDamage, windState);
        _enemy.gameObject.GetComponent<BaseEnemyScript>().ChangeState(BaseEnemyScript.States.DAMAGE);

        while (Time.realtimeSinceStartup < endTornadoTimeStamp) {
            _enemy.position = Vector3.Lerp(_enemy.position, transform.position, Time.deltaTime * suctionSpeed);           

            dmgTimer -= Time.deltaTime;
            if (dmgTimer <= 0)
            {
                if (!audioManager.IsPlayingSound() && life.currLife > 0)
                    audioManager.PlaySound();

                dmgTimer = dmgFrequency;
                life.Damage(tornadoDamage, null);
                _enemy.gameObject.GetComponent<BaseEnemyScript>().ChangeState(BaseEnemyScript.States.DAMAGE);
            }

            yield return new WaitForEndOfFrame();
        }

    }

}
