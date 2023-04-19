using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class ShopManager : MonoBehaviour
{
    [SerializeField] Transform itemsHolder;
    [SerializeField] TextMeshProUGUI playerMoneyText;
    [SerializeField] internal ShopSkillExtraInfo extraInfoBox;
    [SerializeField] GameObject cardPrefabUI;

    PassiveSkills_Manager skillsManager;
    ShopItemData[] itemsInfo;
    Transform cardListPivot;


    // Start is called before the first frame update
    void Awake()
    {
        skillsManager = FindObjectOfType<PassiveSkills_Manager>();
        cardListPivot = GameObject.FindGameObjectWithTag("CardGrid").transform;
        InitItemsInfo();

        GetComponent<CanvasGroup>().alpha = 1f;
        gameObject.SetActive(false);
    }

    private void OnEnable()
    {
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
            _playerItemData = skillsManager.FindSkill(itemsInfo[_itemIdx].data);
        if (_playerItemData != null)
        {
            itemsInfo[_itemIdx].data.SetShopLevel(_playerItemData.Level);
            if (!_playerItemData.CanBeImproved)
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

            PassiveSkill_Base playerSkill = skillsManager.FindSkill(itemsInfo[_itemIdx].data);
            if (playerSkill == null)
            {
                skillsManager.AddSkill(tmpItemData, true);
                RefreshItemInfo(_itemIdx);
            }
            else
            {
                playerSkill.AddLevel(1);
                RefreshItemInfo(_itemIdx, playerSkill);
            }

            extraInfoBox.SetExtraInfo(itemsInfo[_itemIdx].data);
            playerMoneyText.text = MoneyManager.MoneyAmount.ToString();
            SpawnCardInUI(itemsInfo[_itemIdx].data.skillType, itemsInfo[_itemIdx].iconImage);
        }
    }


    CardUIScript CheckIfThisCardExistsAlready(PassiveSkill_Base.SkillType _skillType)
    {
        foreach (Transform child in cardListPivot)
        {
            if (child.GetComponent<CardUIScript>().skillType.Equals(_skillType))
                return child.GetComponent<CardUIScript>();
        }

        return null;
    }

    void SpawnCardInUI(PassiveSkill_Base.SkillType _skillType, Image _iconImage)
    {
        CardUIScript cardCheck = CheckIfThisCardExistsAlready(_skillType);

        if (cardCheck != null)
        {
            cardCheck.ModifyCardTier();
        }
        else
        {
            CardUIScript card = Instantiate(cardPrefabUI, cardListPivot).GetComponent<CardUIScript>();

            card.skillType = _skillType;
            card.cardSprite = _iconImage.sprite;
        }
    }

}
