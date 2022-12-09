using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DecisiveCut_Behaviour : Card_Behaviour
{
    [SerializeField] private GameObject cutEffectParticles;
    [SerializeField] private GameObject cutDamageZone;

    public override void Activate(PlayerMovement _playerData)
    {
        base.Activate(_playerData);
        Transform player = _playerData.transform;
        Instantiate(cutEffectParticles, player);
        DamageData dmgZoneData = Instantiate(cutDamageZone, player).GetComponent<DamageData>();
        dmgZoneData.ownerTransform = _playerData.transform;
    }
}
