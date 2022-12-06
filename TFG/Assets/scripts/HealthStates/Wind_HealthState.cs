using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Wind_HealthState : HealthState
{
    //Nota: el efecto del viento puede que fuera mejor controlarlo desde el prefab de la habilidad como tal como tal, pero ya veremos
    public enum WindBehaviour { PUSH_TOWARDS_DIRECTION, PUSH_TOWARDS_POINT, PUSH_AGAINST_POINT }

    [SerializeField] internal WindBehaviour windBehaviour;
    [SerializeField] internal Vector3 windDirection, windPoint;
    [SerializeField] internal float windForce = 10f;


    public Wind_HealthState()
    {
        state = HealthState.Effect.WIND;
    }
    public override void Init(LifeSystem _lifeSystem)
    {
        base.Init(_lifeSystem);

        name = "Wind State";
        state = HealthState.Effect.WIND;
        effectDuration = 10.0f; //Depende de la carta

        compatibilityMap_DmgMultipliers.Add(Effect.BURNED, 0.5f);

        //Has no compatibilities with this or other effects

    }

    public override void StartEffect()
    {
        base.StartEffect();

        lifeSystem.StartCoroutine(WindCoroutine());
    }

    public override void EndEffect()
    {
        lifeSystem.StopCoroutine(WindCoroutine());
        base.EndEffect();
    }


    IEnumerator WindCoroutine()
    {
        Rigidbody affectedEntityRb = lifeSystem.GetComponent<Rigidbody>();
        float endEffectTimeStamp = Time.timeSinceLevelLoad + effectDuration;

        while (Time.timeSinceLevelLoad < endEffectTimeStamp)
        {
            switch (windBehaviour)
            {
                case WindBehaviour.PUSH_TOWARDS_DIRECTION:
                    affectedEntityRb.AddForce(windDirection.normalized * windForce * Time.deltaTime, ForceMode.Acceleration);
                    break;

                case WindBehaviour.PUSH_TOWARDS_POINT:
                    windDirection = (windPoint - affectedEntityRb.position).normalized;
                    affectedEntityRb.AddForce(windDirection.normalized * windForce * Time.deltaTime, ForceMode.Acceleration);
                    break;

                case WindBehaviour.PUSH_AGAINST_POINT:
                    windDirection = (affectedEntityRb.position - windPoint).normalized;
                    affectedEntityRb.AddForce(windDirection.normalized * windForce * Time.deltaTime, ForceMode.Acceleration);
                    break;


                default:
                    break;
            }
            yield return new WaitForEndOfFrame();
        }

    }


}
