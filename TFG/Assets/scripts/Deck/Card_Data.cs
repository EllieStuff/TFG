using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Card_Data : MonoBehaviour
{
    [SerializeField] internal DeckManager.CardType cardType;
    [SerializeField] internal Card_Behaviour cardBehaviour;
    
    internal Sprite iconSprite;


    private void Start()
    {
        Transform iconRef = transform.Find("Icon");
        if (iconRef.GetComponent<SpriteRenderer>() != null)
        {
            iconSprite = iconRef.GetComponent<SpriteRenderer>().sprite;
        }

    }

    public void Init(Card_Data _cardData)
    {
        cardType = _cardData.cardType;
        cardBehaviour = _cardData.cardBehaviour;
        iconSprite = _cardData.iconSprite;

        Transform iconRef = transform.Find("Icon");
        if(iconRef.GetComponent<Image>() != null)
        {
            iconRef.GetComponent<Image>().sprite = iconSprite;
        }
    }


}
