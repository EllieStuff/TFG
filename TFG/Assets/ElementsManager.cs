using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ElementsManager : MonoBehaviour
{
    public enum Elements { FIRE, GRASS, WATER, NORMAL, COUNT }
    public class ElementClass
    {
        public Dictionary<Elements, float> receiveDamage = new Dictionary<Elements, float>();
        public Dictionary<Elements, float> inflictDamage = new Dictionary<Elements, float>();
        public float chargeElementDelay = 1.5f;
    }

    static Dictionary<Elements, ElementClass> elementsData = null;

    PlayerAttack attackManager;
    PlayerMovement moveManager;
    WalkMark walkMark;
    [SerializeField] Slider changeElementSlider;
    Elements elementChanging = Elements.NORMAL;
    Elements elementIdx;
    [SerializeField] Image nearSliderElementIcon, uiElementIcon;
    [SerializeField] Sprite[] icons;

    public ParticleSystem changeElementBlue;
    public ParticleSystem changeElementGreen;
    public ParticleSystem changeElementRed;
    public ParticleSystem changeElementNeutral;

    // Start is called before the first frame update
    void Awake()
    {
        attackManager = GetComponent<PlayerAttack>();
        moveManager = GetComponent<PlayerMovement>();
        walkMark = FindObjectOfType<WalkMark>();

        elementIdx = attackManager.currentAttackElement;
        nearSliderElementIcon.color = new Color(1, 1, 1, 0);

        if(elementsData == null)
            InitElementsData();
    }

    void InitElementsData()
    {
        elementsData = new Dictionary<Elements, ElementClass>();
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
        KeyboardInputsManager();
        MouseInputsManager();
    }

    void KeyboardInputsManager()
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

    void MouseInputsManager()
    {
        if (Input.GetAxis("Mouse ScrollWheel") < 0f)
        {
            StopParticles(elementIdx);
            elementIdx--;
            if (elementIdx < 0) elementIdx = Elements.COUNT - 1;
            ChangeElement(elementIdx, elementsData[elementIdx].chargeElementDelay);

        }
        else if (Input.GetAxis("Mouse ScrollWheel") > 0f)
        {
            StopParticles(elementIdx);
            elementIdx++;
            if (elementIdx >= Elements.COUNT) elementIdx = 0;
            ChangeElement(elementIdx, elementsData[elementIdx].chargeElementDelay);
        }
    }


    void ChangeElement(Elements _element, float _changeAttackDelay)
    {
        PlayParticles(_element);
        elementChanging = _element;
        StartCoroutine(ChangeElementCor(_element, _changeAttackDelay));
    }

    IEnumerator ChangeElementCor(Elements _element, float _changeAttackDelay)
    {
        attackManager.canAttack = moveManager.canMove = false;
        attackManager.SetAttackTimer(attackManager.attackDelay);
        moveManager.targetMousePos = Vector3.zero;
        walkMark.SetWalkMarkActive(false);
        nearSliderElementIcon.sprite = icons[(int)_element];
        StartCoroutine(LerpImageAlpha(nearSliderElementIcon, 0, 1, 0.3f));

        float timer = 0, maxTime = _changeAttackDelay;
        while(timer < maxTime)
        {
            yield return new WaitForEndOfFrame();
            if (elementChanging != _element) 
            {
                StopParticles(_element);
                yield break;  
            }
            else if (moveManager.targetMousePos != Vector3.zero /*!moveManager.Moving*/)
            {
                StartCoroutine(LerpImageAlpha(nearSliderElementIcon, 1, 0, 0.3f));
                attackManager.canAttack = moveManager.canMove = true;
                elementIdx = attackManager.currentAttackElement;
                changeElementSlider.value = 0;
                StopParticles(_element);
                yield break;
            }

            timer += Time.deltaTime;
            changeElementSlider.value = Mathf.Lerp(1, 0, timer / maxTime);
        }

        StartCoroutine(LerpImageAlpha(nearSliderElementIcon, 1, 0, 0.3f));
        //yield return new WaitForEndOfFrame();
        //if (elementChanging != _element) yield break;
        changeElementSlider.value = 0;
        attackManager.currentAttackElement = _element;
        attackManager.canAttack = moveManager.canMove = true;
        //uiElementIcon.sprite = icons[(int)_element];
        attackManager.SetAttackTimer(attackManager.attackDelay / 4f);
    }

    IEnumerator LerpImageAlpha(Image _image, float _initAlpha, float _targetAlpha, float _lerpTime = 0.5f)
    {
        float timer = 0, maxTime = _lerpTime;
        while (timer < maxTime)
        {
            yield return new WaitForEndOfFrame();
            timer += Time.deltaTime;
            _image.color = new Color(_image.color.r, _image.color.g, _image.color.b, Mathf.Lerp(_initAlpha, _targetAlpha, timer / maxTime));
        }
        yield return new WaitForEndOfFrame();
        _image.color = new Color(_image.color.r, _image.color.g, _image.color.b, _targetAlpha);
    }


    public static float GetReceiveDamageMultiplier(Elements _entityElement, Elements _damageElement)
    {
        return elementsData[_entityElement].receiveDamage[_damageElement];
    }
    public static float GetInflictDamageMultiplier(Elements _entityElement, Elements _damageElement)
    {
        return elementsData[_entityElement].inflictDamage[_damageElement];
    }

    void StopParticles(Elements _element)
    {
        switch (_element)
        {
            case Elements.FIRE:
                changeElementRed.Stop();
                changeElementRed.Clear();
                break;
            case Elements.WATER:
                changeElementBlue.Stop();
                changeElementBlue.Clear();
                break;
            case Elements.GRASS:
                changeElementGreen.Stop();
                changeElementGreen.Clear();
                break;
            case Elements.NORMAL:
                changeElementNeutral.Stop();
                changeElementNeutral.Clear();
                break;
            default: break;

        };
        
    }

    void PlayParticles(Elements _element)
    {
        switch (_element)
        {
            case Elements.FIRE:
                changeElementRed.Play();
                break;
            case Elements.WATER:
                changeElementBlue.Play();
                break;
            case Elements.GRASS:
                changeElementGreen.Play();
                break;
            case Elements.NORMAL:
                changeElementNeutral.Play();
                break;
            default: break;

        };

    }

}
