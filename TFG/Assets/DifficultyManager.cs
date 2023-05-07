using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum DifficultyMode { EASY, NORMAL, HARD }

public class DifficultyManager : MonoBehaviour
{
    static DifficultyMode difficulty = DifficultyMode.NORMAL;
    public static DifficultyMode Difficulty { get { return difficulty; } }


    public static void SetDifficulty(int _difficultyId)
    {
        difficulty = (DifficultyMode)_difficultyId;
    }
    public static void SetDifficulty(DifficultyMode _difficulty)
    {
        difficulty = _difficulty;
    }

}
