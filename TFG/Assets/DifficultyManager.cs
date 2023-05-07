using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public enum DifficultyMode { EASY, NORMAL, HARD }

public class DifficultyManager : MonoBehaviour
{
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
