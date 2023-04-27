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
    [SerializeField] bool switchUI;
    Transform parentFind;

    enum SwitchMode { OVERLAY, CAMERA }

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
        SwitchUI(SwitchMode.CAMERA);
        fadeInOrFadeOut = true;
        image.color = new Color(fadeInColor.r, fadeInColor.g, fadeInColor.b, 0);
        fadeOutColor = image.color;
    }

    private void OnDisable()
    {
        SwitchUI(SwitchMode.OVERLAY);
    }

    private void Update()
    {
        if (switchUI)
            CheckIfThisElementMustBeDisabled();

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

    void CheckIfThisElementMustBeDisabled()
    {
        if (parentFind == null)
            parentFind = transform.parent.Find("DeathScreen").GetChild(0);
        else
        {
            if (parentFind.gameObject.activeSelf)
                gameObject.SetActive(false);
        }
    }

    void SwitchUI(SwitchMode _mode)
    {
        if (!switchUI)
            return;

        if(_mode.Equals(SwitchMode.OVERLAY))
            transform.parent.GetComponent<Canvas>().renderMode = RenderMode.ScreenSpaceOverlay;
        if (_mode.Equals(SwitchMode.CAMERA))
            transform.parent.GetComponent<Canvas>().renderMode = RenderMode.ScreenSpaceCamera;
    }
}
