using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Tornado_Behaviour : Card_Behaviour
{
    [SerializeField] GameObject tornadoPrefab;

    public override void Activate(PlayerMovement _playerData) 
    {
        base.Activate(_playerData);
        Instantiate(tornadoPrefab, GameObject.Find("Player").transform);
    }
}
