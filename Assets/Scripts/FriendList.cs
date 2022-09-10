using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using PlayFab;
using PlayFab.ClientModels;

public class FriendList : MonoBehaviour
{
    enum FriendIdType { PlayFabId, Username, Email, DisplayName };

    List<FriendInfo> _friends = null;

    // Start is called before the first frame update
    void Start()
    {
        
    }


void AddFriend(FriendIdType idType, string friendId) 
{
    var request = new AddFriendRequest();
    switch (idType) {
        case FriendIdType.PlayFabId:
            request.FriendPlayFabId = friendId;
            break;
        case FriendIdType.Username:
            request.FriendUsername = friendId;
            break;
        case FriendIdType.Email:
            request.FriendEmail = friendId;
            break;
        case FriendIdType.DisplayName:
            request.FriendTitleDisplayName = friendId;
            break;
    }
    // Execute request and update friends when we are done
    PlayFabClientAPI.AddFriend(request, result => {
        Debug.Log("Friend added successfully!");
    }, PlayfabManager.OnRequestFailure);
}        

public void GetFriends() 
{
    PlayFabClientAPI.GetFriendsList(new GetFriendsListRequest 
    {
        IncludeSteamFriends = false,
        IncludeFacebookFriends = false,
        XboxToken = null
    }, result => 
    {
        _friends = result.Friends;
        DisplayFriends(_friends); // triggers your UI
    }, PlayfabManager.OnRequestFailure);
}

    void DisplayFriends(List<FriendInfo> friendsCache) 
    { 
        friendsCache.ForEach(f => Debug.Log(f.FriendPlayFabId)); 
    }

// unlike AddFriend, RemoveFriend only takes a PlayFab ID
// you can get this from the FriendInfo object under FriendPlayFabId
void RemoveFriend(FriendInfo friendInfo) 
{
    PlayFabClientAPI.RemoveFriend(new RemoveFriendRequest 
    {
        FriendPlayFabId = friendInfo.FriendPlayFabId
    }, result => 
    {
        _friends.Remove(friendInfo);
    }, PlayfabManager.OnRequestFailure);
}
}
