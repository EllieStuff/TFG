using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class EliTween
{

    public static void Scale(Transform _transform, Vector3 _targetSize, float _duration, float _delay = 0f)
    {
        MonoBehaviour mb = _transform.GetComponent<MonoBehaviour>();
        mb.StartCoroutine(Scale_Cor(_transform, _targetSize, _duration, _delay));
    }
    static IEnumerator Scale_Cor(Transform _transform, Vector3 _targetSize, float _duration, float _delay)
    {
        yield return new WaitForSecondsRealtime(_delay);
        Vector3 initSize = _transform.localScale;
        float timer = 0f;
        while (timer < _duration)
        {
            yield return null;
            timer += Time.unscaledDeltaTime;
            _transform.localScale = Vector3.Lerp(initSize, _targetSize, timer / _duration);
        }
        yield return null;
    }

    public static void ChangeColor(Image _image, Color _targetColor, float _duration, float _delay = 0f)
    {

        //GameObject parent = _image.transform.parent.parent.parent.gameObject;
        //if (parent.activeSelf || parent == null)
            _image.StartCoroutine(ChangeColor_Cor(_image, _targetColor, _duration, _delay));
    }
    static IEnumerator ChangeColor_Cor(Image _image, Color _targetColor, float _duration, float _delay)
    {
        yield return new WaitForSecondsRealtime(_delay);
        Color initColor = _image.color;
        float timer = 0f;
        while (timer < _duration)
        {
            yield return new WaitForEndOfFrame();
            timer += Time.unscaledDeltaTime;
            _image.color = Color.Lerp(initColor, _targetColor, timer / _duration);
        }
        yield return null;
    }


    public static void ChangeAlpha(CanvasGroup _canvasGroup, float _targetAlpha, float _duration, float _delay = 0f)
    {
        _canvasGroup.GetComponent<MonoBehaviour>().StartCoroutine(ChangeAlpha_Cor(_canvasGroup, _targetAlpha, _duration, _delay));
    }
    static IEnumerator ChangeAlpha_Cor(CanvasGroup _canvasGroup, float _targetAlpha, float _duration, float _delay)
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
    }

}
