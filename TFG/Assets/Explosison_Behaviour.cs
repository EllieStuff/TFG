using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Explosison_Behaviour : Card_Behaviour
{
    [SerializeField] GameObject explosionPrefab;

    public override void Activate(PlayerMovement _playerData)
    {
        base.Activate(_playerData);
        _playerData.targetMousePos = Vector3.zero;
        DamageData explosion = Instantiate(explosionPrefab, _playerData.transform).GetComponent<DamageData>();
        explosion.transform.SetParent(null);
        explosion.ownerTransform = _playerData.transform;
    }

}
