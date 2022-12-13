using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DecisiveCut_Behaviour : Card_Behaviour
{
    [SerializeField] private GameObject cutEffectParticles;
    [SerializeField] private GameObject cutDamageZone;
    [SerializeField] float cutDelayBeforeAttack = 2.0f, cutDelayAfterAttack = 0.5f;

    public override void Activate(PlayerMovement _playerData)
    {
        base.Activate(_playerData);
        _playerData.StartCoroutine(SpawnCut(_playerData));
    }


    IEnumerator SpawnCut(PlayerMovement _playerData)
    {
        _playerData.canMove = false;
        _playerData.targetMousePos = _playerData.transform.position;
        FindObjectOfType<WalkMark>().SetWalkMarkActive(false);

        yield return new WaitForSeconds(cutDelayBeforeAttack);
        Transform player = _playerData.transform;
        Instantiate(cutEffectParticles, player);
        DamageData dmgZoneData = Instantiate(cutDamageZone, player).GetComponent<DamageData>();
        dmgZoneData.ownerTransform = _playerData.transform;

        yield return new WaitForSeconds(cutDelayAfterAttack);
        _playerData.canMove = true;
    }

}
