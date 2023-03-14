using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CustomSceneManager : MonoBehaviour
{
    [SerializeField] Image fader, loadingIcon;
    [SerializeField] float fadeDelay = 1f;

    float loadingIconSpeed = -50f;

    public static CustomSceneManager Instance = null;


    private void Start()
    {
        if (Instance == null) Instance = this;

        StartCoroutine(FaderStart_Cor());
    }


    public void ChangeScene(string _sceneName)
    {
        StopAllCoroutines();
        StartCoroutine(ChangeScene_Cor(_sceneName));
    }
    public void ChangeScene(int _sceneId)
    {
        StopAllCoroutines();
        StartCoroutine(ChangeScene_Cor(_sceneId));
    }


    IEnumerator ChangeScene_Cor(string _sceneName)
    {
        fader.gameObject.SetActive(true);
        yield return LerpImageColor_Cor(fader, Color.clear, Color.black);
        loadingIcon.gameObject.SetActive(true);
        yield return LerpImageColor_Cor(loadingIcon, Color.clear, Color.white);
        StartCoroutine(RotateLoadingIcon_Cor());
        UnityEngine.SceneManagement.SceneManager.LoadSceneAsync(_sceneName);
    }
    IEnumerator ChangeScene_Cor(int _sceneId)
    {
        fader.gameObject.SetActive(true);
        yield return LerpImageColor_Cor(fader, Color.clear, Color.black);
        loadingIcon.gameObject.SetActive(true);
        yield return LerpImageColor_Cor(loadingIcon, Color.clear, Color.white);
        StartCoroutine(RotateLoadingIcon_Cor());
        UnityEngine.SceneManagement.SceneManager.LoadSceneAsync(_sceneId);
    }


    IEnumerator FaderStart_Cor()
    {
        fader.gameObject.SetActive(true);
        //loadingIcon.gameObject.SetActive(true);
        //yield return LerpImageColor_Cor(loadingIcon, Color.white, Color.clear);
        yield return new WaitForSeconds(fadeDelay);
        yield return LerpImageColor_Cor(fader, Color.black, Color.clear, 2f);
        fader.gameObject.SetActive(false);
        loadingIcon.gameObject.SetActive(false);
    }


    IEnumerator RotateLoadingIcon_Cor()
    {
        while (loadingIcon.gameObject.activeSelf)
        {
            yield return null;
            loadingIcon.transform.Rotate(new Vector3(0f, 0f, loadingIconSpeed * Time.unscaledDeltaTime));
        }
    }


    IEnumerator LerpImageColor_Cor(Image _image, Color _initColor, Color _targetColor, float _lerpTime = 0.5f)
    {
        _image.color = _initColor;
        float timer = 0f;
        while(timer < _lerpTime)
        {
            yield return null;
            timer += Time.unscaledDeltaTime;
            _image.color = Color.Lerp(_initColor, _targetColor, timer / _lerpTime);
        }
    }

}
