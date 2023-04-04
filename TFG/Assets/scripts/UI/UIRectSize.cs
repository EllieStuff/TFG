using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UIRectSize : MonoBehaviour
{
    RectTransform textureRectTransform;
    [SerializeField] Vector2 uiSize;
    [SerializeField] bool disableWhenCardMenuIsOpened;
    [SerializeField] GameObject cardMenu;

    Image localImage;
    Camera cam;

    void Start()
    {
        localImage = GetComponent<Image>();
        textureRectTransform = GetComponent<RectTransform>();
        cam = Camera.main;
    }

    void FixedUpdate()
    {
        if (disableWhenCardMenuIsOpened && localImage.enabled && cardMenu.activeSelf)
            localImage.enabled = false;
        if(disableWhenCardMenuIsOpened && !localImage.enabled && !cardMenu.activeSelf)
            localImage.enabled = true;

        uiSize = new Vector2(cam.pixelRect.width, cam.pixelRect.height);
        textureRectTransform.sizeDelta = uiSize;
    }
}
