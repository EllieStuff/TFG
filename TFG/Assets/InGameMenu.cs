using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class InGameMenu : MonoBehaviour
{


    public void Exit()
    {
        Time.timeScale = 1f;
        CustomSceneManager.Instance.ChangeScene("MainMenu Scene");
    }

}
