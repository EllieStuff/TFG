using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PassiveSkills_Manager : MonoBehaviour
{
    List<PassiveSkill_Base> skills = new List<PassiveSkill_Base>();

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        foreach(PassiveSkill_Base skill in skills)
        {
            skill.UpdateCall();
        }
    }


    public void AddSkill(PassiveSkill_Base _skill)
    {
        PassiveSkill_Base skill = skills.Find(currSkill => currSkill == _skill);
        if (skill == null)
        {
            skills.Add(_skill);
            skill.Init(transform);
        }
        else
            skill.AddLevel(1);
    }

}
