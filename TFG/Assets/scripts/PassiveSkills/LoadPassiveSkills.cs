using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Formatters.Binary;
using UnityEngine;

[Serializable]
public class SavedPassiveSkills
{
    public List<Tuple<PassiveSkill_Base.SkillType, int>> savedElements;
}

public class LoadPassiveSkills : MonoBehaviour
{
    SavedPassiveSkills save;

    private void Start()
    {
        if (GameObject.FindGameObjectsWithTag("save").Length > 1)
            Destroy(gameObject);
        else
        {
            DontDestroyOnLoad(gameObject);
            ResetSave();
            save = LoadSave();
        }

    }

    public void Save()
    {
        IFormatter formatter = new BinaryFormatter();
        Stream stream = new FileStream(Application.dataPath + "/save.dat", FileMode.Open, FileAccess.Write, FileShare.ReadWrite);

        formatter.Serialize(stream, save);
        stream.Close();
    }

    public SavedPassiveSkills LoadSave()
    {
        IFormatter formatter = new BinaryFormatter();
        Stream stream = new FileStream(Application.dataPath + "/save.dat", FileMode.Open, FileAccess.Read, FileShare.ReadWrite);

        if (stream.Length == 0)
        {
            SavedPassiveSkills saveAux = new SavedPassiveSkills();
            saveAux.savedElements = new List<Tuple<PassiveSkill_Base.SkillType, int>>();
            return saveAux;
        }

        SavedPassiveSkills save = (SavedPassiveSkills) formatter.Deserialize(stream);

        stream.Close();

        return save;
    }

    public void ResetSave()
    {
        Stream stream = new FileStream(Application.dataPath + "/save.dat", FileMode.Create, FileAccess.Write);
        stream.Close();
    }

    public void AddElementToSave(PassiveSkill_Base.SkillType _skill)
    {
        bool createNewSkill = true;

        foreach(Tuple<PassiveSkill_Base.SkillType, int> element in save.savedElements)
        {
            if(_skill.Equals(element.Item1))
            {
                PassiveSkill_Base.SkillType skillSaved = element.Item1;
                int savedLevel = element.Item2+1;

                save.savedElements.Remove(element);
                save.savedElements.Add(new Tuple<PassiveSkill_Base.SkillType, int>(skillSaved, savedLevel));

                createNewSkill = false;

                break;
            }
        }

        if(createNewSkill)
            save.savedElements.Add(new Tuple<PassiveSkill_Base.SkillType, int>(_skill, 1));

        Save();
    }
}
