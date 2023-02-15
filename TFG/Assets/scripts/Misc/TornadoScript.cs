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


    float timer;
    bool destroyOrder = false;
    //bool tornadoCorroutineStarted;

    private void Start()
    {
        timer = tornadoDuration;
        //tornadoCorroutineStarted = false;
        transform.parent = null;
    }

    private void Update()
    {
        if (timer > 0)
            timer -= Time.deltaTime;

        if(timer <= 0 && !destroyOrder)
        {
            destroyOrder = true;
            Destroy(gameObject, 0.1f);
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if(other.CompareTag("Enemy"))
        {
            //tornadoCorroutineStarted = true;
            other.GetComponent<BaseEnemyScript>().StartCoroutine(TornadoCoroutine(other.transform));
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Enemy"))
        {
            //tornadoCorroutineStarted = false;
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
        //life.Damage(tornadoDamage, windState);
        _enemy.gameObject.GetComponent<BaseEnemyScript>().ChangeState(BaseEnemyScript.States.DAMAGE);

        while (Time.realtimeSinceStartup < endTornadoTimeStamp) 
        {
            if (destroyOrder) yield break;

            if (_enemy == null) break;
            else _enemy.position = Vector3.Lerp(_enemy.position, transform.position, Time.deltaTime * suctionSpeed);           

            dmgTimer -= Time.deltaTime;
            if (dmgTimer <= 0)
            {
                //yield return new WaitForEndOfFrame();

                if (!audioManager.IsPlayingSound() && life.currLife > 0)
                    audioManager.PlaySound();

                dmgTimer = dmgFrequency;

                //if(life != null)
                    //life.Damage(tornadoDamage, ElementsManager.Elements.NORMAL);

                if (_enemy != null)
                    _enemy.gameObject.GetComponent<BaseEnemyScript>().ChangeState(BaseEnemyScript.States.DAMAGE);

                if (_enemy == null)
                    break;
            }

            yield return new WaitForEndOfFrame();
        }

        //Destroy(gameObject);
    }

}
