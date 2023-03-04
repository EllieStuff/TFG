using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EstocadaRelampago_Behaviour : Card_Behaviour
{
    Rigidbody playerRB;
    [SerializeField] private float dashForce;
    [SerializeField] private GameObject lightningPartricles;
    [SerializeField] private GameObject damageTriggerBox;

    public override void Activate(PlayerMovement _playerData)
    {
        base.Activate(_playerData);
        playerRB = _playerData.GetComponent<Rigidbody>();
        _playerData.targetMousePos = Vector3.zero;
        //_playerData.cardEffect = true;
        playerRB.constraints = (RigidbodyConstraints.FreezeRotation | RigidbodyConstraints.FreezePositionY);
        Instantiate(lightningPartricles, _playerData.transform);
        Instantiate(damageTriggerBox, _playerData.transform);
        playerRB.AddForce(new Vector3(_playerData.mouseLookVec.x, 0, _playerData.mouseLookVec.y).normalized * dashForce, ForceMode.Impulse);

    }

}
