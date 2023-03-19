using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class TextData : MonoBehaviour
{
    internal float originalFontSize;

    void Awake()
    {
        originalFontSize = GetComponent<TextMeshProUGUI>().fontSize;
    }
}
