using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class EliTween
{
    enum TweenType { SCALE, COLOR }
    static Dictionary<MonoBehaviour, Dictionary<TweenType, Coroutine>> activeCoroutines = new Dictionary<MonoBehaviour, Dictionary<TweenType, Coroutine>>();

    static void CheckActiveCoroutines(MonoBehaviour _mb, TweenType _tt, Coroutine _cor)
    {
        if (activeCoroutines.ContainsKey(_mb))
        {
            if (activeCoroutines[_mb].ContainsKey(_tt))
            {
                if (activeCoroutines[_mb][_tt] != null)
                {
                    _mb.StopCoroutine(activeCoroutines[_mb][_tt]);
                }
                activeCoroutines[_mb][_tt] = _cor;
            }
            else
            {
                activeCoroutines[_mb].Add(_tt, _cor);
            }
        }
        else
        {
            activeCoroutines.Add(_mb, new Dictionary<TweenType, Coroutine> { { _tt, _cor } });
        }
    }
    static void EraseActiveCoroutine(MonoBehaviour _mb, TweenType _tt)
    {
        //if (activeCoroutines.ContainsKey(_mb) && activeCoroutines[_mb].ContainsKey(_tt)) 
        activeCoroutines[_mb].Remove(_tt);
        if (activeCoroutines[_mb].Count == 0) 
            activeCoroutines.Remove(_mb);
    }

    public static void Scale(Transform _transform, Vector3 _targetSize, float _duration, float _delay = 0f)
    {
        MonoBehaviour mb = _transform.GetComponent<MonoBehaviour>();
        Coroutine cor = mb.StartCoroutine(ScaleCor(_transform, _targetSize, _duration, _delay));
        //CheckActiveCoroutines(mb, TweenType.SCALE, cor);
    }
    static IEnumerator ScaleCor(Transform _transform, Vector3 _targetSize, float _duration, float _delay)
    {
        yield return new WaitForSeconds(_delay);
        Vector3 initSize = _transform.localScale;
        float timer = 0f;
        while (timer < _duration)
        {
            yield return null;
            timer += Time.deltaTime;
            _transform.localScale = Vector3.Lerp(initSize, _targetSize, timer / _duration);
        }
        yield return null;
        //EraseActiveCoroutine(_transform.GetComponent<MonoBehaviour>(), TweenType.SCALE);
    }

    public static void ChangeColor(Image _image, Color _targetColor, float _duration, float _delay = 0f)
    {
        Coroutine cor = _image.StartCoroutine(LerpImageColor(_image, _targetColor, _duration, _delay));
        //CheckActiveCoroutines(_image, TweenType.SCALE, cor);
    }
    static IEnumerator LerpImageColor(Image _image, Color _targetColor, float _duration, float _delay)
    {
        yield return new WaitForSeconds(_delay);
        Color initColor = _image.color;
        float timer = 0f;
        while (timer < _duration)
        {
            yield return new WaitForEndOfFrame();
            timer += Time.deltaTime;
            _image.color = Color.Lerp(initColor, _targetColor, timer / _duration);
        }
        yield return null;
        //EraseActiveCoroutine(_image, TweenType.COLOR);
    }

}
