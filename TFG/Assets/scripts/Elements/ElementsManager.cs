using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using FMOD.Studio;

public class ElementsManager : MonoBehaviour
{
    const float
        EFFECTIVE_HIT_MULTIPLIER = 2.0f,
        SAME_ELEMENT_HIT_MULTIPLIER = 0.7f,
        NOT_EFFECTIVE_HIT_MULTIPLIER = 0.1f,
        NEUTRAL_ELEMENT_MULTIPLIER = 1.0f;

    public enum Elements { FIRE, GRASS, WATER, NEUTRAL, COUNT }
    public class ElementClass
    {
        public Dictionary<Elements, float> receiveDamage = new Dictionary<Elements, float>();
        //public Dictionary<Elements, float> inflictDamage = new Dictionary<Elements, float>();
        public Color colorParticles;
    }

    static Dictionary<Elements, ElementClass> elementsData = null;

    PlayerAttack attackManager;
    LifeSystem playerLife;
    [SerializeField] bool allowElementChangeOnceStarted = false;
    bool changingElement = false;
    public bool tutorialDone = false;
    [SerializeField] Slider changeElementSlider;
    Elements elementChanging = Elements.FIRE;
    Elements elementIdx;
    [SerializeField] Image nearSliderElementIcon;
    [SerializeField] Sprite[] icons;
    [SerializeField] ParticleSystem effectTypeParticles;
    [SerializeField] ParticleSystem effectTypeParticles_UI;

    public ParticleSystem changeElementBlue;
    public ParticleSystem changeElementGreen;
    public ParticleSystem changeElementRed;
    public ParticleSystem changeElementNeutral;

    bool CanChangeElement { get { return !(!allowElementChangeOnceStarted && changingElement); } }
    public bool ChangingElement { get { return changingElement; } }

    private RadialRotation UIRadialRot;
    internal bool mouseClockWise = true;

    //AUDIO
    EventInstance uiChangeElement;

    // Start is called before the first frame update
    void Awake()
    {
        attackManager = GetComponent<PlayerAttack>();
        playerLife = GetComponent<LifeSystem>();
        UIRadialRot = GameObject.FindGameObjectWithTag("Radial").GetComponent<RadialRotation>();
        //moveManager = GetComponent<PlayerMovement>();
        //walkMark = FindObjectOfType<WalkMark>();

        elementIdx = attackManager.currentAttackElement;
        nearSliderElementIcon.color = new Color(1, 1, 1, 0);

        if(elementsData == null)
            InitElementsData();
    }

    private void Start()
    {
        //AUDIO
        uiChangeElement = AudioManager.instance.CreateInstance(FMODEvents.instance.uiChangeElement);
    }

    void InitElementsData()
    {
        elementsData = new Dictionary<Elements, ElementClass>();
        ElementClass normalCompatibility = new ElementClass();
        normalCompatibility.colorParticles = Color.white;
        normalCompatibility.receiveDamage.Add(Elements.NEUTRAL, NEUTRAL_ELEMENT_MULTIPLIER);
        normalCompatibility.receiveDamage.Add(Elements.FIRE, NEUTRAL_ELEMENT_MULTIPLIER);
        normalCompatibility.receiveDamage.Add(Elements.GRASS, NEUTRAL_ELEMENT_MULTIPLIER);
        normalCompatibility.receiveDamage.Add(Elements.WATER, NEUTRAL_ELEMENT_MULTIPLIER);
        elementsData.Add(Elements.NEUTRAL, normalCompatibility);

        ElementClass fireCompatibility = new ElementClass();
        fireCompatibility.colorParticles = Color.red;
        fireCompatibility.receiveDamage.Add(Elements.NEUTRAL, NEUTRAL_ELEMENT_MULTIPLIER);
        fireCompatibility.receiveDamage.Add(Elements.FIRE, SAME_ELEMENT_HIT_MULTIPLIER);
        fireCompatibility.receiveDamage.Add(Elements.GRASS, NOT_EFFECTIVE_HIT_MULTIPLIER);
        fireCompatibility.receiveDamage.Add(Elements.WATER, EFFECTIVE_HIT_MULTIPLIER);
        elementsData.Add(Elements.FIRE, fireCompatibility);

        ElementClass grassCompatibility = new ElementClass();
        grassCompatibility.colorParticles = Color.green;
        grassCompatibility.receiveDamage.Add(Elements.NEUTRAL, NEUTRAL_ELEMENT_MULTIPLIER);
        grassCompatibility.receiveDamage.Add(Elements.FIRE, EFFECTIVE_HIT_MULTIPLIER);
        grassCompatibility.receiveDamage.Add(Elements.GRASS, SAME_ELEMENT_HIT_MULTIPLIER);
        grassCompatibility.receiveDamage.Add(Elements.WATER, NOT_EFFECTIVE_HIT_MULTIPLIER);
        elementsData.Add(Elements.GRASS, grassCompatibility);

        ElementClass waterCompatibility = new ElementClass();
        waterCompatibility.colorParticles = Color.cyan;
        waterCompatibility.receiveDamage.Add(Elements.NEUTRAL, NEUTRAL_ELEMENT_MULTIPLIER);
        waterCompatibility.receiveDamage.Add(Elements.FIRE, NOT_EFFECTIVE_HIT_MULTIPLIER);
        waterCompatibility.receiveDamage.Add(Elements.GRASS, EFFECTIVE_HIT_MULTIPLIER);
        waterCompatibility.receiveDamage.Add(Elements.WATER, SAME_ELEMENT_HIT_MULTIPLIER);
        elementsData.Add(Elements.WATER, waterCompatibility);

        SwitchElementParticles(elementIdx);
    }


