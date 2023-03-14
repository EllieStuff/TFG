using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.EventSystems;

public class DeathScreenManager : MonoBehaviour
{
    const float APPEAR_DELAY = 1f;
    //enum Options { TRY_AGAIN, EXIT, COUNT }

    [SerializeField] CanvasGroup bg, gameOverText, continueBttn, startOverBttn, exitBttn;


    //Button[] bttns;
    //int idx = 0;
    //bool menuActive = false;


    private void Awake()
    {
        if (PlayerPrefs.GetString("UseDeathRoomCoordinates", "false") == "true")
        {
            Transform player = GameObject.FindGameObjectWithTag("Player").transform;
            player.position = new Vector3(0f, player.position.y, PlayerPrefs.GetFloat("DeathRoomZ", -1f));
            PlayerPrefs.SetString("UseDeathRoomCoordinates", "false");
        }

        //bttns = new Button[(int)Options.COUNT];
        //bttns[(int)Options.TRY_AGAIN] = tryAgainBttn.GetComponent<Button>();
        //bttns[(int)Options.EXIT] = exitBttn.GetComponent<Button>();

        ///DeathScreenAppear(1);
    }



    public void Continue()
    {
        Time.timeScale = 1;
        PlayerPrefs.SetString("UseDeathRoomCoordinates", "true");
        CustomSceneManager.Instance.ChangeScene(SceneManager.GetActiveScene().name);
    }

    public void TryAgain()
    {
        Time.timeScale = 1;
        CustomSceneManager.Instance.ChangeScene(SceneManager.GetActiveScene().name);
    }

    public void Exit()
    {
        Time.timeScale = 1;
        CustomSceneManager.Instance.ChangeScene("MainMenu Scene");
    }


    public void DeathScreenAppear(float _delay = APPEAR_DELAY)
    {
        continueBttn.gameObject.SetActive(true);
        continueBttn.alpha = 0f;
        StartCoroutine(DeathScreenAppearCor(_delay));
    }

    IEnumerator DeathScreenAppearCor(float _delay)
    {
        yield return new WaitForSeconds(_delay);
        Time.timeScale = 0f;
        //menuActive = true;

        bg.gameObject.SetActive(true);
        yield return ChangeGroupAlpha(bg, 0, 1);

        gameOverText.gameObject.SetActive(true);
        yield return ChangeGroupAlpha(gameOverText, 0, 1, 0.5f);

        yield return new WaitForSecondsRealtime(0.5f);

        continueBttn.gameObject.SetActive(true);
        StartCoroutine(ChangeGroupAlpha(continueBttn, 0, 1, 0.5f));
        startOverBttn.gameObject.SetActive(true);
        yield return ChangeGroupAlpha(startOverBttn, 0, 1, 0.5f);

        yield return new WaitForSecondsRealtime(0.5f);

        exitBttn.gameObject.SetActive(true);
        yield return ChangeGroupAlpha(exitBttn, 0, 1, 0.5f);

        yield return new WaitForSecondsRealtime(0.5f);
        EventSystem.current.SetSelectedGameObject(null);
        EventSystem.current.SetSelectedGameObject(continueBttn.gameObject);


        //Time.timeScale = 1f;
    }


    IEnumerator ChangeGroupAlpha(CanvasGroup _group, float _initAlpha, float _finalAlpha, float _lerpTime = 1f)
    {
        _group.alpha = _initAlpha;
        float timer = 0f, lerpRefreshFreq = 0.03f;
        while(timer < _lerpTime)
        {
            yield return new WaitForSecondsRealtime(lerpRefreshFreq);
            timer += lerpRefreshFreq;
            _group.alpha = Mathf.Lerp(_initAlpha, _finalAlpha, timer / _lerpTime);
        }
        yield return new WaitForSecondsRealtime(lerpRefreshFreq);
        _group.alpha = _finalAlpha;
    }

}
