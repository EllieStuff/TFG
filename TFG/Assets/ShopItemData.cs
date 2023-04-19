using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class ShopItemData : MonoBehaviour
{
    public PassiveSkill_Base.SkillType skillType;
    public TextMeshProUGUI nameText, priceText;
    public Image iconImage;

    internal PassiveSkill_Base data;
    internal ShopManager manager;

    UIFeedback_Base uiFeedback;
    bool selectedFlag = false;

    private void Awake()
    {
        data = PassiveSkills_Manager.GetSkillByType(skillType);
        uiFeedback = GetComponent<UIFeedback_Base>();
    }


    private void Update()
    {
        if(uiFeedback.selected && !selectedFlag)
        {
            selectedFlag = true;
            manager.extraInfoBox.SetExtraInfo(data);
        }
        else if(!uiFeedback.selected && selectedFlag)
        {
            selectedFlag = false;
        }
    }


}
