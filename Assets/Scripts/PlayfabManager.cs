using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using PlayFab;
using PlayFab.ClientModels;


public class PlayfabManager : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
       
    }
    
    

    static public void OnRequestFailure(PlayFabError error)
    {
        print(error.GenerateErrorReport());
    }

    

    static public void DisplayTitleData(Dictionary<string,string> result)
    {
        
            if(!result.ContainsKey("MOTD"))
            {
                print("No Data Found!");
                return;
            } 

            print(result["MOTD"]);
            
        
    }
}
