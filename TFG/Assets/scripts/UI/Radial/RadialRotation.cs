using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RadialRotation : MonoBehaviour
{
    [SerializeField] internal ElementsManager.Elements UIRadialCurrentItem;
    [SerializeField] private Transform insideUIRadial;
    Dictionary<ElementsManager.Elements, Quaternion> rotationsByElements;
    private RectTransform rect;
    private PlayerAttack playerAttack;

    float current_time = 0;
    float lerpTime = 1.5f;


    private void Start()
    {
        rect = GetComponent<RectTransform>();
        playerAttack = GameObject.FindGameObjectWithTag("Player").GetComponent<PlayerAttack>();
        SetupRotations();
    }

    void SetupRotations()
    {
        rotationsByElements = new Dictionary<ElementsManager.Elements, Quaternion>();
        rotationsByElements[ElementsManager.Elements.FIRE] = Quaternion.Euler(0, 0, 90);
        rotationsByElements[ElementsManager.Elements.WATER] = Quaternion.Euler(0, 0, -30);
        rotationsByElements[ElementsManager.Elements.GRASS] = Quaternion.Euler(0, 0, -150);
        //rotationsByElements[ElementsManager.Elements.NORMAL] = Quaternion.Euler(0, 0, 90);
    }

    void Update()
    {
        lerpTime = playerAttack.changeAttackDelay;
        UIRadialCurrentItem = playerAttack.currentAttackElement;
        SetRadialRotation();
    }

    void SetRadialRotation()
    {
        if (current_time <= lerpTime) current_time += Time.deltaTime;

        rect.localRotation = Quaternion.RotateTowards(rect.localRotation, rotationsByElements[UIRadialCurrentItem], current_time / lerpTime);
        insideUIRadial.localRotation = Quaternion.RotateTowards(insideUIRadial.localRotation, rotationsByElements[UIRadialCurrentItem], current_time / lerpTime);
    }

    public void ResetRadialTime()
    {
        current_time = 0;
    }
}
