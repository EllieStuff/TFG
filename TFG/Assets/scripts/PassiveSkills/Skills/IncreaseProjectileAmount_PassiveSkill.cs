using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class IncreaseProjectileAmount_PassiveSkill : PassiveSkill_Base
{
    int extraProjectiles = 0;

    public IncreaseProjectileAmount_PassiveSkill()
    {
        skillType = SkillType.INCREASE_PROJECTILE_AMOUNT;
        maxLevel = 3;
        name = "Double Projectile";
        description = "You can shoot another projectile at the same time!";
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
        extraProjectiles++;
        PlayerAttack playeAttack = playerRef.GetComponent<PlayerAttack>();
        playeAttack.extraProjectiles = extraProjectiles;
    }

}
