using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UIFeedback_Default : UIFeedback_Base
{
    [Space]
    [Header("Custom Feedback")]
    [Space(2)]
    [SerializeField] Color baseColor = Color.white;
    [SerializeField] Color 
        selectedColor = new Color(0.63f, 0.63f, 0.63f, 1f), 
        clickedColor = new Color(0.48f, 0.48f, 0.48f, 1f);
    [Space]
    [SerializeField] float sizeInc = 1.1f;
    [SerializeField] float
        selectIncreaseSizeSpeed = 0.05f,
        selectDecreaseSizeSpeed = 0.2f,
        clickIncreaseSizeSpeed = 0.2f,
        clickDecreaseSizeSpeed = 0.15f,
        colorSpeed = 0.5f;
    
    Vector3 originalSize, targetSize;

    internal override void Start_Call() { base.Start_Call(); }
    internal override void Init()
    {
        base.Init();
        originalSize = transform.localScale;
        targetSize = originalSize * sizeInc;
        feedbackImage.color = baseColor;
    }
    internal override void StartClicked_Call()
    {
        base.StartClicked_Call();
        transform.localScale = targetSize;
        feedbackImage.color = clickedColor;
    }


    internal override void OnEnable_Call() { base.OnEnable_Call(); }


    internal override void Update_Call() { base.Update_Call(); }
    internal override void KeepSelected_Call()
    {
        base.KeepSelected_Call();
        transform.localScale = targetSize;
        //feedbackImage.color = clickedColor;
        if (selected)
            feedbackImage.color = selectedColor;
        else
            feedbackImage.color = clickedColor;
    }



    internal override void Select_Feedback()
    {
        base.Select_Feedback();
        EliTween.Scale(transform, targetSize, selectIncreaseSizeSpeed);
        EliTween.Scale(transform, originalSize, selectDecreaseSizeSpeed, clickIncreaseSizeSpeed);
        EliTween.ChangeColor(feedbackImage, selectedColor, colorSpeed);
    }

    internal override void UnSelect_Feedback()
    {
        base.UnSelect_Feedback();
        EliTween.Scale(transform, originalSize, selectDecreaseSizeSpeed);
        EliTween.ChangeColor(feedbackImage, baseColor, colorSpeed);
    }

    internal override void Click_Feedback()
    {
        base.Click_Feedback();
        EliTween.Scale(transform, targetSize, clickIncreaseSizeSpeed);
        EliTween.ChangeColor(feedbackImage, clickedColor, colorSpeed);
    }

    internal override void UnClick_Feedback()
    {
        base.UnClick_Feedback();
        EliTween.Scale(transform, originalSize, clickDecreaseSizeSpeed);
        EliTween.ChangeColor(feedbackImage, baseColor, colorSpeed);
    }

    internal override void ResetValues()
    {
        base.ResetValues();
        feedbackImage.color = baseColor;
        transform.localScale = originalSize;
    }

}
