using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MoneyManager : MonoBehaviour
{
    const int MAX_MONEY_AMOUNT = 999999;
    const float SAVE_MONEY_TIMER = 2f;
    const string MONEY_KEY = "MoneyAmount";

    static int moneyAmount = 0;
    static float nextSaveMoneyTimeStamp = -1f;
    static MoneyInGameUiReference uiManager;
    
    public static int MoneyAmount { get { return moneyAmount; } }

    private void Start()
    {
        moneyAmount = PlayerPrefs.GetInt(MONEY_KEY, 0);
        //moneyAmount = 0;
        uiManager = GetComponent<MoneyInGameUiReference>();
        if (uiManager != null)
        {
            uiManager.moneyText.text = moneyAmount.ToString();
            uiManager.DeactivateMoneyFeedback(1f, 2f);
            
        }
    }

    private void Update()
    {
        if (uiManager != null && nextSaveMoneyTimeStamp > 0f && nextSaveMoneyTimeStamp < Time.time)
        {
            nextSaveMoneyTimeStamp = -1f;
            uiManager.StopAllCoroutines();
            uiManager.DeactivateMoneyFeedback();
            SaveCurrentMoney();
        }
    }


    public static void AddMoney(int _moneyToAdd)
    {
        moneyAmount += _moneyToAdd;
        if (moneyAmount > MAX_MONEY_AMOUNT) moneyAmount = MAX_MONEY_AMOUNT;
        nextSaveMoneyTimeStamp = Time.time + SAVE_MONEY_TIMER;
        //Debug.Log("CurrentMoney: " + moneyAmount);

        if (uiManager != null)
        {
            if (!uiManager.feedbackActive)
            {
                uiManager.ActivateMoneyFeedback();
            }
            uiManager.moneyText.text = moneyAmount.ToString();
            //Animacion dineros
            if (uiManager.moneyUpAnim.GetCurrentAnimatorStateInfo(0).normalizedTime > 1f)
                uiManager.moneyUpAnim.Play("Base Layer.AnimationCoinsUp",0,0.25f);
        }
    }

    public static void SetMoney(int _moneyAmount)
    {
        moneyAmount = _moneyAmount;
        if (moneyAmount > MAX_MONEY_AMOUNT) moneyAmount = MAX_MONEY_AMOUNT;
    }

    public static void SaveCurrentMoney()
    {
        PlayerPrefs.SetInt(MONEY_KEY, moneyAmount);
    }


    [ContextMenu("SaveCurrentMoney")]
    void SaveMoneyAmount()
    {
        PlayerPrefs.SetInt(MONEY_KEY, moneyAmount);
    }

    [ContextMenu("ResetMoneyAmount")]
    void ResetMoneyAmount()
    {
        PlayerPrefs.SetInt(MONEY_KEY, 0);
    }

}
