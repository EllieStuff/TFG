using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Enemy_CacoRato : BaseEnemyScript
{

    internal override void Start_Call() { base.Start_Call(); }

    internal override void Update_Call() { base.Update_Call(); }

    internal override void FixedUpdate_Call() { base.FixedUpdate_Call(); }


    internal override void IdleUpdate()
    {
        base.IdleUpdate();

    }
    internal override void MoveToTargetUpdate()
    {
        base.MoveToTargetUpdate();

    }
    internal override void AttackUpdate()
    {
        base.AttackUpdate();

    }


    internal override void IdleStart() { base.IdleStart(); }
    internal override void MoveToTargetStart() { base.MoveToTargetStart(); }
    internal override void AttackStart() { base.AttackStart(); }


    internal override void IdleExit() { base.IdleExit(); }
    internal override void MoveToTargetExit() { base.MoveToTargetExit(); }
    internal override void AttackExit() { base.AttackExit(); }


}
