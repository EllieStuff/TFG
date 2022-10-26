using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerLifeSystem : MonoBehaviour
{
    [SerializeField] internal float life;

    internal int playerMaxLife = 100;
    private enum playerStates { NORMAL }

    [SerializeField] private playerStates playerState;

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
