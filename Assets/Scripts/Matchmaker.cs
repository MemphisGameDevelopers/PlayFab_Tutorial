using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using PlayFab;
using PlayFab.MultiplayerModels;
using PlayFab.Json;

public class Matchmaker : MonoBehaviour
{
    const string QueueName = "default";
    string ticketID;
    Coroutine pollticket;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    public void StartMatchmaking()
    {
        PlayFabMultiplayerAPI.CreateMatchmakingTicket(new CreateMatchmakingTicketRequest
        {
            Creator = new MatchmakingPlayer
            {
                Entity = new EntityKey
                {
                    Id = PlayfabLogin.EntityID,
                    Type = "title_player_account"
                },
                Attributes = new MatchmakingPlayerAttributes
                {
                    DataObject = new 
                    {
                        DisplayName = PlayfabLogin.DisplayName
                    }
                }
            },
            GiveUpAfterSeconds = 120,
            QueueName = QueueName            

        }, result =>
        {
            ticketID = result.TicketId; 
            pollticket = StartCoroutine(PollTicket());

        }, PlayfabManager.OnRequestFailure);
    }

    IEnumerator PollTicket()
    {
        while (true)
        {
            PlayFabMultiplayerAPI.GetMatchmakingTicket( new GetMatchmakingTicketRequest
            {
                TicketId = ticketID,
                QueueName = QueueName
            }, result =>
            {
                //TODO: Update Status text

              switch (result.Status)
              {
                case "Matched":
                    StopCoroutine(pollticket);
                    StartMatch(result.MatchId);
                    break;
                case "Canceled":
                    break;  
              }

            }, PlayfabManager.OnRequestFailure);


            //Can only poll 10 times per sec
            yield return new WaitForSeconds(6); 
        }        
    }

    void StartMatch(string matchID)
    {
        PlayFabMultiplayerAPI.GetMatch( new GetMatchRequest
        {
            MatchId = matchID,
            QueueName = QueueName,
            ReturnMemberAttributes = true
            
        }, result =>
        {
            //TODO: create/join MLAPI room named {matchID}
            print("Joined Lobby with: \n");
            foreach (var player in result.Members)
            {
                //print(player.Entity.Id + "\n");
                print(player.Attributes.DataObject.Displayname);
            }
        }, PlayfabManager.OnRequestFailure);
    }

    
}
