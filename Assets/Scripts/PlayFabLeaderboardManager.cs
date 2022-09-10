using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using PlayFab;
using PlayFab.ClientModels;

public class PlayFabLeaderboardManager : MonoBehaviour
{
    string playerID;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    public void SendHighScore(int score)
    {
        PlayFabClientAPI.UpdatePlayerStatistics( new UpdatePlayerStatisticsRequest
        {
            Statistics = new List<StatisticUpdate>{
                new StatisticUpdate{
                    StatisticName = "Kills",
                    Value = score
                }
            }
        }, 
        success=>
        {
            print("Updated Leaderboard!");
        }, PlayfabManager.OnRequestFailure);
    }

    public static void GetLeaderboard()
    {
        PlayFabClientAPI.GetLeaderboard( new GetLeaderboardRequest
        {
            StatisticName = "Kills",
            StartPosition = 0,
            MaxResultsCount = 10
        }, result =>
        {            
            foreach (var item in result.Leaderboard)
            {
                print(item.Position + " " + item.DisplayName + " " + item.StatValue);
            }
        }, PlayfabManager.OnRequestFailure);
    }

    public void GetLocalLeaderboard()
    {
        PlayFabClientAPI.GetLeaderboardAroundPlayer(new GetLeaderboardAroundPlayerRequest
        {
            StatisticName = "Clicks",           
            MaxResultsCount = 10
        }, result =>
        {
            foreach (var item in result.Leaderboard)
            {
                print(item.Position + " " + item.DisplayName + " " + item.StatValue);
                if(item.PlayFabId == PlayfabLogin.playerID)
                {
                    //This is the Player's score!
                    //Change color or highlight
                }
            }
        }, PlayfabManager.OnRequestFailure);
    }

    public void DisplayLeaderBoard()
    {

    }
}
