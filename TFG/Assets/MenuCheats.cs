using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class MenuCheats : MonoBehaviour
{
    const int MONEY_WHEN_INFINITE = 999999;

    [SerializeField] TextMeshProUGUI moneyText;

    bool infiniteMoney = false;
    int originalMoneyAmount = 0;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.F1))
        {
            infiniteMoney = !infiniteMoney;
            if (infiniteMoney)
            {
                originalMoneyAmount = MoneyManager.MoneyAmount;
            }
            else
            {
                MoneyManager.SetMoney(originalMoneyAmount);
                if (moneyText.gameObject.activeInHierarchy) moneyText.text = originalMoneyAmount.ToString();
            }
        }

        if (infiniteMoney)
        {
            MoneyManager.SetMoney(MONEY_WHEN_INFINITE);
            if (moneyText.gameObject.activeInHierarchy) moneyText.text = MONEY_WHEN_INFINITE.ToString();
        }


        if (Input.GetKeyDown(KeyCode.F5))
        {
            GameObject.FindGameObjectWithTag("save").GetComponent<LoadPassiveSkills>().ResetBoughtSkills();
            FindObjectOfType<ShopManager>().ReestartInfo();
        }

    }


}
