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
    public const string InGamePath = "/inGameSave.dat", ShopPath = "/shopSave.dat";

    SavedPassiveSkills inGameSkillsSave, shopSkillsSave;
    Stream stream;

    private void Awake()
    {
        if (GameObject.FindGameObjectsWithTag("save").Length > 1)
            Destroy(gameObject);
        else
        {
            DontDestroyOnLoad(gameObject);
            ResetSave(InGamePath);
            inGameSkillsSave = LoadSave(InGamePath);
            //ResetSave(ShopPath);
            shopSkillsSave = LoadSave(ShopPath);
        }

    }

    private void OnDestroy()
    {
        if(stream != null)
            stream.Close();
    }

    public void Save_InGame()
    {
        IFormatter formatter = new BinaryFormatter();
        stream = new FileStream(Application.dataPath + InGamePath, FileMode.Open, FileAccess.Write, FileShare.ReadWrite);

        formatter.Serialize(stream, inGameSkillsSave);
        stream.Close();
    }

    public void Save_Shop()
    {
        IFormatter formatter = new BinaryFormatter();
        stream = new FileStream(Application.dataPath + ShopPath, FileMode.Open, FileAccess.Write, FileShare.ReadWrite);

        formatter.Serialize(stream, shopSkillsSave);
        stream.Close();
    }


    public SavedPassiveSkills LoadSave(string _path)
    {
        IFormatter formatter = new BinaryFormatter();

        try 
        {
            stream = new FileStream(Application.dataPath + _path, FileMode.Open, FileAccess.Read, FileShare.ReadWrite);
        }
        catch 
        {
            stream = new FileStream(Application.dataPath + ShopPath, FileMode.Create, FileAccess.ReadWrite);
        }

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

    public void ResetSave(string _path)
    {
        if (stream != null)
            stream.Close();

        File.Delete(Application.dataPath + _path);
        stream = new FileStream(Application.dataPath + _path, FileMode.Create, FileAccess.Write);
        stream.Close();
    }

    [ContextMenu("ResetBoughtSkills")]
    public void ResetBoughtSkills()
    {
        File.Delete(Application.dataPath + ShopPath);
        stream = new FileStream(Application.dataPath + ShopPath, FileMode.Create, FileAccess.Write);
        stream.Close();
    }
    

    public void AddElementToSave_InGame(PassiveSkill_Base.SkillType _skill)
    {
        bool createNewSkill = true;

        foreach(Tuple<PassiveSkill_Base.SkillType, int> element in inGameSkillsSave.savedElements)
        {
            if(_skill.Equals(element.Item1))
            {
                PassiveSkill_Base.SkillType skillSaved = element.Item1;
                int savedLevel = element.Item2+1;

                inGameSkillsSave.savedElements.Remove(element);
                inGameSkillsSave.savedElements.Add(new Tuple<PassiveSkill_Base.SkillType, int>(skillSaved, savedLevel));

                createNewSkill = false;

                break;
            }
        }

        if(createNewSkill)
            inGameSkillsSave.savedElements.Add(new Tuple<PassiveSkill_Base.SkillType, int>(_skill, 1));

        Save_InGame();
    }

    public void AddElementToSave_Shop(PassiveSkill_Base.SkillType _skill)
    {
        bool createNewSkill = true;

        foreach (Tuple<PassiveSkill_Base.SkillType, int> element in shopSkillsSave.savedElements)
        {
            if (_skill.Equals(element.Item1))
            {
                PassiveSkill_Base.SkillType skillSaved = element.Item1;
                int savedLevel = element.Item2 + 1;

                shopSkillsSave.savedElements.Remove(element);
                shopSkillsSave.savedElements.Add(new Tuple<PassiveSkill_Base.SkillType, int>(skillSaved, savedLevel));

                createNewSkill = false;

                break;
            }
        }

        if (createNewSkill)
            shopSkillsSave.savedElements.Add(new Tuple<PassiveSkill_Base.SkillType, int>(_skill, 1));

        Save_Shop();
    }

}
