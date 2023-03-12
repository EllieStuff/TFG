using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class QuickConjuring_PassiveSkill : PassiveSkill_Base
{
    const float MAX_SPEED_INCREASE = 0.5f;
    float speedIncrease = 0.3f;

    public QuickConjuring_PassiveSkill()
    {
        skillType = SkillType.QUICK_CONJURING;
        maxLevel = -1;
        name = "Agile Conjuration";
        description = "Speed up your element casting time!";
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

        if (playerAttack.changeAttackDelay > MAX_SPEED_INCREASE)
        {
            playerAttack.changeAttackDelay -= speedIncrease;
            if (playerAttack.changeAttackDelay <= MAX_SPEED_INCREASE)
            {
                playerAttack.changeAttackDelay = MAX_SPEED_INCREASE;
                level = maxLevel = 1;
            }
        }
    }

}
