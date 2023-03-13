using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UIFeedback_SwapSprite : UIFeedback_Base
{
    [Space]
    [Header("Custom Feedback")]
    [Space(2)]
    [SerializeField] Sprite baseImage; 
    [SerializeField] Sprite selectedImage, clickedImage;
    [Space]
    [SerializeField] Color 
        baseColor = Color.white;
    [SerializeField]
    Color
        selectedColor = Color.white,
        clickedColor = Color.white;
    [Space]
    [SerializeField] float sizeInc = 1.1f;
    [SerializeField] float
        selectIncreaseSizeSpeed = 0.2f,
        selectDecreaseSizeSpeed = 0.2f,
        colorSpeed = 0.5f;
    
    Vector3 originalSize, targetSize;

    internal override void Start_Call() { base.Start_Call(); }
    internal override void Init()
    {
        base.Init();
        originalSize = transform.localScale;
        targetSize = originalSize * sizeInc;
        feedbackImage.color = baseColor;
        feedbackImage.sprite = baseImage;
    }
    internal override void StartClicked_Call()
    {
        base.StartClicked_Call();
        transform.localScale = targetSize;
        feedbackImage.color = clickedColor;
        feedbackImage.sprite = clickedImage;
    }


    internal override void OnEnable_Call() { base.OnEnable_Call(); }


    internal override void Update_Call() { base.Update_Call(); }
    internal override void KeepSelected_Call()
    {
        base.KeepSelected_Call();
        //transform.localScale = targetSize;
        feedbackImage.sprite = clickedImage;

        if (selected)
            feedbackImage.color = selectedColor;
        else
            feedbackImage.color = clickedColor;
    }



    internal override void Select_Visuals()
    {
        base.Select_Visuals();
        feedbackImage.sprite = selectedImage;
        EliTween.Scale(transform, targetSize, selectIncreaseSizeSpeed);
        EliTween.ChangeColor(feedbackImage, selectedColor, colorSpeed);
    }

    public override void UnSelect()
    {
        base.UnSelect();
        EliTween.Scale(transform, originalSize, selectDecreaseSizeSpeed);
    }
    internal override void UnSelect_Visuals()
    {
        base.UnSelect_Visuals();
        feedbackImage.sprite = baseImage;
        EliTween.ChangeColor(feedbackImage, baseColor, colorSpeed);
    }

    internal override void Click_Visuals()
    {
        base.Click_Visuals();
        feedbackImage.sprite = clickedImage;
        EliTween.ChangeColor(feedbackImage, clickedColor, colorSpeed);
    }

    internal override void UnClick_Visuals()
    {
        base.UnClick_Visuals();
        if (!keepSelected)
        {
            EliTween.Scale(transform, originalSize, selectDecreaseSizeSpeed);
            if (selected) feedbackImage.sprite = selectedImage;
            else feedbackImage.sprite = baseImage;
        }
        EliTween.ChangeColor(feedbackImage, baseColor, colorSpeed);
    }

    internal override void ResetValues()
    {
        base.ResetValues();
        feedbackImage.sprite = baseImage;
        feedbackImage.color = baseColor;
        transform.localScale = originalSize;
    }

}
