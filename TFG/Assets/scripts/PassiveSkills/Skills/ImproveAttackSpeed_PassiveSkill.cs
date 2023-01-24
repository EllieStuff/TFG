using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ImproveAttackSpeed_PassiveSkill : PassiveSkill_Base
{
    float delayDecrease = 0.4f;

    public ImproveAttackSpeed_PassiveSkill()
    {
        skillType = SkillType.IMPROVE_ATTACK_SPEED;
        maxLevel = 5;
        name = "Attack Speed Improve";
        description = "Your attack speed gets quicker!";
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
        playeAttack.attackDelay -= delayDecrease;
    }

}
