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
    internal LoadPassiveSkills passiveSkillsSave;
    List<PassiveSkill_Base> skills = new List<PassiveSkill_Base>();
    [SerializeField] internal SkillClassList[] skillImages;
    [SerializeField] bool LoadPassiveSkills;
    GameObject passiveSkillUI;

    // Start is called before the first frame update
    void Start()
    {
        passiveSkillsSave = GameObject.FindGameObjectWithTag("save").GetComponent<LoadPassiveSkills>();

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

    private void LoadAllSkills(SavedPassiveSkills _save)
    {
        foreach (Tuple<PassiveSkill_Base.SkillType, int> element in _save.savedElements)
        {
            PassiveSkill_Base.SkillType skill = element.Item1;
            int tier = element.Item2;

            PassiveSkill_Base skillCreated = GetSkillByType(skill);
            passiveSkillUI.SetActive(true);

            AbilityButton abilityButtonUI = passiveSkillUI.transform.GetChild(1).GetChild(0).GetComponent<AbilityButton>();

            for (int i = 0; i < tier; i++)
            {
                AddSkill(skillCreated, false);
                abilityButtonUI.SpawnCardInUIBySave(element.Item1);
            }
        }

        passiveSkillUI.SetActive(false);
    }

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

        if (LoadPassiveSkills)
        {
            if(passiveSkillUI == null)
                passiveSkillUI = GameObject.Find("Misc Canvas").transform.GetChild(10).gameObject;
            else
            {
                SavedPassiveSkills save = passiveSkillsSave.LoadSave();
                LoadAllSkills(save);
                LoadPassiveSkills = false;
            }
        }
    }


    public void AddSkill(PassiveSkill_Base _skill, bool save)
    {
        PassiveSkill_Base skill = skills.Find(currSkill => currSkill.skillType == _skill.skillType);

        if(save)
            passiveSkillsSave.AddElementToSave(_skill.skillType);

        if (skill == null)
        {
            _skill.Init(transform);
            skills.Add(_skill);
        }
        else
            skill.AddLevel(1);
    }

    public PassiveSkill_Base FindSkill(PassiveSkill_Base _skill)
    {
        return skills.Find(_currSkill => _currSkill.skillType == _skill.skillType);
    }
    public PassiveSkill_Base FindSkill(PassiveSkill_Base.SkillType _skillType)
    {
        return skills.Find(_currSkill => _currSkill.skillType == _skillType);
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

            case PassiveSkill_Base.SkillType.CRITICAL_CHANCE:
                return new Criticon_PassiveSkill();

            default:
                return null;
        }

    }

}
