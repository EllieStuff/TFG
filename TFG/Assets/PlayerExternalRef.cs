using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerExternalRef : MonoBehaviour
{
    internal DeckManager deckManager;


    private void Awake()
    {
        deckManager = FindObjectOfType<DeckManager>();
    }

}
