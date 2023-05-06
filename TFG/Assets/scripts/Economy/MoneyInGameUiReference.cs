using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class MoneyInGameUiReference : MonoBehaviour
{
    public CanvasGroup moneyFeedback;
    public TextMeshProUGUI moneyText;
    public Animator moneyUpAnim;

    internal bool activatingFeedback = false, deactivatingFeedback = false, feedbackActive = true;


    public void ActivateMoneyFeedback(float _duration = 1f, float _delay = 0f)
    {
        activatingFeedback = true;
        feedbackActive = true;
        StopAllCoroutines();
        StartCoroutine(ActivateMoneyFeedback_Cor(moneyFeedback, 1f, _duration, _delay));
    }
    public void DeactivateMoneyFeedback(float _duration = 1f, float _delay = 0f)
    {
        deactivatingFeedback = true;
        feedbackActive = false;
        StopAllCoroutines();
        StartCoroutine(DeactivateMoneyFeedback_Cor(moneyFeedback, 0f, _duration, _delay));
    }


    IEnumerator ActivateMoneyFeedback_Cor(CanvasGroup _canvasGroup, float _targetAlpha, float _duration, float _delay)
    {
        yield return new WaitForSecondsRealtime(_delay);
        float initAlpha = _canvasGroup.alpha;
        float timer = 0f;
        while (timer < _duration)
        {
            yield return new WaitForEndOfFrame();
            timer += Time.unscaledDeltaTime;
            _canvasGroup.alpha = Mathf.Lerp(initAlpha, _targetAlpha, timer / _duration);
        }
        yield return null;
        activatingFeedback = false;
    }
    IEnumerator DeactivateMoneyFeedback_Cor(CanvasGroup _canvasGroup, float _targetAlpha, float _duration, float _delay)
    {
        yield return new WaitForSecondsRealtime(_delay);
        float initAlpha = _canvasGroup.alpha;
        float timer = 0f;
        while (timer < _duration)
        {
            yield return new WaitForEndOfFrame();
            timer += Time.unscaledDeltaTime;
            _canvasGroup.alpha = Mathf.Lerp(initAlpha, _targetAlpha, timer / _duration);
        }
        yield return null;
        deactivatingFeedback = false;
    }

}
