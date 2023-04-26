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


    // Start is called before the first frame update
    void Awake()
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

    private void OnEnable()
    {
        if (inited) RefreshAllItemsInfo();
    }

    void InitOwnedSkills()
    {
        //passiveSkillsSave.ResetSave(LoadPassiveSkills.ShopPath);
        SavedPassiveSkills save = passiveSkillsSave.LoadSave(LoadPassiveSkills.ShopPath);
        foreach (Tuple<PassiveSkill_Base.SkillType, int> element in save.savedElements)
        {
            PassiveSkill_Base skillToAdd = new PassiveSkill_Base();
            skillToAdd.skillType = element.Item1;
            skillToAdd.SetShopLevel(element.Item2);

            ownedSkills.Add(skillToAdd);
        }

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
        ///ToDo: Revisar quan sistema de guardat estigui fet
        //if (_playerItemData == null)
        //    _playerItemData = skillsManager.FindSkill(itemsInfo[_itemIdx].data);
        //if (_playerItemData != null)
        //{
        //    itemsInfo[_itemIdx].data.SetShopLevel(_playerItemData.Level);
        //    if (!_playerItemData.CanBeImproved)
        //    {
        //        itemsInfo[_itemIdx].iconImage.color = Color.gray;
        //        itemsInfo[_itemIdx].GetComponent<Button>().interactable = false;
        //        itemsInfo[_itemIdx].priceText.text = "Sold Out";
        //        return;
        //    }
        //}

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

            //PassiveSkill_Base playerSkill = skillsManager.FindSkill(tmpItemData.data);
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
            passiveSkillsSave.AddElementToSave_Shop(tmpItemData.skillType);
            //SpawnCardInUI(itemsInfo[_itemIdx].data.skillType, itemsInfo[_itemIdx].iconImage);
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


    //CardUIScript CheckIfThisCardExistsAlready(PassiveSkill_Base.SkillType _skillType)
    //{
    //    foreach (Transform child in cardListPivot)
    //    {
    //        if (child.GetComponent<CardUIScript>().skillType.Equals(_skillType))
    //            return child.GetComponent<CardUIScript>();
    //    }

    //    return null;
    //}

    //void SpawnCardInUI(PassiveSkill_Base.SkillType _skillType, Image _iconImage)
    //{
    //    CardUIScript cardCheck = CheckIfThisCardExistsAlready(_skillType);

    //    if (cardCheck != null)
    //    {
    //        cardCheck.ModifyCardTier();
    //    }
    //    else
    //    {
    //        CardUIScript card = Instantiate(cardPrefabUI, cardListPivot).GetComponent<CardUIScript>();

    //        card.skillType = _skillType;
    //        card.cardSprite = _iconImage.sprite;
    //    }
    //}

}
