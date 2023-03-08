using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
class SkillClassList
{
    public PassiveSkill_Base.SkillType skill;
    public Sprite image;
}

public class PassiveSkills_Manager : MonoBehaviour
{
    List<PassiveSkill_Base> skills = new List<PassiveSkill_Base>();
    [SerializeField] internal SkillClassList[] skillImages;

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

    public Sprite SearchSkillImage(PassiveSkill_Base.SkillType _skill)
    {
        foreach(SkillClassList skill in skillImages)
        {
            if (skill.skill.Equals(_skill))
                return skill.image;
        }

        return null;
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

            case PassiveSkill_Base.SkillType.HEAL:
                return new Heal_PassiveSkill();

            case PassiveSkill_Base.SkillType.IMPROVE_ATTACK_SPEED:
                return new ImproveAttackSpeed_PassiveSkill();

            case PassiveSkill_Base.SkillType.IMPROVE_ATTACK_DAMAGE:
                return new ImproveAttackDamage_PassiveSkill();

            case PassiveSkill_Base.SkillType.INCREASE_PROJECTILE_AMOUNT:
                return new IncreaseProjectileAmount_PassiveSkill();

            case PassiveSkill_Base.SkillType.VAMPIRE:
                return new Vampire_PassiveSkill();

            case PassiveSkill_Base.SkillType.QUICK_CONJURING:
                return new QuickConjuring_PassiveSkill();

            case PassiveSkill_Base.SkillType.PIERCE:
                return new Pierce_PassiveSkill();

            case PassiveSkill_Base.SkillType.WALK_SPEED:
                return new WalkFaster_PassiveSkill();

            case PassiveSkill_Base.SkillType.FIRST_STRIKE:
                return new FirstStrike_PassiveSkill();

            default:
                return null;
        }

    }

}
