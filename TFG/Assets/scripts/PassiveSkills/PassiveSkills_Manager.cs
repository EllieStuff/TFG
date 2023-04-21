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

struct SkillAppearRatio
{
    public PassiveSkill_Base.SkillType skillType;
    public Vector2 appearRange;
    public SkillAppearRatio(PassiveSkill_Base.SkillType _skillType, Vector2 _appearRange)
    {
        skillType = _skillType;
        appearRange = _appearRange;
    }
}

public class PassiveSkills_Manager : MonoBehaviour
{
    const float MAX_APPEAR_VALUE = 100f;
    const float APPEAR_SKILL_THRESHOLD = 0.001f;

    internal LoadPassiveSkills passiveSkillsSave;
    List<PassiveSkill_Base> skills = new List<PassiveSkill_Base>();
    [SerializeField] internal SkillClassList[] skillImages;
    [SerializeField] bool mustLoadPassiveSkills;
    internal GameObject passiveSkillUI;

    static SkillAppearRatio[] skillsAppearRatio;

    private void Awake()
    {
        
    }

    // Start is called before the first frame update
    void Start()
    {
        passiveSkillsSave = GameObject.FindGameObjectWithTag("save").GetComponent<LoadPassiveSkills>();

        float[] tmpSkillsAppearRatio = new float[(int)PassiveSkill_Base.SkillType.COUNT];
        float proportionalAppearPercentage = 0f;
        for (int i = 0; i < tmpSkillsAppearRatio.Length; i++)
        {
            tmpSkillsAppearRatio[i] = GetSkillByType((PassiveSkill_Base.SkillType)i).AppearRatio;
            proportionalAppearPercentage += tmpSkillsAppearRatio[i];
        }
        
        skillsAppearRatio = new SkillAppearRatio[(int)PassiveSkill_Base.SkillType.COUNT];
        float prevAppearRangeMargin = APPEAR_SKILL_THRESHOLD;
        for(int i = 0; i < skillsAppearRatio.Length; i++)
        {
            Vector2 appearRange = new Vector2(prevAppearRangeMargin, prevAppearRangeMargin + tmpSkillsAppearRatio[i] * MAX_APPEAR_VALUE / proportionalAppearPercentage);
            //Debug.Log(((PassiveSkill_Base.SkillType)i).ToString() + " ratio: " + (appearRange.y - appearRange.x));
            prevAppearRangeMargin = appearRange.y;

            skillsAppearRatio[i] = new SkillAppearRatio((PassiveSkill_Base.SkillType)i, appearRange);
        }
    }


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

        if (mustLoadPassiveSkills)
        {
            if (passiveSkillUI == null)
            {
                passiveSkillUI = GameObject.Find("Misc Canvas").transform.Find("ChooseAbility").gameObject;
                passiveSkillUI.SetActive(false);
            }
            else
            {
                SavedPassiveSkills save = passiveSkillsSave.LoadSave(LoadPassiveSkills.InGamePath);
                LoadAllSkills(save);
                mustLoadPassiveSkills = false;
            }
        }
    }


    public void AddSkill(PassiveSkill_Base _skill, bool save)
    {
        PassiveSkill_Base skill = skills.Find(currSkill => currSkill.skillType == _skill.skillType);

        if(save)
            passiveSkillsSave.AddElementToSave_InGame(_skill.skillType);

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


    public static PassiveSkill_Base.SkillType GetRndSkillType()
    {
        float rndSkillRatio = UnityEngine.Random.Range(0f, MAX_APPEAR_VALUE - APPEAR_SKILL_THRESHOLD);
        for (int i = 0; i < skillsAppearRatio.Length; i++)
        {
            if (rndSkillRatio <= skillsAppearRatio[i].appearRange.y)
                return skillsAppearRatio[i].skillType;
        }

        return PassiveSkill_Base.SkillType.NONE;
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

            //case PassiveSkill_Base.SkillType.INCREASE_PROJECTILE_AMOUNT:
            //    return new IncreaseProjectileAmount_PassiveSkill();

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
