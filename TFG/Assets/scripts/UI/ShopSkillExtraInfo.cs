using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class ShopSkillExtraInfo : MonoBehaviour
{
    [SerializeField] TextMeshProUGUI skillName, skillDescription, skillLevels, skillPrice;

    public void SetExtraInfo(PassiveSkill_Base _skillData)
    {
        skillName.text = _skillData.Name;
        skillDescription.text = _skillData.Description;
        skillLevels.text = _skillData.Level.ToString() + "/" + (_skillData.MaxLevel < 0 ? "Any" : _skillData.MaxLevel.ToString()) + " levels";
        skillPrice.text = _skillData.Price.ToString();
    }

}
