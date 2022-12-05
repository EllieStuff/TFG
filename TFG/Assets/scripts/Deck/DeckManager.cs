using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DeckManager : MonoBehaviour
{
    //public enum CardType { FIRE, ICE, COUNT }

    [SerializeField] RectTransform cardsFatherRef;
    [SerializeField] GameObject cardUIPrefab;
    [SerializeField] int maxCards = 3;
    [SerializeField] GameObject[] cardsRef = new GameObject[3];

    internal List<Card_Data> cards = new List<Card_Data>();
    Vector3 activeSize = new Vector3(1.05f, 0.89f, 0.89f);
    Vector3 nonActiveSize = new Vector3(1.0f, 0.84f, 0.84f);
    //PlayerUseCard playerUseCard;
    //int idx = 0;


    // Start is called before the first frame update
    void Start()
    {
        //playerUseCard = GameObject.FindGameObjectWithTag("Player").GetComponent<PlayerUseCard>();

        for(int i = 0; i < cardsRef.Length; i++)
        {
            Card_Data cardData = cardsRef[i].GetComponent<Card_Data>();
            cardData.Init();
            cardData.assignedKey = i + 1;
            AddCard(cardData);
        }
        ReallocCards();
    }

    // Update is called once per frame
    void Update()
    {

    }


    public void UseSelectedCard(int _idx, PlayerMovement _playerData)
    {
        if (_idx < cards.Count && !cards[_idx].inCooldown)
        {
            cards[_idx].cardBehaviour.Activate(_playerData);
            cards[_idx].StartCooldown();
        }
        //StartCoroutine(DestroyCard(cards[_idx].transform, _idx));
    }

    public void ReallocCards()
    {
        for (int i = 0; i < cards.Count; i++)
        {
            Vector3 newCardPos = GetCardPos(i);
            cards[i].transform.position = newCardPos;
        }
    }


    public void AddCard(Card_Data _cardData)
    {
        //Vector3 newCardPos = GetCardPos(cards.Count);
        Card_Data newCard = Instantiate(cardUIPrefab, Vector3.zero, Quaternion.identity, cardsFatherRef).GetComponent<Card_Data>();
        newCard.Init(_cardData);
        cards.Add(newCard);
        StartCoroutine(LerpCardSize(cards[cards.Count - 1].transform, Vector3.zero, activeSize));
    }

    Vector3 GetCardPos(int _cardIdx)
    {
        float xPos;
        if (cards.Count > 2)
            xPos = cardsFatherRef.position.x + (cardsFatherRef.rect.width * _cardIdx / (cards.Count - 1));
        else if (cards.Count == 2)
            xPos = cardsFatherRef.position.x + cardsFatherRef.rect.width / 4f + (cardsFatherRef.rect.width * _cardIdx / 2f);
        else
            xPos = cardsFatherRef.position.x + (cardsFatherRef.rect.width / 2f);
        return new Vector3(xPos, cardsFatherRef.position.y, cardsFatherRef.position.z);
    }

    public bool ReachedCardsLimit()
    {
        return cards.Count >= maxCards;
    }



    IEnumerator LerpCardSize(Transform _card, Vector3 _initSize, Vector3 _finalSize, float _lerpSpeed = 0.1f)
    {
        _card.localScale = _initSize;
        float timer = 0, maxTime = _lerpSpeed;
        while(timer < maxTime)
        {
            yield return new WaitForEndOfFrame();
            timer += Time.deltaTime;
            _card.localScale = Vector3.Lerp(_initSize, _finalSize, timer / maxTime);
        }
        yield return new WaitForEndOfFrame();
        _card.localScale = _finalSize;
    }

}
