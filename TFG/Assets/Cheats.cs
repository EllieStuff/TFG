using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class Cheats : MonoBehaviour
{
    LifeSystem playerLife;
    bool infiniteLife = false;
    [SerializeField] GameObject cardSelectCheat;

    private void Start()
    {
        playerLife = GameObject.FindGameObjectWithTag("Player").GetComponent<LifeSystem>();
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.F1))
            CustomSceneManager.Instance.ChangeScene(SceneManager.GetActiveScene().name);

        if (Input.GetKeyDown(KeyCode.F2))
            infiniteLife = !infiniteLife;

        if (Input.GetKeyDown(KeyCode.F3))
            cardSelectCheat.SetActive(true);

        if (infiniteLife)
        {
            playerLife.currLife = playerLife.maxLife;
        }
    }

}
