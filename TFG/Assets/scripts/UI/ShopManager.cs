using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System;

public class ShopManager : MonoBehaviour
{
    [SerializeField] Transform itemsHolder;
    [SerializeField] TextMeshProUGUI playerMoneyText;
    [SerializeField] internal ShopSkillExtraInfo extraInfoBox;
    [SerializeField] GameObject cardPrefabUI;

    //PassiveSkills_Manager skillsManager;
    LoadPassiveSkills passiveSkillsSave;
    List<PassiveSkill_Base> ownedSkills = new List<PassiveSkill_Base>();
    ShopItemData[] itemsInfo;
    //Transform cardListPivot;
    bool inited = false;
    float timer = 1;

    // Start is called before the first frame update
    void Update()
    {
        if(!inited)
        {
            timer -= Time.deltaTime;

            if(timer <= 0)
            {
                //skillsManager = FindObjectOfType<PassiveSkills_Manager>();
                //cardListPivot = GameObject.FindGameObjectWithTag("CardGrid").transform;
                passiveSkillsSave = GameObject.FindGameObjectWithTag("save").GetComponent<LoadPassiveSkills>();
                InitOwnedSkills();
                InitItemsInfo();

                GetComponent<CanvasGroup>().alpha = 1f;
                gameObject.SetActive(false);
                inited = true; 
            }
        }
    }

    private void Start()
    {
        InitItemsInfo();
    }


    private void OnEnable()
    {
        GetComponent<Canvas>().sortingOrder = 3;

        if (inited) RefreshAllItemsInfo();
    }

    private void OnDisable()
    {
        GetComponent<Canvas>().sortingOrder = 2;
    }

    void InitOwnedSkills()
    {
        //passiveSkillsSave.ResetSave(LoadPassiveSkills.ShopPath);
        ownedSkills = new List<PassiveSkill_Base>();
        SavedPassiveSkills save = passiveSkillsSave.LoadSave(LoadPassiveSkills.ShopPath);
        foreach (Tuple<PassiveSkill_Base.SkillType, int> element in save.savedElements)
        {
            PassiveSkill_Base skillToAdd = new PassiveSkill_Base();
            skillToAdd.skillType = element.Item1;
            skillToAdd.SetShopLevel(element.Item2);

            ownedSkills.Add(skillToAdd);
        }

    }

    public void ReestartInfo()
    {
        for(int i = 0; i < ownedSkills.Count; i++)
        {
            ownedSkills[i].SetShopLevel(0);
            RefreshItemInfo(i);
            itemsInfo[i].priceText.text = itemsInfo[i].data.Price.ToString();
            Button bttn = itemsInfo[i].GetComponent<Button>();
            if (!bttn.interactable)
            {
                itemsInfo[i].iconImage.color = Color.white;
                itemsInfo[i].GetComponent<Button>().interactable = true;
            }
            extraInfoBox.SetExtraInfo(ownedSkills[i]);
        }
        RefreshAllItemsInfo();
    }

    void InitItemsInfo()
    {
        itemsInfo = new ShopItemData[itemsHolder.childCount];
        for (int i = 0; i < itemsInfo.Length; i++)
        {
            itemsInfo[i] = itemsHolder.GetChild(i).GetComponent<ShopItemData>();
            itemsInfo[i].manager = this;
            RefreshItemInfo(i);
        }
    }

    void RefreshAllItemsInfo()
    {
        for (int i = 0; i < itemsInfo.Length; i++)
            RefreshItemInfo(i);
        playerMoneyText.text = MoneyManager.MoneyAmount.ToString();
        extraInfoBox.SetExtraInfo(itemsInfo[0].data);
    }

    void RefreshItemInfo(int _itemIdx, PassiveSkill_Base _playerItemData = null)
    {
        if (_playerItemData == null)
        {
            PassiveSkill_Base.SkillType tmpItemDataType = itemsInfo[_itemIdx].data.skillType;
            _playerItemData = ownedSkills.Find(_skill => _skill.skillType == tmpItemDataType);
        }
        if(_playerItemData != null)
        {
            itemsInfo[_itemIdx].data.SetShopLevel(_playerItemData.Level);
            if (!itemsInfo[_itemIdx].data.CanBeImproved)
            {
                itemsInfo[_itemIdx].iconImage.color = Color.gray;
                itemsInfo[_itemIdx].GetComponent<Button>().interactable = false;
                itemsInfo[_itemIdx].priceText.text = "Sold Out";
                return;
            }
        }

        itemsInfo[_itemIdx].nameText.text = itemsInfo[_itemIdx].data.Name;
        itemsInfo[_itemIdx].priceText.text = itemsInfo[_itemIdx].data.Price.ToString();
    }


    public void ItemChosen(int _itemIdx)
    {
        PassiveSkill_Base tmpItemData = itemsInfo[_itemIdx].data;
        if (tmpItemData.CanBeImproved && MoneyManager.MoneyAmount >= tmpItemData.Price)
        {
            MoneyManager.SetMoney(MoneyManager.MoneyAmount - tmpItemData.Price);
            MoneyManager.SaveCurrentMoney();

            PassiveSkill_Base playerSkill = ownedSkills.Find(_skill => _skill.skillType == tmpItemData.skillType);
            if (playerSkill == null)
            {
                tmpItemData.SetShopLevel(1);
                ownedSkills.Add(tmpItemData);
                RefreshItemInfo(_itemIdx);
            }
            else
            {
                playerSkill.SetShopLevel(playerSkill.Level + 1);
                RefreshItemInfo(_itemIdx, playerSkill);
            }

            extraInfoBox.SetExtraInfo(itemsInfo[_itemIdx].data);
            playerMoneyText.text = MoneyManager.MoneyAmount.ToString();
            if(tmpItemData != null)
                passiveSkillsSave.AddElementToSave_Shop(tmpItemData.skillType);
        }
        else
        {
            if (!itemsInfo[_itemIdx].busy)
                StartCoroutine(NotEnoughMoneyFeedback(itemsInfo[_itemIdx]));
        }
    }


    IEnumerator NotEnoughMoneyFeedback(ShopItemData _shopItem, float _lerpTime = 0.2f)
    {
        _shopItem.busy = true;
        Image shopItemImage = _shopItem.GetComponent<Image>();
        Color originColor = shopItemImage.color;
        float timer = 0f;
        while(timer < _lerpTime)
        {
            yield return null;
            timer += Time.deltaTime;
            shopItemImage.color = Color.Lerp(originColor, Color.red, timer / _lerpTime);
        }
        timer = 0f;
        while (timer < _lerpTime)
        {
            yield return null;
            timer += Time.deltaTime;
            shopItemImage.color = Color.Lerp(Color.red, originColor, timer / _lerpTime);
        }
        _shopItem.busy = false;
    }


}
