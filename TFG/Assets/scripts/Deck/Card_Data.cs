using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class Card_Data : MonoBehaviour
{
    //[SerializeField] internal DeckManager.CardType cardType;
    [SerializeField] internal Card_Behaviour cardBehaviour;
    [SerializeField] internal float useDelay;
    [SerializeField] internal bool inCooldown = false;
    
    internal Sprite iconSprite;
    internal TextMeshProUGUI countdown;
    internal Image cooldownImage;
    internal int assignedKey = 1;

    private void Awake()
    {
        Init();

    }

    public void Init()
    {
        Transform iconRef = transform.Find("Icon");
        if (iconRef.GetComponent<SpriteRenderer>() != null)
        {
            iconSprite = iconRef.GetComponent<SpriteRenderer>().sprite;
        }
    }

    public void Init(Card_Data _cardData)
    {
        //cardType = _cardData.cardType;
        cardBehaviour = _cardData.cardBehaviour;
        iconSprite = _cardData.iconSprite;
        assignedKey = _cardData.assignedKey;
        useDelay = _cardData.useDelay;

        Transform iconRef = transform.Find("Icon");
        if(iconRef.GetComponent<Image>() != null)
        {
            iconRef.GetComponent<Image>().sprite = iconSprite;

            transform.Find("AssignedKey text").GetComponent<TextMeshProUGUI>().text = assignedKey.ToString();

            countdown = transform.Find("Countdown Text").GetComponent<TextMeshProUGUI>();
            cooldownImage = transform.Find("Cooldown Image").GetComponent<Image>();
            inCooldown = countdown.enabled = cooldownImage.enabled = false;
        }
    }


    public void StartCooldown()
    {
        StartCoroutine(CooldownCoroutine());
    }

    IEnumerator CooldownCoroutine()
    {
        inCooldown = countdown.enabled = cooldownImage.enabled = true;
        countdown.text = useDelay.ToString("0.0");

        float timer = useDelay;
        while(timer > 0)
        {
            yield return new WaitForEndOfFrame();
            timer -= Time.deltaTime;
            countdown.text = timer.ToString("0.0");
        }

        yield return new WaitForEndOfFrame();
        inCooldown = countdown.enabled = cooldownImage.enabled = false;

    }


}
