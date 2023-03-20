using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class Cheats : MonoBehaviour
{
    LifeSystem playerLife;
    bool infiniteLife = false;
    [SerializeField] GameObject cardSelectCheat;
    [SerializeField] Transform preBossLocation;

    private WalkMark walkmark;

    private void Start()
    {
        playerLife = GameObject.FindGameObjectWithTag("Player").GetComponent<LifeSystem>();
        walkmark = GameObject.Find("UI_Walk").GetComponent<WalkMark>();
    }

    // Update is called once per frame
    void Update()
    {

        if (Input.GetKeyDown(KeyCode.F3))
            infiniteLife = !infiniteLife;

        if (Input.GetKeyDown(KeyCode.F4))
            cardSelectCheat.SetActive(true);


        if (Input.GetKeyDown(KeyCode.F5))
            playerLife.Damage(10000, new ElementsManager.Elements());

        if (Input.GetKeyDown(KeyCode.F6))
            CustomSceneManager.Instance.ChangeScene(SceneManager.GetActiveScene().name);

        if (infiniteLife)
        {
            playerLife.currLife = playerLife.maxLife;
        }

        if (Input.GetKeyDown(KeyCode.F7))
        {
            playerLife.transform.position = new Vector3(preBossLocation.transform.position.x, playerLife.transform.position.y, preBossLocation.transform.position.z);
            walkmark.ResetMousePos();
        }
    }

}
