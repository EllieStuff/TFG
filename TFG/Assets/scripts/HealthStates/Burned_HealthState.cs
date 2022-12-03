using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Burned_HealthState : HealthState
{
    [SerializeField] float
        defaultDmgInc = 1.0f,
        shieldDmgInc = 4.0f;

    [SerializeField] float
        dmgFreq = 7f,
        dmg = 5;


    public Burned_HealthState()
    {
        state = HealthState.Effect.BURNED;
    }
    public override void Init(LifeSystem _lifeSystem)
    {
        base.Init(_lifeSystem);

        name = "Burned State";
        state = HealthState.Effect.BURNED;
        effectDuration = 20.0f;

        electrocutedCompatibility_DmgMultiplier = 0.5f;
        windCompatibility_DmgMultiplier = 0.5f;

        burnedCompatibility_FinalEffect = new Burned_HealthState();
        wetCompatibility_FinalEffect = new HealthState();
        coldCompatibility_FinalEffect = new HealthState();
        //frozenCompatibility_FinalEffect = new Cold_HealthState();

    }

    public override void StartEffect()
    {
        base.StartEffect();
        lifeSystem.StartCoroutine(BurnedEffectCoroutine());

        if (lifeSystem.entityType == LifeSystem.EntityType.SHIELD)
            lifeSystem.dmgInc = shieldDmgInc;
        else
            lifeSystem.dmgInc = defaultDmgInc;

    }

    public override void EndEffect()
    {
        base.EndEffect();
        lifeSystem.StopCoroutine(BurnedEffectCoroutine());

        lifeSystem.dmgInc = 1.0f;
    }


    IEnumerator BurnedEffectCoroutine()
    {
        float finishEffectTimeStamp = Time.timeSinceLevelLoad + effectDuration;
        do
        {
            lifeSystem.Damage(dmg, null);
            //ToDo: stun player/enemies
            yield return new WaitForSeconds(dmgFreq);
        }
        while (Time.timeSinceLevelLoad < finishEffectTimeStamp);
    }


}
