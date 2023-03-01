using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Conjure_PassiveSkill : PassiveSkill_Base
{
    float speedIncrease = 0.2f;

    public Conjure_PassiveSkill()
    {
        skillType = SkillType.IMPROVE_LIFE;
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
        
        if(playerAttack.changeAttackDelay > 0.5f)
            playerAttack.changeAttackDelay -= 0.2f;
    }

}
