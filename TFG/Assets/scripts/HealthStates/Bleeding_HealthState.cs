using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bleeding_HealthState : HealthState
{
    [SerializeField]
    float
        dmgFreq = 1f,
        dmg = 2;


    public Bleeding_HealthState()
    {
        state = HealthState.Effect.BLEEDING;
    }
    public override void Init(LifeSystem _lifeSystem)
    {
        base.Init(_lifeSystem);

        state = HealthState.Effect.BURNED;
        effectDuration = 20.0f;

        //No dmg multiplying 

        //No compatibilities with this or other effects

    }

    public override void StartEffect()
    {
        base.StartEffect();
        lifeSystem.StartCoroutine(BleedingEffectCoroutine());

    }

    public override void EndEffect()
    {
        base.EndEffect();
        lifeSystem.StopCoroutine(BleedingEffectCoroutine());

        lifeSystem.dmgInc = 1.0f;
    }


    IEnumerator BleedingEffectCoroutine()
    {
        float finishEffectTimeStamp = Time.timeSinceLevelLoad + effectDuration;
        do
        {
            lifeSystem.Damage(dmg, null);
            yield return new WaitForSeconds(dmgFreq);
        }
        while (Time.timeSinceLevelLoad < finishEffectTimeStamp);
    }


}
