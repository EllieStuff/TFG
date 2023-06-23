using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Video;
using UnityEngine.UI;

public class SkipVideo : MonoBehaviour
{
    [SerializeField] string targetScene;
    [SerializeField] VideoClip videoClip;
    [SerializeField] float skipSpeed = 0.5f;
    [SerializeField] VideoPlayer videoPlayer;
    [SerializeField] CanvasGroup skipCG;
    [SerializeField] Slider skipSlider;

    bool changingScene = false;
    bool keyPressed = false;

    // Start is called before the first frame update
    void Start()
    {
        skipCG.alpha = 0f;
        videoPlayer.clip = videoClip;
        if (videoPlayer.playOnAwake) videoPlayer.Play();
        StartCoroutine(ChangeSceneWhenVideoFinished_Cor());
    }

    private void Update()
    {
        if (changingScene) return;

        if (Input.anyKeyDown)
        {
            keyPressed = true;
            StartCoroutine(SkipAppear_Cor());
            return;
        }
        
        if (Input.anyKey)
        {
            skipSlider.value += Time.deltaTime * skipSpeed;
            if(skipSlider.value >= 0.99f)
            {
                changingScene = true;
                CustomSceneManager.Instance.ChangeScene(targetScene);
            }
        }
        else if (keyPressed)
        {
            keyPressed = false;
            StartCoroutine(SkipDisappear_Cor());
        }
    }


    IEnumerator ChangeSceneWhenVideoFinished_Cor()
    {
        yield return new WaitForSeconds((float)videoPlayer.clip.length);
        if (!changingScene)
        {
            changingScene = true;
            CustomSceneManager.Instance.ChangeScene(targetScene);
        }
    }

    IEnumerator SkipAppear_Cor(float _lerpTime = 0.5f)
    {
        float timer = 0;
        while(timer < _lerpTime)
        {
            yield return null;
            timer += Time.deltaTime;
            skipCG.alpha = Mathf.Lerp(0f, 1f, timer / _lerpTime);
        }
    }

    IEnumerator SkipDisappear_Cor(float _lerpTime = 0.5f)
    {
        float timer = 0;
        while (timer < _lerpTime)
        {
            yield return null;
            timer += Time.deltaTime;
            skipCG.alpha = Mathf.Lerp(1f, 0f, timer / _lerpTime);
        }
        if(!keyPressed) skipSlider.value = 0f;
    }

}
