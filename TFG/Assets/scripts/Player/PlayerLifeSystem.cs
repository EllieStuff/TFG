using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerLifeSystem : MonoBehaviour
{
    [SerializeField] internal float life;

    internal int playerMaxLife = 100;
    private enum playerStates { NORMAL }

    [SerializeField] private playerStates playerState;
}