    private void Update()
    {
        if (playerLife.isDead) return;
        KeyboardInputsManager();
        MouseInputsManager();
    }

    void KeyboardInputsManager()
    {
        if (!CanChangeElement || !tutorialDone) return;

        if (Input.GetKeyDown(KeyCode.Alpha1) && attackManager.currentAttackElement != Elements.FIRE)
        {
            elementIdx = Elements.FIRE;
            ChangeElement(Elements.FIRE, attackManager.changeAttackDelay);
        }
        else if (Input.GetKeyDown(KeyCode.Alpha2) && attackManager.currentAttackElement != Elements.WATER)
        {
            elementIdx = Elements.WATER;
            ChangeElement(Elements.WATER, attackManager.changeAttackDelay);
        }
        else if (Input.GetKeyDown(KeyCode.Alpha3) && attackManager.currentAttackElement != Elements.GRASS)
        {
            elementIdx = Elements.GRASS;
            ChangeElement(Elements.GRASS, attackManager.changeAttackDelay);
        }
    }

    void MouseInputsManager()
    {
        if (!CanChangeElement || !tutorialDone) return;

        if ((mouseClockWise && Input.GetAxis("Mouse ScrollWheel") < 0f) || (!mouseClockWise && Input.GetAxis("Mouse ScrollWheel") > 0f))
        {
            StopParticles(elementIdx);
            elementIdx--;
            if (elementIdx < 0) elementIdx = Elements.COUNT - 2;
            ChangeElement(elementIdx, attackManager.changeAttackDelay);

        }
        else if ((mouseClockWise && Input.GetAxis("Mouse ScrollWheel") > 0f) || (!mouseClockWise && Input.GetAxis("Mouse ScrollWheel") < 0f))
        {
            StopParticles(elementIdx);
            elementIdx++;
            if (elementIdx >= Elements.COUNT - 1) elementIdx = 0;
            ChangeElement(elementIdx, attackManager.changeAttackDelay);
        }
    }


    void ChangeElement(Elements _element, float _changeAttackDelay)
    {
        attackManager.ResetCritQuantity();
        PlayParticles(_element);
        elementChanging = _element;
        StartCoroutine(ChangeElement_Cor(_element, _changeAttackDelay));

        //AUDIO
        ChangeElementSound(_element);
    }

    IEnumerator ChangeElement_Cor(Elements _element, float _changeAttackDelay)
    {
        changingElement = true;
        effectTypeParticles.Stop();
        effectTypeParticles_UI.Stop();
        attackManager.canAttack = false;
        attackManager.SetAttackTimer(attackManager.changeAttackDelay);
        nearSliderElementIcon.sprite = icons[(int)_element];
        StartCoroutine(LerpImageAlpha(nearSliderElementIcon, 0, 1, 0.3f));
        attackManager.currentAttackElement = _element;

        UIRadialRot.ResetRadialTime();
        float timer = 0, maxTime = _changeAttackDelay;
        while(timer < maxTime)
        {
            yield return new WaitForEndOfFrame();
            if (allowElementChangeOnceStarted && elementChanging != _element) 
            {
                StopParticles(_element);
                yield break;  
            }

            timer += Time.deltaTime;
            changeElementSlider.value = Mathf.Lerp(1, 0, timer / maxTime);
        }

        StartCoroutine(LerpImageAlpha(nearSliderElementIcon, 1, 0, 0.3f));
        changeElementSlider.value = 0;
        attackManager.canAttack = true;
        attackManager.SetAttackTimer(attackManager.attackDelay / 4f);

        SwitchElementParticles(_element);
        changingElement = false;
    }

    void SwitchElementParticles(Elements _element)
    {
        var main = effectTypeParticles.main;
        main.startColor = elementsData[_element].colorParticles;
        effectTypeParticles.Play();

        var main2 = effectTypeParticles_UI.main;
        main2.startColor = elementsData[_element].colorParticles;
        effectTypeParticles_UI.Play();
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
    //public static float GetInflictDamageMultiplier(Elements _entityElement, Elements _damageElement)
    //{
    //    return elementsData[_entityElement].inflictDamage[_damageElement];
    //}

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
            case Elements.NEUTRAL:
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
            case Elements.NEUTRAL:
                changeElementNeutral.Play();
                break;
            default: break;

        };

    }
    //AUDIO
    //changes the element and plays attack sound 
    public void ChangeElementSound(Elements _element)
    {
        if (_element == Elements.FIRE)
        {
            AudioManager.instance.SetFMODLabeledParameter("element", "fire", uiChangeElement);
        }
        else if (_element == Elements.GRASS)
        {
            AudioManager.instance.SetFMODLabeledParameter("element", "plant", uiChangeElement);
        }
        else if (_element == Elements.WATER)
        {
            AudioManager.instance.SetFMODLabeledParameter("element", "water", uiChangeElement);
        }

        uiChangeElement.start();
    }
}
