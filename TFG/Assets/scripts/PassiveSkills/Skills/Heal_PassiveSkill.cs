using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Heal_PassiveSkill : PassiveSkill_Base
{
    float lifePercentageToHeal = 60f;

    public Heal_PassiveSkill()
    {
        skillType = SkillType.HEAL;
        maxLevel = -1;
        name = "Heal";
        description = "Heals your wounds";
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
        float actualLifeToHeal = lifePercentageToHeal * 100f / playerLife.maxLife;
        playerLife.AddLife(actualLifeToHeal);
    }

}
