using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DeckManager : MonoBehaviour
{
    const float CARD_DIST = 200;

    public enum CardType { FIRE, ICE, COUNT }

    [SerializeField] Transform beginPos, cardsRef;
    [SerializeField] GameObject cardUIPrefab;
    [SerializeField] int maxCards = 3;

    List<Card_Data> cards = new List<Card_Data>();
    PlayerUseCard playerUseCard;
    int idx = 0;


    // Start is called before the first frame update
    void Start()
    {
        playerUseCard = GameObject.FindGameObjectWithTag("Player").GetComponent<PlayerUseCard>();
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.RightArrow))
        {
            idx++;
            if (idx >= cards.Count) idx = 0;
            //Feedback
        }
        if (Input.GetKeyDown(KeyCode.LeftArrow))
        {
            idx--;
            if (idx < 0) idx = cards.Count - 1;
            //Feedback
        }
        if (Input.GetKeyDown(KeyCode.Return))
        {
            cards[idx].cardBehaviour.Activate(playerUseCard);
            Destroy(cards[idx].gameObject);
            cards.RemoveAt(idx);
            ReallocCards();
        }
    }


    void ReallocCards()
    {
        //ToDo
    }

    //Llamar desde el jugador al colisionar con carta
    public void AddCard(Card_Data _cardData)
    {
        Vector3 newCardPos = new Vector3(beginPos.position.x + cards.Count * CARD_DIST, beginPos.position.y, beginPos.position.z);
        Card_Data newCard = Instantiate(cardUIPrefab, newCardPos, Quaternion.identity, cardsRef).GetComponent<Card_Data>();
        newCard.Init(_cardData);
        cards.Add(newCard);
    }
    public bool ReachedCardsLimit()
    {
        return cards.Count >= maxCards;
    }
}
