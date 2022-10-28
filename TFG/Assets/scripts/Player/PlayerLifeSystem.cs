using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerLifeSystem : MonoBehaviour
{
    [SerializeField] internal float life;

    internal int playerMaxLife = 100;
    internal enum playerStates { NORMAL, ATTACKING }

    [SerializeField] internal playerStates playerState;

    private void Update()
    {
        CheckPlayerLifeLimits();
    }
    void CheckPlayerLifeLimits()
    {
        if (life > playerMaxLife)
            life = playerMaxLife;
        else if (life < 0)
            life = 0;
    }

}
