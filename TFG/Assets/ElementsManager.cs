using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ElementsManager : MonoBehaviour
{
    public enum Elements { NORMAL, FIRE, GRASS, WATER }
    public class ElementClass
    {
        public Dictionary<Elements, float> receiveDamage = new Dictionary<Elements, float>();
        public Dictionary<Elements, float> inflictDamage = new Dictionary<Elements, float>();
        public float chargeElementDelay = 1.5f;
    }

    static Dictionary<Elements, ElementClass> elementsData = new Dictionary<Elements, ElementClass>();

    PlayerAttack attackManager;
    PlayerMovement moveManager;
    WalkMark walkMark;
    [SerializeField] Slider changeElementSlider;
    Elements elementChanging = Elements.NORMAL;


    // Start is called before the first frame update
    void Awake()
    {
        attackManager = GetComponent<PlayerAttack>();
        moveManager = GetComponent<PlayerMovement>();
        walkMark = FindObjectOfType<WalkMark>();

        InitElementsData();
    }

    void InitElementsData()
    {
        ElementClass normalCompatibility = new ElementClass();
        normalCompatibility.receiveDamage.Add(Elements.NORMAL, 1.3f);
        normalCompatibility.receiveDamage.Add(Elements.FIRE, 1.3f);
        normalCompatibility.receiveDamage.Add(Elements.GRASS, 1.3f);
        normalCompatibility.receiveDamage.Add(Elements.WATER, 1.3f);
        normalCompatibility.inflictDamage.Add(Elements.NORMAL, 1.3f);
        normalCompatibility.inflictDamage.Add(Elements.FIRE, 1.3f);
        normalCompatibility.inflictDamage.Add(Elements.GRASS, 1.3f);
        normalCompatibility.inflictDamage.Add(Elements.WATER, 1.3f);
        elementsData.Add(Elements.NORMAL, normalCompatibility);

        ElementClass fireCompatibility = new ElementClass();
        fireCompatibility.receiveDamage.Add(Elements.NORMAL, 0.8f);
        fireCompatibility.receiveDamage.Add(Elements.FIRE, 1.0f);
        fireCompatibility.receiveDamage.Add(Elements.GRASS, 0.5f);
        fireCompatibility.receiveDamage.Add(Elements.WATER, 2.0f);
        fireCompatibility.inflictDamage.Add(Elements.NORMAL, 0.8f);
        fireCompatibility.inflictDamage.Add(Elements.FIRE, 1.0f);
        fireCompatibility.inflictDamage.Add(Elements.GRASS, 2.0f);
        fireCompatibility.inflictDamage.Add(Elements.WATER, 0.5f);
        elementsData.Add(Elements.FIRE, fireCompatibility);

        ElementClass grassCompatibility = new ElementClass();
        grassCompatibility.receiveDamage.Add(Elements.NORMAL, 0.8f);
        grassCompatibility.receiveDamage.Add(Elements.FIRE, 2.0f);
        grassCompatibility.receiveDamage.Add(Elements.GRASS, 1.0f);
        grassCompatibility.receiveDamage.Add(Elements.WATER, 0.5f);
        grassCompatibility.inflictDamage.Add(Elements.NORMAL, 0.8f);
        grassCompatibility.inflictDamage.Add(Elements.FIRE, 0.5f);
        grassCompatibility.inflictDamage.Add(Elements.GRASS, 1.0f);
        grassCompatibility.inflictDamage.Add(Elements.WATER, 2.0f);
        elementsData.Add(Elements.GRASS, grassCompatibility);

        ElementClass waterCompatibility = new ElementClass();
        waterCompatibility.receiveDamage.Add(Elements.NORMAL, 0.8f);
        waterCompatibility.receiveDamage.Add(Elements.FIRE, 0.5f);
        waterCompatibility.receiveDamage.Add(Elements.GRASS, 2.0f);
        waterCompatibility.receiveDamage.Add(Elements.WATER, 1.0f);
        waterCompatibility.inflictDamage.Add(Elements.NORMAL, 0.8f);
        waterCompatibility.inflictDamage.Add(Elements.FIRE, 2.0f);
        waterCompatibility.inflictDamage.Add(Elements.GRASS, 0.5f);
        waterCompatibility.inflictDamage.Add(Elements.WATER, 1.0f);
        elementsData.Add(Elements.WATER, waterCompatibility);
    }


    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Alpha1))
        {
            ChangeElement(Elements.FIRE, elementsData[Elements.FIRE].chargeElementDelay);
        }
        else if (Input.GetKeyDown(KeyCode.Alpha2))
        {
            ChangeElement(Elements.WATER, elementsData[Elements.WATER].chargeElementDelay);
        }
        else if (Input.GetKeyDown(KeyCode.Alpha3))
        {
            ChangeElement(Elements.GRASS, elementsData[Elements.GRASS].chargeElementDelay);
        }
        else if (Input.GetKeyDown(KeyCode.Alpha4))
        {
            ChangeElement(Elements.NORMAL, elementsData[Elements.NORMAL].chargeElementDelay);
        }
    }

    void ChangeElement(Elements _element, float _changeAttackDelay)
    {
        elementChanging = _element;
        StartCoroutine(ChangeElementCor(_element, _changeAttackDelay));
    }

    IEnumerator ChangeElementCor(Elements _element, float _changeAttackDelay)
    {
        attackManager.canAttack = moveManager.canMove = false;
        moveManager.targetMousePos = Vector3.zero;
        walkMark.SetWalkMarkActive(false);
        float timer = 0, maxTime = _changeAttackDelay;
        while(timer < maxTime)
        {
            yield return new WaitForEndOfFrame();
            if (elementChanging != _element) yield break;
            else if (moveManager.targetMousePos != Vector3.zero)
            {
                attackManager.canAttack = moveManager.canMove = true;
                changeElementSlider.value = 0;
                yield break;
            }

            timer += Time.deltaTime;
            changeElementSlider.value = Mathf.Lerp(1, 0, timer / maxTime);
        }

        yield return new WaitForEndOfFrame();
        if (elementChanging != _element) yield break;
        changeElementSlider.value = 0;
        attackManager.currentAttackElement = _element;
        attackManager.canAttack = moveManager.canMove = true;
        attackManager.SetAttackTimer(attackManager.attackDelay / 2f);
    }


    public static float GetReceiveDamageMultiplier(Elements _entityElement, Elements _damageElement)
    {
        return elementsData[_entityElement].receiveDamage[_damageElement];
    }
    public static float GetInflictDamageMultiplier(Elements _entityElement, Elements _damageElement)
    {
        return elementsData[_entityElement].inflictDamage[_damageElement];
    }


}
