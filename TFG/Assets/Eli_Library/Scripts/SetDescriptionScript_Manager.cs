using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class SetDescriptionScript_Manager : MonoBehaviour
{
    [SerializeField] internal TextMeshProUGUI targetTMPro;
    [SerializeField] string description = "";


    public void SetDescription(string _description)
    {
        targetTMPro.text = _description;
        description = _description;
    }

    public void ResetDescription()
    {
        targetTMPro.text = description;
    }

}
