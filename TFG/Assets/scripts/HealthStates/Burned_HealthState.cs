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

        compatibilityMap_DmgMultipliers.Add(Effect.ELECTROCUTED, 0.5f);
        compatibilityMap_DmgMultipliers.Add(Effect.WIND, 0.5f);

        compatibilityMap_FinalEffects.Add(Effect.BURNED, new Burned_HealthState());
        compatibilityMap_FinalEffects.Add(Effect.WET, new HealthState());
        compatibilityMap_FinalEffects.Add(Effect.COLD, new HealthState());

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
            //lifeSystem.Damage(dmg, ElementsManager.Elements.NORMAL);
            //ToDo: stun player/enemies
            yield return new WaitForSeconds(dmgFreq);
        }
        while (Time.timeSinceLevelLoad < finishEffectTimeStamp);
    }


}
