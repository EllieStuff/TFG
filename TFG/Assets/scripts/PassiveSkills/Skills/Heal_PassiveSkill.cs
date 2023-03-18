using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Heal_PassiveSkill : PassiveSkill_Base
{
    const float LIFE_PERCENTAGE_TO_HEAL = 0.65f;

    public Heal_PassiveSkill()
    {
        skillType = SkillType.HEAL;
        maxLevel = -1;
        name = "Heal";
        initialDescription = "Heals your wounds!";
        improvementDescription = initialDescription;
    }

    public override void Init(Transform _playerRef)
    {
        base.Init(_playerRef);
        AddLevelEvent();
    }


    public override void UpdateCall()
    {
        base.UpdateCall();
    }


    internal override void AddLevelEvent()
    {
        base.AddLevelEvent();
        LifeSystem playerLife = playerRef.GetComponent<LifeSystem>();
        float actualLifeToHeal = playerLife.maxLife * LIFE_PERCENTAGE_TO_HEAL;
        playerLife.AddLife(actualLifeToHeal);
    }

}
