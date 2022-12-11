using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EstocadaRelampago_Behaviour : Card_Behaviour
{
    Rigidbody playerRB;
    PlayerMovement playerMovement;
    [SerializeField] private float dashForce;
    [SerializeField] private GameObject lightningPartricles;
    [SerializeField] private GameObject damageTriggerBox;

    public override void Activate(PlayerMovement _playerData)
    {
        base.Activate(_playerData);
        GameObject player = GameObject.Find("Player");
        playerRB = player.GetComponent<Rigidbody>();
        playerMovement = player.GetComponent<PlayerMovement>();
        playerMovement.targetMousePos = Vector3.zero;
        playerMovement.cardEffect = true;
        playerRB.constraints = (RigidbodyConstraints.FreezeRotation | RigidbodyConstraints.FreezePositionY);
        Instantiate(lightningPartricles, player.transform);
        Instantiate(damageTriggerBox, player.transform);
        playerRB.AddForce(new Vector3(playerMovement.mouseLookVec.x, 0, playerMovement.mouseLookVec.y).normalized * dashForce, ForceMode.Impulse);

    }

    // Update is called once per frame
    void Update()
    {

    }
}
