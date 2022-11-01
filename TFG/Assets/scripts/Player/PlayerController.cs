using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    public enum PlayerState { NORMAL, ATTACKING }

    public PlayerState state = PlayerState.NORMAL;


    public bool StateEquals(PlayerState _state)
    {
        return state == _state;
    }
    public void ChangeState(PlayerState _state)
    {
        state = _state;
    }

}
