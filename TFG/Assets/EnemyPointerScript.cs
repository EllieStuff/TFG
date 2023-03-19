using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyPointerScript : MonoBehaviour
{
    const float HEIGHT_MARGIN = 1.5f;

    Transform target;

    Material mat;
    float rotSpeed = 80f;
    Color originalColor;

    public Transform Target { get { return target; } }

    private void Awake()
    {
        mat = GetComponent<MeshRenderer>().material;
        originalColor = mat.color;
        mat.color = Color.clear;
    }

    // Update is called once per frame
    void Update()
    {
        if (target != null && gameObject.activeInHierarchy && mat.color != Color.clear)
        {
            transform.position = new Vector3(target.position.x, target.position.y + target.localScale.y * HEIGHT_MARGIN, target.position.z);
            Vector3 eulerRot = transform.rotation.eulerAngles;
            transform.rotation = Quaternion.Euler(new Vector3(eulerRot.x, eulerRot.y + rotSpeed * Time.deltaTime, eulerRot.z));
        }
    }


    public void SetTarget(Transform _target)
    {
        target = _target;
    }
    public bool IsDifferentTarget(Transform _target)
    {
        return _target != target;
    }

    //public void Appear()
    //{
    //    StopAllCoroutines();
    //    StartCoroutine(Appear_Cor());
    //}
    //public void DisAppear()
    //{
    //    StopAllCoroutines();
    //    StartCoroutine(DisAppear_Cor());
    //}


    public IEnumerator Appear_Cor(float _lerpTime = 0.2f)
    {
        Color initColor = mat.color;
        float timer = 0f;
        while(timer < _lerpTime)
        {
            yield return null;
            timer += Time.deltaTime;
            mat.color = Color.Lerp(initColor, originalColor, timer / _lerpTime);
        }
    }
    public IEnumerator Disappear_Cor(float _lerpTime = 0.2f)
    {
        Color initColor = mat.color;
        float timer = 0f;
        while (timer < _lerpTime)
        {
            yield return null;
            timer += Time.deltaTime;
            mat.color = Color.Lerp(initColor, Color.clear, timer / _lerpTime);
        }
    }

}
