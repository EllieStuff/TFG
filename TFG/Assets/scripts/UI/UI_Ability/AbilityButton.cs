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


    private void OnEnable()
    {
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

    PassiveSkill_Base.SkillType InitializeCardSkill() 
    {
        return (PassiveSkill_Base.SkillType) Random.Range(0, (int)PassiveSkill_Base.SkillType.COUNT);
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
        playerSkills.AddSkill(skill);
    }

}
