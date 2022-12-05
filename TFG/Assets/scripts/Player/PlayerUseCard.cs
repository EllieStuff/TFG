using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerUseCard : MonoBehaviour
{
    DeckManager deck;
    PlayerMovement playerMov;
    int idx = 0;

    // Start is called before the first frame update
    void Start()
    {
        deck = FindObjectOfType<DeckManager>();
        playerMov = GetComponent<PlayerMovement>();
    }

    // Update is called once per frame
    void Update()
    {
        ManageInputs();

    }


    void ManageInputs()
    {
        if (Input.GetKeyDown(KeyCode.Alpha1))
        {
            deck.UseSelectedCard(0, playerMov);
        }
        else if (Input.GetKeyDown(KeyCode.Alpha2))
        {
            deck.UseSelectedCard(1, playerMov);
        }
        else if (Input.GetKeyDown(KeyCode.Alpha3))
        {
            deck.UseSelectedCard(2, playerMov);
        }
        else if (Input.GetKeyDown(KeyCode.Alpha4))
        {
            deck.UseSelectedCard(3, playerMov);
        }
        else if (Input.GetKeyDown(KeyCode.Alpha5))
        {
            deck.UseSelectedCard(4, playerMov);
        }
        else if (Input.GetKeyDown(KeyCode.Alpha6))
        {
            deck.UseSelectedCard(5, playerMov);
        }

    }

}
