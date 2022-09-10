using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using PlayFab;
using PlayFab.ClientModels;

public class Inventory : MonoBehaviour
{
    List<ItemInstance> items;
    Dictionary<string,int> currencies;

    public GameObject slotPrefab, InventoryWindow;

    static Inventory _instance;
    public static Inventory Instance
    {
        get
        {
            if(_instance == null) _instance = new Inventory();
            return _instance;
        }
    }
    
    // Start is called before the first frame update
    void Start()
    {
         PlayfabLogin.OnLoginSuccess += LoggedIn;    
    }

    void LoggedIn(LoginResult success)
    {
        GetInventory();
    }
    

    public void GetInventory()
    {
        PlayFabClientAPI.GetUserInventory(new GetUserInventoryRequest(), 
        result =>
        {
            items = result.Inventory;
            currencies = result.VirtualCurrency;            
        }, PlayfabManager.OnRequestFailure);
    }

    public void DisplayInventory()
    {
        foreach (var item in items)
        {
            var slot = Instantiate(slotPrefab,InventoryWindow.transform);
            slot.GetComponentInChildren<TMP_Text>().text = item.DisplayName;
        }
    }

    public static void GrantItem(ItemInstance item)
    {
        PlayFabServerAPI.GrantItemsToUser(new PlayFab.ServerModels.GrantItemsToUserRequest
        {
            ItemIds = new List<string>{item.ItemId},
            PlayFabId = PlayfabLogin.playerID
        }, result =>
        {
            print($"{item.DisplayName} Granted!");
        }, PlayfabManager.OnRequestFailure);
    }

    public static void ConsumeItem(ItemInstance item)
    {
        PlayFabClientAPI.ConsumeItem(new ConsumeItemRequest
        {
            ItemInstanceId = item.ItemInstanceId,
            ConsumeCount = 1
        }, result =>
        {
            print($"used 1 {item.DisplayName}");
            Instance.GetInventory();
        }, PlayfabManager.OnRequestFailure);
    }

    
}
