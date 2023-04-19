using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Criticon_PassiveSkill : PassiveSkill_Base
{

    const float CRITICAL_CHANGE_IMPROVE = 0.1f;

    public Criticon_PassiveSkill()
    {
        skillType = SkillType.CRITICAL_CHANCE;
        maxLevel = 3;
        basePrice = 3500;
        priceInc = 1000;
        name = "Critical improve";
        //initialDescription = "Improves your critical hit chance!";
        initialDescription = "Improves your critical hit chance by 10%!";
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
        PlayerAttack playeAttack = playerRef.GetComponent<PlayerAttack>();

        playeAttack.critChancePercentage += CRITICAL_CHANGE_IMPROVE;
    }

}
