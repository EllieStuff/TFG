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
        Transform player = GameObject.Find("Player").transform;
        Instantiate(cutEffectParticles, player);
        Instantiate(cutDamageZone, player);
    }
}
