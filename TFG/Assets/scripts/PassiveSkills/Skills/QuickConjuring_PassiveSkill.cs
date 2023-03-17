using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class QuickConjuring_PassiveSkill : PassiveSkill_Base
{
    const float SPEED_INCREASE = 0.4f;

    public QuickConjuring_PassiveSkill()
    {
        skillType = SkillType.QUICK_CONJURING;
        maxLevel = 3;
        name = "Agile Conjuration";
        initialDescription = "Speed up your element casting time!";
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
        PlayerAttack playerAttack = playerRef.GetComponent<PlayerAttack>();
        playerAttack.changeAttackDelay -= SPEED_INCREASE;
    }

}
