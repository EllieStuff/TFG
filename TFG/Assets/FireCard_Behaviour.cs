using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FireCard_Behaviour : Card_Behaviour
{
    [SerializeField] GameObject fireAttackPrefab;
    [SerializeField] bool lockPlayerPos = true;



    // Start is called before the first frame update
    void Start()
    {
        
    }

    public override void Activate(PlayerMovement _playerData)
    {
        base.Activate(_playerData);
        _playerData.canMove = _playerData.canRotate = !lockPlayerPos;
        FireAttack fireAttack = GameObject.Instantiate(fireAttackPrefab, _playerData.transform.position, fireAttackPrefab.transform.rotation).GetComponent<FireAttack>();
        fireAttack.Initialize(_playerData, lockPlayerPos);
        fireAttack.Shoot(_playerData.transform.forward);
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
