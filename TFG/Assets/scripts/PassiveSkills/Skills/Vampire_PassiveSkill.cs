using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Vampire_PassiveSkill : PassiveSkill_Base
{

    public Vampire_PassiveSkill()
    {
        skillType = SkillType.INCREASE_PROJECTILE_AMOUNT;
        maxLevel = 5;
        name = "Vampire";
        description = "Steal enemy life by damaging them with critical elements!";
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
        playerAttack.stealLifeEnabled = true;
        playerAttack.stealLifePercentage += 0.05f;
    }

}
