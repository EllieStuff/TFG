using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class Cheats : MonoBehaviour
{
    LifeSystem playerLife;
    bool infiniteLife = false;

    private void Start()
    {
        playerLife = GameObject.FindGameObjectWithTag("Player").GetComponent<LifeSystem>();
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.C))
        {
            SceneManager.LoadSceneAsync(SceneManager.GetActiveScene().name);
        }

        if (Input.GetKeyDown(KeyCode.V))
        {
            infiniteLife = !infiniteLife;
        }
        if (infiniteLife)
        {
            playerLife.currLife = playerLife.maxLife;
        }

    }

}
