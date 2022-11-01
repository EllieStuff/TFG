using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class RaglotonShield : MonoBehaviour
{
    [SerializeField] Enemy_Ragloton raglotonScript;
    [SerializeField] Collider raglotonCollider;
    [SerializeField] Slider lifeBar;
    [SerializeField] GameObject shieldSetRef;

    Transform cam;
    LifeSystem lifeSystem;
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
        lifeBarGroup = lifeBar.GetComponent<CanvasGroup>();
        lifeSystem = GetComponent<LifeSystem>();

        Quaternion targetRot = Quaternion.LookRotation((cam.position - lifeBar.transform.position).normalized, Vector3.up);
        lifeBar.transform.rotation = Quaternion.RotateTowards(transform.rotation, targetRot, lifeBarRotSpeed);

        shieldLifeCopy = lifeSystem.currLife;
        lifeBar.value = lifeSystem.GetLifePercentage();
        lifeBarGroup.alpha = 0;
        lifeBar.gameObject.SetActive(false);

    }

    private void Update()
    {
        if (shieldLifeCopy != lifeSystem.currLife)
        {
            shieldLifeCopy = lifeSystem.currLife;
            StopAllCoroutines();
            StartCoroutine(UpdateLifeBar());
        }

        if (lifeBar.isActiveAndEnabled)
        {
            Quaternion targetRot = Quaternion.LookRotation((cam.position - lifeBar.transform.position).normalized, Vector3.up);
            lifeBar.transform.rotation = Quaternion.RotateTowards(transform.rotation, targetRot, lifeBarRotSpeed);
        }

    }


    IEnumerator UpdateLifeBar(float _disappearDelay = 3.0f)
    {
        if (!lifeBar.gameObject.activeSelf)
        {
            lifeBar.gameObject.SetActive(true);
            yield return LerpLifeBarAlpha(lifeBarGroup.alpha, 1);
        }

        yield return LerpLifeBarValue(lifeBar.value, lifeSystem.GetLifePercentage());
        if (lifeBar.value <= 0.0001f)
        {
            raglotonScript.hasShield = false;
            Destroy(shieldSetRef);
            StopAllCoroutines();
        }

        yield return new WaitForSeconds(_disappearDelay);

        yield return LerpLifeBarAlpha(1, 0);
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


    private void OnTriggerEnter(Collider other)
    {
        if(other.CompareTag("SwordRegion") || other.CompareTag("Weapon"))
        {
            Physics.IgnoreCollision(other, raglotonCollider, true);
        }
    }

}
