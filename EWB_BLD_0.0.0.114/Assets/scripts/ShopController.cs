using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ShopController : MonoBehaviour {
    
    Canvas shopCanvas;
    public GameObject powerSword;
    public GameObject powerSwordButton;

    public GameObject ultPowerSword;
    public GameObject ultPowerSwordButton;

    public GameObject powerArmor;
    public GameObject powerArmorButton;

    public GameObject ultPowerArmor;
    public GameObject ultPowerArmorButton;


    public void CloseShopMenu()
    {
        GameManager.instance.ToggleLockInput(false);

        gameObject.SetActive(false);
    }
    
    void Start ()
    {
        shopCanvas = GetComponent<Canvas>();
	}

    /*
     * MANAGE SHOP UI POPUP AND ITEM AVAILABILITY
     */
    private void OnEnable()
    {
        GameManager.instance.ToggleLockInput(true);
        GameManager.instance.SetPlayerToIdle();

        Image image;
        if (GameManager.instance.eventsManager.CheckEvent(GameConstants.GOT_POWER_SWORD))
        {
            image = powerSword.GetComponent<Image>();
            image.color = new Color32(107, 107, 107, 255);

            powerSwordButton.SetActive(false);
        }
        if (GameManager.instance.eventsManager.CheckEvent(GameConstants.GOT_POWER_ARMOR))
        {
            image = powerArmor.GetComponent<Image>();
            image.color = new Color32(107, 107, 107, 255);

            powerArmorButton.SetActive(false);
        }
        if (GameManager.instance.eventsManager.CheckEvent(GameConstants.GOT_ULT_POWER_SWORD))
        {
            image = ultPowerSword.GetComponent<Image>();
            image.color = new Color32(107, 107, 107, 255);

            ultPowerSwordButton.SetActive(false);
        }
        if (GameManager.instance.eventsManager.CheckEvent(GameConstants.GOT_ULT_POWER_ARMOR))
        {

            image = ultPowerArmor.GetComponent<Image>();
            image.color = new Color32(107, 107, 107, 255);

            ultPowerArmorButton.SetActive(false);
        }
    }

    public void BuyItem(string eventItem)
    {
        bool canAfford = false;

        if (eventItem == GameConstants.GOT_POWER_SWORD)
        {
            if(GameManager.instance.GetMoney() >= 10000)
            {
                GameManager.instance.coins -= 10000;
                canAfford = true;
            }
        }
        else if(eventItem == GameConstants.GOT_ULT_POWER_SWORD)
        {
            if (GameManager.instance.GetMoney() >= 20000)
            {
                GameManager.instance.coins -= 20000;
                canAfford = true;
            }
        }
        else if(eventItem == GameConstants.GOT_POWER_ARMOR)
        {
            if (GameManager.instance.GetMoney() >= 10000)
            {
                GameManager.instance.coins -= 10000;
                canAfford = true;
            }
        }
        else if (eventItem == GameConstants.GOT_ULT_POWER_ARMOR)
        {
            if (GameManager.instance.GetMoney() >= 20000)
            {
                GameManager.instance.coins -= 20000;
                canAfford = true;
            }
        }
        
        if (canAfford)
        {
            GameManager.instance.eventsManager.TriggerEvent(eventItem, true);
        }
        //    else
        //    {
        //     GameManager.instance.dialogController.MoveDialogBubble(GameManager.instance.shopKeeper.GetComponent<Character>(), GameManager.instance.dialogController.GetDialog("sorry"));
        //    }
        
        GameManager.instance.ToggleLockInput(false);
        gameObject.SetActive(false);
    }
}
