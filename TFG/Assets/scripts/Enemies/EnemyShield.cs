using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class EnemyShield : MonoBehaviour
{
    [SerializeField] Enemy_Ragloton raglotonScript;
    [SerializeField] Slider lifeBar;

    Transform cam;
    //LifeSystem lifeSystem;
    CanvasGroup lifeBarGroup;
    float shieldLifeCopy;
    float lifeBarRotSpeed = 1000;

    /// Això hauria d'anar al sistema de vida del Xavi i agafar la referència d'allí
    //[SerializeField] float shieldInitialLife = 10;
    //float shieldLife;

    public bool OnAttack { get { return raglotonScript.isAttacking; } }


    // Start is called before the first frame update
    void Start()
    {
        cam = GameObject.FindGameObjectWithTag("MainCamera").transform;
        Quaternion targetRot = Quaternion.LookRotation((cam.position - lifeBar.transform.position).normalized, Vector3.up);
        lifeBar.transform.rotation = Quaternion.RotateTowards(transform.rotation, targetRot, lifeBarRotSpeed);
        //lifeSystem = GetComponent<LifeSystem>();
        //shieldLifeCopy = lifeSystem.currLife;
        //lifeBar.value = lifeSystem.GetLifePercentage();
    }

    private void Update()
    {
        //if(shieldLifeCopy != lifeSystem.currLife)
        //{
        //    shieldLifeCopy = lifeSystem.currLife;
        //    StopAllCoroutines();
        //    StartCoroutine(UpdateLifeBar());
        //}

        if (lifeBar.isActiveAndEnabled)
        {
            Quaternion targetRot = Quaternion.LookRotation((cam.position - lifeBar.transform.position).normalized, Vector3.up);
            lifeBar.transform.rotation = Quaternion.RotateTowards(transform.rotation, targetRot, lifeBarRotSpeed);
        }

    }


    IEnumerator UpdateLifeBar(float _disappearDelay = 2.0f)
    {
        if (!lifeBar.gameObject.activeSelf)
        {
            lifeBar.gameObject.SetActive(true);
            yield return LerpLifeBarAlpha(0, 1);
        }

        //yield return LerpLifeBarValue(lifeBar.value, lifeSystem.GetLifePercentage());
        if (lifeBar.value <= 0.0001f) Destroy(gameObject);

        yield return new WaitForSeconds(_disappearDelay);

        yield return LerpLifeBarAlpha(1, 0, 0.5f);
        lifeBar.gameObject.SetActive(false);
    }
    IEnumerator LerpLifeBarValue(float _initValue, float _targetValue, float _lerpTime = 0.2f)
    {
        lifeBar.value = _initValue;
        float timer = 0, maxTime = _lerpTime;
        while (timer < maxTime)
        {
            yield return new WaitForEndOfFrame();
            timer += Time.deltaTime;
            lifeBar.value = Mathf.Lerp(_initValue, _targetValue, timer / maxTime);
        }
        yield return new WaitForEndOfFrame();
        lifeBar.value = _targetValue;
    }
    IEnumerator LerpLifeBarAlpha(float _initValue, float _targetValue, float _lerpTime = 0.2f)
    {
        lifeBarGroup.alpha = _initValue;
        float timer = 0, maxTime = _lerpTime;
        while(timer < maxTime)
        {
            yield return new WaitForEndOfFrame();
            timer += Time.deltaTime;
            lifeBarGroup.alpha = Mathf.Lerp(_initValue, _targetValue, timer / maxTime);
        }
        yield return new WaitForEndOfFrame();
        lifeBarGroup.alpha = _targetValue;
    }

}
