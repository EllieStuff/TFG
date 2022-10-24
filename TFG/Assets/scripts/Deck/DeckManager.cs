using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DeckManager : MonoBehaviour
{
    const float CARD_DIST = 100;

    public enum CardType { FIRE, ICE, COUNT }

    [SerializeField] Transform beginPos, cardsRef;
    [SerializeField] GameObject cardUIPrefab;
    [SerializeField] int maxCards = 3;

    internal List<Card_Data> cards = new List<Card_Data>();
    Vector3 selectedSize = Vector3.one;
    Vector3 nonSelectedSize = new Vector3(0.5f, 0.5f, 0.5f);
    //PlayerUseCard playerUseCard;
    //int idx = 0;


    // Start is called before the first frame update
    void Start()
    {
        //playerUseCard = GameObject.FindGameObjectWithTag("Player").GetComponent<PlayerUseCard>();
    }

    // Update is called once per frame
    void Update()
    {
        //if (Input.GetKeyDown(KeyCode.RightArrow))
        //{
        //    idx++;
        //    if (idx >= cards.Count) idx = 0;
        //    //Feedback
        //}
        //if (Input.GetKeyDown(KeyCode.LeftArrow))
        //{
        //    idx--;
        //    if (idx < 0) idx = cards.Count - 1;
        //    //Feedback
        //}
        //if (Input.GetKeyDown(KeyCode.Return))
        //{
        //    cards[idx].cardBehaviour.Activate(playerUseCard);
        //    Destroy(cards[idx].gameObject);
        //    cards.RemoveAt(idx);
        //    ReallocCards();
        //}
    }


    public void SelectCard(int _idx)
    {
        Transform cardTransform = cards[_idx].transform;
        StartCoroutine(LerpCardSize(cardTransform, cardTransform.localScale, selectedSize));
        //cards[_idx].transform.localScale = selectedSize;
    }
    public void UnSelectCard(int _idx)
    {
        Transform cardTransform = cards[_idx].transform;
        StartCoroutine(LerpCardSize(cardTransform, cardTransform.localScale, nonSelectedSize));
        //cards[_idx].transform.localScale = nonSelectedSize;
    }

    public void UseSelectedCard(ref int _idx, PlayerMovement _playerData)
    {
        int tmpIdx = _idx;
        if (_idx == cards.Count - 1 && _idx > 0) _idx--;
        cards[tmpIdx].cardBehaviour.Activate(_playerData);
        StartCoroutine(DestroyCard(cards[tmpIdx].transform, tmpIdx));
    }

    public void ReallocCards(int _idx)
    {
        //ToDo
        if (cards.Count == 0) return;
        if (_idx >= cards.Count) _idx = cards.Count - 1;
        SelectCard(_idx);
        for(int i = 0; i < cards.Count; i++)
        {
            Vector3 newCardPos = GetCardPos(i);
            cards[i].transform.position = newCardPos;
        }
    }

    //Llamar desde el jugador al colisionar con carta
    public void AddCard(Card_Data _cardData)
    {
        Vector3 newCardPos = GetCardPos(cards.Count);
        Card_Data newCard = Instantiate(cardUIPrefab, newCardPos, Quaternion.identity, cardsRef).GetComponent<Card_Data>();
        newCard.Init(_cardData);
        cards.Add(newCard);
        if (cards.Count > 1)
            StartCoroutine(LerpCardSize(cards[cards.Count - 1].transform, Vector3.zero, nonSelectedSize));
        else
            StartCoroutine(LerpCardSize(cards[cards.Count - 1].transform, Vector3.zero, selectedSize));
    }

    Vector3 GetCardPos(int _cardIdx)
    {
        return new Vector3(beginPos.position.x + _cardIdx * CARD_DIST, beginPos.position.y, beginPos.position.z);
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
    IEnumerator DestroyCard(Transform _card, int _idx, float _lerpSpeed = 0.2f)
    {
        Vector3 initSize = _card.localScale;
        float timer = 0, maxTime = _lerpSpeed;
        while(timer < maxTime)
        {
            yield return new WaitForEndOfFrame();
            timer += Time.deltaTime;
            _card.localScale = Vector3.Lerp(initSize, Vector3.zero, timer / maxTime);
        }
        yield return new WaitForEndOfFrame();
        _card.localScale = Vector3.zero;

        Destroy(cards[_idx].gameObject);
        cards.RemoveAt(_idx);
        ReallocCards(_idx);
    }

}
