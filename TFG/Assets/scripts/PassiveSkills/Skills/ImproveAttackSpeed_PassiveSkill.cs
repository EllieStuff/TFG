using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ImproveAttackSpeed_PassiveSkill : PassiveSkill_Base
{
    const float DELAY_DECREASE = 0.15f;

    public ImproveAttackSpeed_PassiveSkill()
    {
        skillType = SkillType.IMPROVE_ATTACK_SPEED;
        maxLevel = 3;
        appearRatio = 0.6f;
        basePrice = 30000;
        priceInc = 14000;
        name = "Quicker Attack";
        //initialDescription = "Your attack rate gets quicker!";
        initialDescription = "Your attack rate gets 20% quicker!";
        improvementDescription = initialDescription;
    }

    public override void Init(Transform _playerRef)
    {
        base.Init(_playerRef);
        AddLevelEvent();
    }


    public override void Update_Call()
    {
        base.Update_Call();
    }


    protected override void AddLevelEvent()
    {
        base.AddLevelEvent();
        PlayerAttack playeAttack = playerRef.GetComponent<PlayerAttack>();
        playeAttack.attackDelay -= DELAY_DECREASE;
    }

}
