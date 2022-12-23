using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ElementsManager : MonoBehaviour
{
    public enum Elements { NORMAL, FIRE, GRASS, WATER }
    public class ElementClass
    {
        public Dictionary<Elements, float> receiveDamage = new Dictionary<Elements, float>();
        public Dictionary<Elements, float> inflictDamage = new Dictionary<Elements, float>();
    }

    static Dictionary<Elements, ElementClass> elementsData = new Dictionary<Elements, ElementClass>();


    // Start is called before the first frame update
    void Awake()
    {
        InitElementsData();
    }

    void InitElementsData()
    {
        ElementClass fireCompatibility = new ElementClass();
        fireCompatibility.receiveDamage.Add(Elements.FIRE, 1.0f);
        fireCompatibility.receiveDamage.Add(Elements.GRASS, 0.5f);
        fireCompatibility.receiveDamage.Add(Elements.WATER, 1.5f);
        fireCompatibility.inflictDamage.Add(Elements.FIRE, 1.0f);
        fireCompatibility.inflictDamage.Add(Elements.GRASS, 1.5f);
        fireCompatibility.inflictDamage.Add(Elements.WATER, 0.5f);
        elementsData.Add(Elements.FIRE, fireCompatibility);

        ElementClass grassCompatibility = new ElementClass();
        grassCompatibility.receiveDamage.Add(Elements.FIRE, 1.5f);
        grassCompatibility.receiveDamage.Add(Elements.GRASS, 1.0f);
        grassCompatibility.receiveDamage.Add(Elements.WATER, 0.5f);
        grassCompatibility.inflictDamage.Add(Elements.FIRE, 0.5f);
        grassCompatibility.inflictDamage.Add(Elements.GRASS, 1.0f);
        grassCompatibility.inflictDamage.Add(Elements.WATER, 1.5f);
        elementsData.Add(Elements.GRASS, grassCompatibility);

        ElementClass waterCompatibility = new ElementClass();
        waterCompatibility.receiveDamage.Add(Elements.FIRE, 0.5f);
        waterCompatibility.receiveDamage.Add(Elements.GRASS, 1.5f);
        waterCompatibility.receiveDamage.Add(Elements.WATER, 1.0f);
        waterCompatibility.inflictDamage.Add(Elements.FIRE, 1.5f);
        waterCompatibility.inflictDamage.Add(Elements.GRASS, 0.5f);
        waterCompatibility.inflictDamage.Add(Elements.WATER, 1.0f);
        elementsData.Add(Elements.WATER, waterCompatibility);
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
