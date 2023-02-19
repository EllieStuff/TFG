using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AbilityButton : MonoBehaviour
{
    private RectTransform imageTransform;
    Vector2 originalSize;
    Vector2 biggerSize;
    float SIZE_RECT_LERP_SPEED = 5;
    private PassiveSkills_Manager playerSkills;
    internal PassiveSkill_Base skill;

    bool isMouseOver;

    private void Start()
    {
        imageTransform = GetComponent<RectTransform>();
        originalSize = imageTransform.sizeDelta;
        biggerSize = originalSize * 2;
        playerSkills = GameObject.FindWithTag("Player").GetComponent<PassiveSkills_Manager>();
    }

    private void Update()
    {
        if (!isMouseOver)
            imageTransform.sizeDelta = Vector2.Lerp(imageTransform.sizeDelta, originalSize, Time.deltaTime * SIZE_RECT_LERP_SPEED);
    }

    private void OnMouseOver()
    {
        imageTransform.sizeDelta = Vector2.Lerp(imageTransform.sizeDelta, biggerSize, Time.deltaTime * SIZE_RECT_LERP_SPEED);
        isMouseOver = true;
    }

    private void OnMouseExit()
    {
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
