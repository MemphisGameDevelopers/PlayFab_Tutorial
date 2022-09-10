using UnityEngine;
using UnityEngine.SceneManagement;
//using Mirror;
using MLAPI;
using MLAPI.Messaging;
using UnityEngine.Events;
using System;
using System.IO;
using PlayFab;

/*
	Documentation: https://mirror-networking.com/docs/Components/NetworkManager.html
	API Reference: https://mirror-networking.com/docs/api/Mirror.NetworkManager.html
*/

public class NewNetworkManager : MonoBehaviour
{

    //public NewNetworkManager Instance { get; private set; }

    public ConnectedEvent OnConnected = new ConnectedEvent();
    public DisconnectedEvent OnDisconnected = new DisconnectedEvent();

    public class ConnectedEvent : UnityEvent { }
    public class DisconnectedEvent : UnityEvent<int?> { }

    public void Awake()
    {
        //base.Awake();
        //Instance = this;
        NetworkManager.Singleton.OnClientConnectedCallback += OnClientConnect;
        NetworkManager.Singleton.OnClientDisconnectCallback += OnClientDisconnect;
    }

    public void Start()
    {
        //base.Start();
        NetworkManager.Singleton.StartClient();
        
    }

    public void Send(string msgType, ulong clientID, string message)
    {            
        CustomMessagingManager.SendNamedMessage(msgType, clientID, new MemoryStream(System.Text.Encoding.UTF8.GetBytes(message)));                       
    }


    public void OnClientConnect(ulong conn)
    {
        //base.OnClientConnect(conn);
        Debug.Log("connected");
        OnConnected.Invoke();
    }


    public void OnClientDisconnect(ulong conn)
    {
        //base.OnClientDisconnect(conn);
        Debug.Log("disconnected");
        OnDisconnected.Invoke(null);
    }

    // /// <summary>
    // /// Called on clients when a network error occurs.
    // /// </summary>
    // /// <param name="conn">Connection to a server.</param>
    // /// <param name="errorCode">Error code.</param>
    // public override void OnClientError(NetworkConnection conn, int errorCode) { }

    // /// <summary>
    // /// Called on clients when a servers tells the client it is no longer ready.
    // /// <para>This is commonly used when switching scenes.</para>
    // /// </summary>
    // /// <param name="conn">Connection to the server.</param>
    // public override void OnClientNotReady(NetworkConnection conn) { }

    // /// <summary>
    // /// Called from ClientChangeScene immediately before SceneManager.LoadSceneAsync is executed
    // /// <para>This allows client to do work / cleanup / prep before the scene changes.</para>
    // /// </summary>
    // /// <param name="newSceneName">Name of the scene that's about to be loaded</param>
    // /// <param name="sceneOperation">Scene operation that's about to happen</param>
    // /// <param name="customHandling">true to indicate that scene loading will be handled through overrides</param>
    // public override void OnClientChangeScene(string newSceneName, SceneOperation sceneOperation, bool customHandling) { }

    // /// <summary>
    // /// Called on clients when a scene has completed loaded, when the scene load was initiated by the server.
    // /// <para>Scene changes can cause player objects to be destroyed. The default implementation of OnClientSceneChanged in the NetworkManager is to add a player object for the connection if no player object exists.</para>
    // /// </summary>
    // /// <param name="conn">The network connection that the scene change message arrived on.</param>
    // public override void OnClientSceneChanged(NetworkConnection conn)
    // {
    //     base.OnClientSceneChanged(conn);
    // }



    // /// <summary>
    // /// This is invoked when the client is started.
    // /// </summary>
    // public override void OnStartClient()
    // {
    //     base.OnStartClient();
    // }


    // /// <summary>
    // /// This is called when a client is stopped.
    // /// </summary>
    // public override void OnStopClient() { }

}


public class CustomGameServerMessageTypes
{
    public const short ReceiveAuthenticate = 900;
    public const short ShutdownMessage = 901;
    public const short MaintenanceMessage = 902;
}

public struct ReceiveAuthenticateMessage //: NetworkMessage
{
    public string PlayFabId;
}

public struct ShutdownMessage {}//: NetworkMessage { }

[Serializable]
public struct MaintenanceMessage //: NetworkMessage
{
    public DateTime ScheduledMaintenanceUTC;
}

public static class MaintenanceMessageFunctions
{
    // public static MaintenanceMessage Deserialize(this NetworkReader reader)
    // {
    //     MaintenanceMessage msg = new MaintenanceMessage();
            
    //     var json = PlayFab.PluginManager.GetPlugin<ISerializerPlugin>(PluginContract.PlayFab_Serializer);
    //     msg.ScheduledMaintenanceUTC = json.DeserializeObject<DateTime>(reader.ReadString());

    //     return msg;
    // }

    // public static void Serialize(this NetworkWriter writer, MaintenanceMessage value)
    // {
    //     var json = PlayFab.PluginManager.GetPlugin<ISerializerPlugin>(PluginContract.PlayFab_Serializer);
    //     var str = json.SerializeObject(value.ScheduledMaintenanceUTC);
    //     writer.Write(str);
    // }
}
