using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ImproveAttackDamage_PassiveSkill : PassiveSkill_Base
{
    float dmgIncrease = 5f;

    public ImproveAttackDamage_PassiveSkill()
    {
        skillType = SkillType.IMPROVE_ATTACK_DAMAGE;
        maxLevel = 10;
        name = "Attack Damage Improve";
        description = "Your attacks will apply more damage!";
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
        playeAttack.dmgIncrease += dmgIncrease;
    }

}
