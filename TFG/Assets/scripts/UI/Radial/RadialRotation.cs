using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RadialRotation : MonoBehaviour
{
    [SerializeField] internal ElementsManager.Elements UIRadialCurrentItem;
    Dictionary<ElementsManager.Elements, Quaternion> rotationsByElements;
    private RectTransform rect;
    private PlayerAttack playerAttack;

    private const float LERP_SPEED = 1.5f;

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
        rotationsByElements[ElementsManager.Elements.WATER] = Quaternion.Euler(0, 0, -27);
        rotationsByElements[ElementsManager.Elements.GRASS] = Quaternion.Euler(0, 0, 200);
        //rotationsByElements[ElementsManager.Elements.NORMAL] = Quaternion.Euler(0, 0, 90);
    }

    void Update()
    {
        UIRadialCurrentItem = playerAttack.currentAttackElement;
        SetRadialRotation();
    }

    void SetRadialRotation()
    {
        rect.localRotation = Quaternion.Lerp(rect.localRotation, rotationsByElements[UIRadialCurrentItem], Time.deltaTime * LERP_SPEED);
    }
}
