using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class OnboardingChangeScene : MonoBehaviour
{
    private void OnTriggerEnter(Collider other)
    {
        bool isPlayer = other.tag.Equals("Player");
        if (isPlayer)
        {
            CustomSceneManager.Instance.ChangeScene("Albert Scene");
        }
    }
}
