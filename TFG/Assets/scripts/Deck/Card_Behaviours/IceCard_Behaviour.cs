using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class IceCard_Behaviour : Card_Behaviour
{
    [SerializeField] GameObject iceAttackPrefab;
    [SerializeField] bool lockPlayerPos = true;



    // Start is called before the first frame update
    void Start()
    {

    }

    public override void Activate(PlayerMovement _playerData)
    {
        base.Activate(_playerData);
        _playerData.canMove = _playerData.canRotate = !lockPlayerPos;
        //FireAttack iceAttack = GameObject.Instantiate(iceAttackPrefab, _playerData.transform.position, iceAttackPrefab.transform.rotation).GetComponent<FireAttack>();
        //iceAttack.Initialize(_playerData, lockPlayerPos);
        //iceAttack.Shoot(_playerData.transform.forward);
    }

    // Update is called once per frame
    void Update()
    {

    }
}
