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
        if (Input.GetKeyDown(KeyCode.RightArrow))
        {
            deck.UnSelectCard(idx);
            idx++;
            if (idx >= deck.cards.Count) idx = 0;
            //Feedback
            deck.SelectCard(idx);
        }
        if (Input.GetKeyDown(KeyCode.LeftArrow))
        {
            deck.UnSelectCard(idx);
            idx--;
            if (idx < 0) idx = deck.cards.Count - 1;
            //Feedback
            deck.SelectCard(idx);
        }
        if (Input.GetKeyDown(KeyCode.Return))
        {
            deck.UseSelectedCard(ref idx, playerMov);
        }
    }
}
