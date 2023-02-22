using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class AbilityButton : MonoBehaviour
{
    TextMeshProUGUI uiTextName;
    [SerializeField] TextMeshProUGUI uiTextDescription;

    private RectTransform imageTransform;
    Vector2 originalSize;
    Vector2 biggerSize;
    float SIZE_RECT_LERP_SPEED = 5;

    private PassiveSkills_Manager playerSkills;
    private PassiveSkill_Base skill;
    PassiveSkill_Base.SkillType cardSkill;

    bool isMouseOver;
    bool sizePlaced;

    FadeInFadeOut_UI UIFade;

    const float DISABLE_TIMER = 3;

    bool pushedButton;

    private void Start()
    {
        UIFade = transform.parent.parent.GetComponent<FadeInFadeOut_UI>();
    }

    private void OnEnable()
    {
        isMouseOver = false;
        pushedButton = false;


        imageTransform = GetComponent<RectTransform>();

        if(!sizePlaced)
        {
            originalSize = imageTransform.sizeDelta;
            sizePlaced = true;
        }

        imageTransform.sizeDelta = Vector3.zero;
        biggerSize = originalSize * 2;
        playerSkills = GameObject.FindWithTag("Player").GetComponent<PassiveSkills_Manager>();
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
        return (PassiveSkill_Base.SkillType) Random.Range(0, (int)PassiveSkill_Base.SkillType.COUNT);
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
    }

    private void OnMouseOver()
    {
        uiTextDescription.text = skill.description;
        imageTransform.sizeDelta = Vector2.Lerp(imageTransform.sizeDelta, biggerSize, Time.deltaTime * SIZE_RECT_LERP_SPEED);
        isMouseOver = true;
    }

    private void OnMouseExit()
    {
        uiTextDescription.text = "";
        isMouseOver = false;
    }

    private void OnMouseDown()
    {
        AddAbility();
    }

    private void AddAbility()
    {
        if(!pushedButton)
        {
            playerSkills.AddSkill(skill);
            StartCoroutine(DisableUI());
            pushedButton = true;
        }
    }

    IEnumerator DisableUI()
    {
        DisableOtherButtons();
        UIFade.EnableFadeOut();
        yield return new WaitForSeconds(DISABLE_TIMER);
        UIFade.gameObject.SetActive(false);
        yield return 0;
    }

}
