using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using PlayFab;
using PlayFab.ClientModels;
using System;

public class CharacterManager : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        PlayfabLogin.OnLoginSuccess += OnLoggedin;        
    }

    private void OnLoggedin(LoginResult success)
    {
        PlayFabClientAPI.GrantCharacterToUser(new GrantCharacterToUserRequest
        {
            CharacterName = "Fighter",
            ItemId = "Fighter",
            CatalogVersion = "Characters"
        }, result=>
        {

        }, PlayfabManager.OnRequestFailure);
    }

    public void GrantCharacter(ItemInstance item)
    {
        PlayFabClientAPI.GrantCharacterToUser(new GrantCharacterToUserRequest
        {
            CharacterName = item.DisplayName,
            ItemId = item.ItemId,
            CatalogVersion = item.CatalogVersion
        }, result=>
        {

        }, PlayfabManager.OnRequestFailure);
    }



    // Update is called once per frame
    void Update()
    {
        
    }
}
