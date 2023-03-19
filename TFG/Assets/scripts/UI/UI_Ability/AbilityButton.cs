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
    float originalTextNameSize;
    float biggerTextNameSize;
    float originalTextDescriptionSize;
    float biggerTextDescriptionSize;
    float SIZE_RECT_LERP_SPEED = 5;

    private PassiveSkills_Manager playerSkills;
    private PlayerMovement playerMovement;
    private LifeSystem playerLife;
    private PassiveSkill_Base skill;
    PassiveSkill_Base.SkillType skillType;
    string skillText;

    bool isMouseOver;
    bool sizePlaced;
    bool firstStart;
    bool firstOverIteration;

    FadeInFadeOut_UI UIFade;

    const float DISABLE_TIMER = 3;
    const float LERP_MOVE_SPEED = 2;
    const float TEXT_SIZE_SPEED = 1.2f;
    const int HEAL_CARD_PERCENTAGE = 100;
    const float TEXT_SIZE_MULTIPLIER_DESCRIPTION = 1.2f;
    const float TEXT_SIZE_MULTIPLIER_NAME = 1.1f;

    bool pushedButton;

    private void Start()
    {
        cardListPivot = GameObject.FindGameObjectWithTag("CardGrid").transform;
        UIFade = transform.parent.parent.GetComponent<FadeInFadeOut_UI>();
        originalTextDescriptionSize = uiTextDescription.GetComponent<TextData>().originalFontSize;
        biggerTextDescriptionSize = originalTextDescriptionSize * TEXT_SIZE_MULTIPLIER_DESCRIPTION;
    }

    private void OnEnable()
    {
        firstOverIteration = true;
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
        playerLife = playerSkills.GetComponent<LifeSystem>();
        playerMovement = playerSkills.GetComponent<PlayerMovement>();
        playerMovement.canMove = false;
        uiTextName = transform.GetChild(0).GetComponent<TextMeshProUGUI>();
        originalTextNameSize = uiTextName.fontSize;
        biggerTextNameSize = originalTextNameSize * TEXT_SIZE_MULTIPLIER_NAME;

        InitializeAbility();
        
        EnableOrDisableCardView(transform, true);
    }

    void InitializeAbility()
    {
        do
        {
            skillType = InitializeCardSkill();
            PassiveSkill_Base playerSkill = playerSkills.FindSkill(skillType);
            if (playerSkill == null) skill = PassiveSkills_Manager.GetSkillByType(skillType); 
            else skill = playerSkill;
        } while (!skill.CanBeImproved);

        if (skill.Level == 0) skillText = skill.initialDescription;
        else skillText = skill.improvementDescription;

        GetComponent<Image>().sprite = playerSkills.SearchSkillImage(skillType);
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
                if(child.GetComponent<AbilityButton>().skillType.Equals(skillType))
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
        PassiveSkill_Base.SkillType tmpSkillType = (PassiveSkill_Base.SkillType)Random.Range(0, (int)PassiveSkill_Base.SkillType.COUNT);

        while (playerSkills.SearchSkillImage(tmpSkillType) == null)
            tmpSkillType = (PassiveSkill_Base.SkillType)Random.Range(0, (int)PassiveSkill_Base.SkillType.COUNT);

        return tmpSkillType;
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
        {
            imageTransform.sizeDelta = Vector2.Lerp(imageTransform.sizeDelta, originalSize, Time.deltaTime * SIZE_RECT_LERP_SPEED);
            uiTextName.fontSize = Mathf.Lerp(uiTextName.fontSize, originalTextNameSize, Time.deltaTime * TEXT_SIZE_SPEED);
        }

        if (pushedButton)
        {
            imageTransform.localPosition = Vector3.Lerp(imageTransform.localPosition, Vector3.zero, Time.deltaTime * LERP_MOVE_SPEED);
            SetTextPositionInElement();
        }
    }

    private void OnMouseOver()
    {
        SetTextPositionInElement();
        uiTextDescription.text = skillText;
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

        firstOverIteration = true;
    }

    private void OnMouseDown()
    {
        AddAbility();
    }

    void SetTextPositionInElement()
    {
        RectTransform textRectTransfom = uiTextDescription.rectTransform;

        if (firstOverIteration)
        {
            uiTextDescription.fontSize = originalTextDescriptionSize;
            firstOverIteration = false;
        }
        else
        {
            textRectTransfom.localPosition = new Vector3(imageTransform.localPosition.x, textRectTransfom.localPosition.y);
            uiTextName.fontSize = Mathf.Lerp(uiTextName.fontSize, biggerTextNameSize, Time.deltaTime * TEXT_SIZE_SPEED);
            uiTextDescription.fontSize = Mathf.Lerp(uiTextDescription.fontSize, biggerTextDescriptionSize, Time.deltaTime * LERP_MOVE_SPEED);
        }

    }

    CardUIScript CheckIfThisCardExistsAlready()
    {
        foreach(Transform child in cardListPivot)
        {
            if (child.GetComponent<CardUIScript>().skillType.Equals(skillType))
                return child.GetComponent<CardUIScript>();
        }

        return null;
    }

    CardUIScript CheckIfThisCardExistsAlreadyBySave(PassiveSkill_Base.SkillType _skillType)
    {
        cardListPivot = GameObject.FindGameObjectWithTag("CardGrid").transform;
        foreach (Transform child in cardListPivot)
        {
            if (child.GetComponent<CardUIScript>().skillType.Equals(_skillType))
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

            card.skillType = skillType;
            card.cardSprite = GetComponent<Image>().sprite;
        }
    }

    public void SpawnCardInUIBySave(PassiveSkill_Base.SkillType _cardType)
    {
        CardUIScript cardCheck = CheckIfThisCardExistsAlreadyBySave(_cardType);

        if (cardCheck != null)
        {
            cardCheck.ModifyCardTier();
        }
        else
        {
            CardUIScript card = Instantiate(cardPrefabUI, cardListPivot).GetComponent<CardUIScript>();

            card.skillType = _cardType;
            card.cardSprite = playerSkills.SearchSkillImage(_cardType);
        }

        playerMovement.canMove = true;
    }

    private void AddAbility()
    {
        if(!pushedButton)
        {
            float lifeToImprove = (HEAL_CARD_PERCENTAGE * playerLife.maxLife) / playerLife.maxLife;
            playerLife.AddLife(lifeToImprove);
            playerSkills.AddSkill(skill, true);

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
        uiTextDescription.text = "";
        UIFade.gameObject.SetActive(false);
        yield return 0;
    }

}
