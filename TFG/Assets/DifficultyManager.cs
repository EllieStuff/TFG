using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public enum DifficultyMode { EASY, NORMAL, HARD }

public class DifficultyManager : MonoBehaviour
{
    public const float 
        Enemies_AtkDmgMultiplier_EasyMode = 0.7f, 
        Enemies_AtkDmgMultiplier_NormalMode = 1f, 
        Enemies_AtkDmgMultiplier_HardMode = 1.3f;
    public const float 
        Enemies_AtkWaitMultiplier_EasyMode = 1.3f, 
        Enemies_AtkWaitMultiplier_NormalMode = 1f, 
        Enemies_AtkWaitMultiplier_HardMode = 0.7f;
    public const float 
        Enemies_LifeMultiplier_EasyMode = 0.7f, 
        Enemies_LifeMultiplier_NormalMode = 1f, 
        Enemies_LifeMultiplier_HardMode = 1.3f;

    public TMP_Dropdown dropdown;

    static DifficultyMode difficulty = DifficultyMode.NORMAL;
    public static DifficultyMode Difficulty { get { return difficulty; } }


    private void Start()
    {
        if (dropdown.gameObject.activeInHierarchy)
        {
            dropdown.value = PlayerPrefs.GetInt("DifficultyMode", 1);
        }
        else
        {
            difficulty = (DifficultyMode)PlayerPrefs.GetInt("DifficultyMode", 1);
        }
    }

    public void SetDifficulty()
    {
        difficulty = (DifficultyMode)dropdown.value;
    }

}
