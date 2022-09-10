using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using PlayFab;
using PlayFab.ClientModels;
using TMPro;
using System;

public class PlayfabLogin : MonoBehaviour
{
    public GameObject SignInWindow, SetUserNameWindow;
    public TMP_InputField Email, Username, Password, NameChange;  
    public static string customID = default, playerID, Displayname;
    public static string SessionTicket, EntityID;
    GetPlayerCombinedInfoRequestParams playerinfo;

    public delegate void LoginSuccessEvent(LoginResult success);
    public static event LoginSuccessEvent OnLoginSuccess;
    
    void Start()
    {
        playerinfo = new GetPlayerCombinedInfoRequestParams
        {
            GetPlayerProfile = true,
            GetTitleData = true
        };

        AnonymousLogin();
    }

    void AnonymousLogin()
    {
        PlayFabClientAPI.LoginWithCustomID(new LoginWithCustomIDRequest
        {
            CustomId = SystemInfo.deviceUniqueIdentifier,
            CreateAccount = true,
            InfoRequestParameters = playerinfo
        },LoginSuccess, OnRequestFailure);
    }
    public void CreateAccount()
    {
        if(Password.text.Length < 6)
        {
            print("Password is too short!");
            return;
        }
        PlayFabClientAPI.RegisterPlayFabUser(new RegisterPlayFabUserRequest
        {
            Username = Username.text,
            Email = Email.text,
            Password = Password.text,
            InfoRequestParameters = playerinfo
        }, result =>
        {
            SessionTicket = result.SessionTicket;
            SignInWindow.SetActive(false);
        }, OnRequestFailure);
    }

    public void SignInWithPlayFab()
    {
        PlayFabClientAPI.LoginWithPlayFab(new LoginWithPlayFabRequest
        {
            Username = Username.text,
            Password = Password.text,
            InfoRequestParameters = playerinfo
        },LoginSuccess, OnRequestFailure);
    }

    public void SignInWithEmail()
    {
        PlayFabClientAPI.LoginWithEmailAddress(new LoginWithEmailAddressRequest
        {
            Email = Email.text,
            Password = Password.text,
            InfoRequestParameters = playerinfo
            
        }, LoginSuccess, OnRequestFailure);
    }

    public void ResetPassword()
    {
        PlayFabClientAPI.SendAccountRecoveryEmail(new SendAccountRecoveryEmailRequest
        {
            Email = Email.text,
            TitleId = "B154C"
            
        },result =>
        {
            print("Password Reset Email Sent!");
        },OnRequestFailure);
    }

    private void LoginSuccess(LoginResult result)
    {
        playerID = result.PlayFabId;
        SessionTicket = result.SessionTicket;
        EntityID = result.EntityToken.Entity.Id;
        //SignInWindow.SetActive(false);
        if(result.InfoResultPayload.TitleData != null)
        {
            PlayfabManager.DisplayTitleData(result.InfoResultPayload.TitleData);
        }
        if(result.InfoResultPayload.PlayerProfile != null)
        {
            if(result.InfoResultPayload.PlayerProfile.DisplayName == null)
            {
                SetUserNameWindow.SetActive(true);
            }
            else
            {
                Displayname = result.InfoResultPayload.PlayerProfile.DisplayName;
            }
        }
        PlayFabLeaderboardManager.GetLeaderboard();

        OnLoginSuccess?.Invoke(result);
    }

    public void ChangeName()
    {
        PlayFabClientAPI.UpdateUserTitleDisplayName(new UpdateUserTitleDisplayNameRequest
        {
            DisplayName = NameChange.text
        }, result =>
        {
            print("Name changed");
            SetUserNameWindow.SetActive(false);
        }, OnRequestFailure);
    }

    private void OnRequestFailure(PlayFabError error)
    {
        print(error.GenerateErrorReport());
    }
}
