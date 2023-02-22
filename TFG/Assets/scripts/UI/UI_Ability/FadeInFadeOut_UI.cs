using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class FadeInFadeOut_UI : MonoBehaviour
{
    [SerializeField] private Image image;
    private bool fadeInOrFadeOut;

    private Color fadeInColor;
    private Color fadeOutColor;
    [SerializeField] float COLOR_LERP_SPEED = 0.7f;

    private void Awake()
    {
        fadeInColor = image.color;
    }

    public void EnableFadeOut()
    {
        fadeInOrFadeOut = false;
    }

    public void EnableFadeIn()
    {
        fadeInOrFadeOut = true;
    }

    private void OnEnable()
    {
        fadeInOrFadeOut = true;
        image.color = new Color(fadeInColor.r, fadeInColor.g, fadeInColor.b, 0);
        fadeOutColor = image.color;
    }

    private void Update()
    {
        if (fadeInOrFadeOut)
            FadeIn();
        else
            FadeOut();
    }

    void FadeIn()
    {
        image.color = Color.Lerp(image.color, fadeInColor, Time.deltaTime * COLOR_LERP_SPEED);
    }

    void FadeOut()
    {
        image.color = Color.Lerp(image.color, fadeOutColor, Time.deltaTime * COLOR_LERP_SPEED);
    }
}
