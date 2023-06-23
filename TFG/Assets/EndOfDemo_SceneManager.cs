using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EndOfDemo_SceneManager : MonoBehaviour
{
    [SerializeField] float goToMenuDelay = 5f;


    void Start()
    {
        StartCoroutine(ChangeSceneAfterDelay_Cor());
    }


    IEnumerator ChangeSceneAfterDelay_Cor()
    {
        yield return new WaitForSeconds(goToMenuDelay);
        CustomSceneManager.Instance.ChangeScene("Main Menu");
    }

}
