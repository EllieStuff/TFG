using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class CardUIScript : MonoBehaviour
{
    [SerializeField] internal PassiveSkill_Base.SkillType skillType;
    [SerializeField] internal Sprite cardSprite;
    [SerializeField] private TextMeshProUGUI tierText;

    int cardTier = 1;

    internal void ModifyCardTier()
    {
        cardTier++;
        tierText.text = cardTier.ToString();

        if (cardTier > 0 && !tierText.enabled)
            tierText.enabled = true;
    }

    void Start()
    {
        if (skillType.Equals(PassiveSkill_Base.SkillType.HEAL))
            Destroy(gameObject);

        transform.name = skillType.ToString();
        GetComponent<Image>().sprite = cardSprite;
        tierText.enabled = false;
    }
}
