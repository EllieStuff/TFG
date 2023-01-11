using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PassiveSkills_Manager : MonoBehaviour
{
    List<PassiveSkill_Base> skills = new List<PassiveSkill_Base>();

    // Start is called before the first frame update
    void Start()
    {
        //StartCoroutine(AddTrialSkillsCoroutine());
    }
    //IEnumerator AddTrialSkillsCoroutine()
    //{
    //    yield return new WaitForSeconds(1f);
    //    AddSkill(new IncreaseProjectileAmount_PassiveSkill());
    //    yield return new WaitForSeconds(5f);
    //    AddSkill(new IncreaseProjectileAmount_PassiveSkill());
    //    yield return new WaitForSeconds(2f);
    //    AddSkill(new IncreaseProjectileAmount_PassiveSkill());
    //}



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
        PassiveSkill_Base skill = skills.Find(currSkill => currSkill.skillType == _skill.skillType);
        if (skill == null)
        {
            _skill.Init(transform);
            skills.Add(_skill);
        }
        else
            skill.AddLevel(1);
    }


    public static PassiveSkill_Base GetSkillByType(PassiveSkill_Base.SkillType _skillType)
    {
        switch (_skillType)
        {
            case PassiveSkill_Base.SkillType.IMPROVE_LIFE:
                return new ImproveLife_PassiveSkill();

            case PassiveSkill_Base.SkillType.IMPROVE_ATTACK_SPEED:
                return new ImproveAttackSpeed_PassiveSkill();

            case PassiveSkill_Base.SkillType.IMPROVE_ATTACK_DAMAGE:
                return new ImproveAttackDamage_PassiveSkill();

            case PassiveSkill_Base.SkillType.INCREASE_PROJECTILE_AMOUNT:
                return new IncreaseProjectileAmount_PassiveSkill();


            default:
                return null;
        }

    }

}
