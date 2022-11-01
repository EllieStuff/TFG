using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerCollisions : MonoBehaviour
{
    PlayerExternalRef externalRefs;


    private void Start()
    {
        externalRefs = GetComponent<PlayerExternalRef>();
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Card"))
        {
            if (!externalRefs.deckManager.ReachedCardsLimit())
            {
                externalRefs.deckManager.AddCard(other.GetComponent<Card_Data>());
                Destroy(other.gameObject);
            }
        }
    }

}
