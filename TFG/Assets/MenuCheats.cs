using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class MenuCheats : MonoBehaviour
{
    const int MONEY_WHEN_INFINITE = 150000;

    [SerializeField] TextMeshProUGUI moneyText;

    bool infiniteMoney = false;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.F1)) infiniteMoney = !infiniteMoney;

        if (infiniteMoney)
        {
            MoneyManager.SetMoney(MONEY_WHEN_INFINITE);
            if (moneyText.gameObject.activeInHierarchy) moneyText.text = MONEY_WHEN_INFINITE.ToString();
        }
    }
}
