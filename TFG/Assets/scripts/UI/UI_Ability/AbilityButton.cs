using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class AbilityButton : MonoBehaviour
{
    TextMeshProUGUI uiTextName;
    [SerializeField] TextMeshProUGUI uiTextDescription;
    [SerializeField] GameObject cardPrefabUI;
    Transform cardListPivot;

    private RectTransform imageTransform;
    Vector2 originalSize;
    Vector2 biggerSize;
    Vector2 originalPos;
    float SIZE_RECT_LERP_SPEED = 5;

    private PassiveSkills_Manager playerSkills;
    private PlayerMovement playerMovement;
    private PassiveSkill_Base skill;
    PassiveSkill_Base.SkillType cardSkill;

    bool isMouseOver;
    bool sizePlaced;
    bool firstStart;

    FadeInFadeOut_UI UIFade;

    const float DISABLE_TIMER = 3;
    const float LERP_MOVE_SPEED = 2;

    bool pushedButton;

    private void Start()
    {
        cardListPivot = GameObject.FindGameObjectWithTag("CardGrid").transform;
        UIFade = transform.parent.parent.GetComponent<FadeInFadeOut_UI>();
    }

    private void OnEnable()
    {
        isMouseOver = false;
        pushedButton = false;


        imageTransform = GetComponent<RectTransform>();

        if(!firstStart)
        {
            originalPos = GetComponent<RectTransform>().localPosition;
            firstStart = true;
        }

        imageTransform.localPosition = originalPos;

        if (!sizePlaced)
        {
            originalSize = imageTransform.sizeDelta;
            sizePlaced = true;
        }

        imageTransform.sizeDelta = Vector3.zero;
        biggerSize = originalSize * 2;
        playerSkills = GameObject.FindWithTag("Player").GetComponent<PassiveSkills_Manager>();
        playerMovement = playerSkills.GetComponent<PlayerMovement>();
        playerMovement.canMove = false;
        uiTextName = transform.GetChild(0).GetComponent<TextMeshProUGUI>();

        InitializeAbility();
        
        EnableOrDisableCardView(transform, true);
    }

    void InitializeAbility()
    {
        cardSkill = InitializeCardSkill();
        skill = PassiveSkills_Manager.GetSkillByType(cardSkill);

        GetComponent<Image>().sprite = playerSkills.SearchSkillImage(cardSkill);
        uiTextName.text = skill.name;
    }

    void CheckRepeatedAbility()
    {
        Transform parent = transform.parent;
        int childCount = parent.childCount;

        for (int index = 0; index < childCount; index++)
        {
            Transform child = parent.GetChild(index);
            if (!child.Equals(transform))
            {
                if(child.GetComponent<AbilityButton>().cardSkill.Equals(cardSkill))
                    InitializeAbility();
            }
        }
    }

    void DisableOtherButtons()
    {
        Transform parent = transform.parent;
        int childCount = parent.childCount;

        for (int index = 0; index < childCount; index++)
        {
            Transform child = parent.GetChild(index);
            if (!child.Equals(transform))
            {
                EnableOrDisableCardView(child, false);
            }
        }
    }

    PassiveSkill_Base.SkillType InitializeCardSkill() 
    {
        PassiveSkill_Base.SkillType skill = (PassiveSkill_Base.SkillType)Random.Range(0, (int)PassiveSkill_Base.SkillType.COUNT);

        while (playerSkills.SearchSkillImage(skill) == null)
            skill = (PassiveSkill_Base.SkillType)Random.Range(0, (int)PassiveSkill_Base.SkillType.COUNT);

        return skill;
    }

    void EnableOrDisableCardView(Transform card, bool enableOrDisable)
    {
        if(enableOrDisable)
            card.GetComponent<FadeInFadeOut_UI>().EnableFadeIn();
        else
            card.GetComponent<FadeInFadeOut_UI>().EnableFadeOut();

        card.GetComponent<BoxCollider>().enabled = enableOrDisable;
        card.GetChild(0).gameObject.SetActive(enableOrDisable);
    }

    private void Update()
    {
        CheckRepeatedAbility();

        if (!isMouseOver)
            imageTransform.sizeDelta = Vector2.Lerp(imageTransform.sizeDelta, originalSize, Time.deltaTime * SIZE_RECT_LERP_SPEED);

        if (pushedButton)
        {
            imageTransform.localPosition = Vector3.Lerp(imageTransform.localPosition, Vector3.zero, Time.deltaTime * LERP_MOVE_SPEED);
            SetTextPositionInElement();
        }
    }

    private void OnMouseOver()
    {
        SetTextPositionInElement();
        uiTextDescription.text = skill.description;
        imageTransform.sizeDelta = Vector2.Lerp(imageTransform.sizeDelta, biggerSize, Time.deltaTime * SIZE_RECT_LERP_SPEED);
        isMouseOver = true;
    }

    private void OnMouseExit()
    {
        if (!pushedButton)
        {
            uiTextDescription.text = "";
            isMouseOver = false;
        }
    }

    private void OnMouseDown()
    {
        AddAbility();
    }

    void SetTextPositionInElement()
    {
        RectTransform textRectTransfom = uiTextDescription.rectTransform;
        uiTextDescription.rectTransform.localPosition = new Vector3(imageTransform.localPosition.x, textRectTransfom.localPosition.y);
    }

    CardUIScript CheckIfThisCardExistsAlready()
    {
        foreach(Transform child in cardListPivot)
        {
            if (child.GetComponent<CardUIScript>().skillType.Equals(cardSkill))
                return child.GetComponent<CardUIScript>();
        }

        return null;
    }

    void SpawnCardInUI()
    {
        CardUIScript cardCheck = CheckIfThisCardExistsAlready();

        if (cardCheck != null)
        {
            cardCheck.ModifyCardTier();
        }
        else
        {
            CardUIScript card = Instantiate(cardPrefabUI, cardListPivot).GetComponent<CardUIScript>();

            card.skillType = cardSkill;
            card.cardSprite = GetComponent<Image>().sprite;
        }
    }

    private void AddAbility()
    {
        if(!pushedButton)
        {
            playerSkills.AddSkill(skill);

            if (cardListPivot != null && cardPrefabUI != null)
                SpawnCardInUI();

            StartCoroutine(DisableUI());
            pushedButton = true;
        }
    }

    IEnumerator DisableUI()
    {
        DisableOtherButtons();
        UIFade.EnableFadeOut();
        yield return new WaitForSeconds(DISABLE_TIMER);
        playerMovement.canMove = true;
        UIFade.gameObject.SetActive(false);
        yield return 0;
    }

}
