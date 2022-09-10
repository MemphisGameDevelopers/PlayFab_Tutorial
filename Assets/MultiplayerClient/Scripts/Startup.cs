using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using PlayFab;
using PlayFab.ClientModels;
using PlayFab.Helpers;
//using Mirror;
using MLAPI;
using MLAPI.Messaging;


public class Startup : MonoBehaviour
{
    PlayFabAuthService _authService;
    public NewNetworkManager _nm;
    MessageWindow _messageWindow;

    // Use this for initialization
    void Start()
    {
        
        _authService = PlayFabAuthService.Instance;
        PlayFabAuthService.OnDisplayAuthentication += OnDisplayAuth;
        PlayFabAuthService.OnLoginSuccess += OnLoginSuccess;
        _nm.OnDisconnected.AddListener(OnDisconnected);
        _nm.OnConnected.AddListener(OnConnected);
        CustomMessagingManager.RegisterNamedMessageHandler("Shutdown",OnServerShutdown);
        CustomMessagingManager.RegisterNamedMessageHandler("Maintenance",OnMaintenanceMessage);

        _messageWindow = MessageWindow.Instance;
    }

    private void OnMaintenanceMessage(ulong sender, Stream payload)
    {
        //var message = msg;
        // _messageWindow.Title.text = "Maintenance Shutdown scheduled";
        // _messageWindow.Message.text = string.Format("Maintenance is scheduled for: {0}", message.ScheduledMaintenanceUTC.ToString("MM-DD-YYYY hh:mm:ss"));
        // _messageWindow.gameObject.SetActive(true);
    }

    private void OnServerShutdown(ulong sender, Stream payload)
    {
        _messageWindow.Title.text = "Shutdown In Progress";
        _messageWindow.Message.text = "Server has issued a shutdown.";
        _messageWindow.gameObject.SetActive(true);
        //NetworkClient.Disconnect();
        NetworkManager.Singleton.DisconnectClient(sender);
    }

    private void OnConnected()
    {
        _authService.Authenticate();
    }

    private void OnDisplayAuth()
    {
        _authService.Authenticate(Authtypes.Silent);
    }

    private void OnLoginSuccess(LoginResult success)
    {
        _messageWindow.Title.text = "Login Successful";
        _messageWindow.Message.text = string.Format("You logged in successfully. ID:{0}", success.PlayFabId);
        _messageWindow.gameObject.SetActive(true);
       
        _nm.Send("Auth",NetworkManager.Singleton.ServerClientId, success.PlayFabId);
    }

    private void OnDisconnected(int? code)
    {
        _messageWindow.Title.text = "Disconnected!";
        _messageWindow.Message.text = "You were disconnected from the server";
        _messageWindow.gameObject.SetActive(true);
    }


}