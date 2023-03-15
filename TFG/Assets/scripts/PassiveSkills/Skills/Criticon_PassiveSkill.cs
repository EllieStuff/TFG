using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Criticon_PassiveSkill : PassiveSkill_Base
{
    const float MAX_PERCENTAGE_IMPROVE = 0.03f * 5;

    public Criticon_PassiveSkill()
    {
        skillType = SkillType.CRITICAL_CHANCE;
        maxLevel = 3;
        name = "Critical improve";
        description = "Improve your critical hit chance!";
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

        if(playeAttack.critChancePercentage < MAX_PERCENTAGE_IMPROVE)
            playeAttack.critChancePercentage += 0.03f;
    }

}
