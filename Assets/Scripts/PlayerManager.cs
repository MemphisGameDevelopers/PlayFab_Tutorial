using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using PlayFab;
using PlayFab.ClientModels;

public class PlayerManager : MonoBehaviour
{
    public string Name;
    public string Class;
    public string Level;

    // Start is called before the first frame update
    void Start()
    {
        PlayfabLogin.OnLoginSuccess += GetPlayerData;
    }

    public void SavePlayerData()
    {
        PlayFabClientAPI.UpdateUserData(new UpdateUserDataRequest
        {
            Data = new Dictionary<string, string>
            {
                {"Name", Name},
                {"Class", Class},
                {"Level", Level}
            }
        }, result =>
        {
            print("Character Saved!");
        }, PlayfabManager.OnRequestFailure);
    }

    public void GetPlayerData(LoginResult success)
    {
        PlayFabClientAPI.GetUserData(new GetUserDataRequest(), 
        result =>
        {
            print("Player Data Recieved!");
            if(result.Data != null)
            {
                Name = result.Data["Name"].Value;
                Class = result.Data["Class"].Value;
                Level = result.Data["Level"].Value;
            }
            else
            {
                print("Player Data does not exist!");
            }

        }, PlayfabManager.OnRequestFailure);
    }
}
