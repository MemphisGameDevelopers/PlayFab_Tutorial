namespace PlayFab.Networking
{
    using System;
    using System.Collections.Generic;
    using UnityEngine;   
    using UnityEngine.Events;
    //using UnityEngine.Networking;
    //using UnityEngine.Networking.NetworkSystem;
    using MLAPI;
    using MLAPI.Connection;
    using MLAPI.Messaging;
    using System.IO;
    using MLAPI.Serialization.Pooled;

    public class UnityNetworkServer : MonoBehaviour
    {
        public PlayerEvent OnPlayerAdded = new PlayerEvent();
        public PlayerEvent OnPlayerRemoved = new PlayerEvent();

        public int MaxConnections = 100;
        public int Port = 7777;

        private NetworkManager _netManager;

        public List<UnityNetworkConnection> Connections
        {
            get { return _connections; }
            private set { _connections = value; }
        }
        private List<UnityNetworkConnection> _connections = new List<UnityNetworkConnection>();
        private readonly object Encoding;

        public class PlayerEvent : UnityEvent<string> { }

        // Use this for initialization
        void Awake()
        {
            //_netManager = FindObjectOfType<NetworkManager>();
            _netManager = NetworkManager.Singleton;
            //NetworkServer.RegisterHandler(MsgType.Connect, OnServerConnect);            
            _netManager.OnClientConnectedCallback += OnServerConnect;
            //NetworkServer.RegisterHandler(MsgType.Disconnect, OnServerDisconnect);
            CustomMessagingManager.RegisterNamedMessageHandler("Disconnect", OnServerDisconnect);
            //NetworkServer.RegisterHandler(MsgType.Error, OnServerError);
            CustomMessagingManager.RegisterNamedMessageHandler("Error", OnServerError);            
            //NetworkServer.RegisterHandler(CustomGameServerMessageTypes.ReceiveAuthenticate, OnReceiveAuthenticate);
            CustomMessagingManager.RegisterNamedMessageHandler("Auth", OnReceiveAuthenticate);
            //_netManager.networkPort = Port;
            
        }

        public void StartServer()
        {
            //NetworkServer.Listen(Port);
            _netManager.StartServer();
        }

        private void OnServerConnect(ulong sender)
        {
            Debug.LogWarning("Client Connected");
            //var conn = _connections.Find(c => c.ConnectionId == netMsg.conn.connectionId);
            var conn = _connections.Find(c => c.ConnectionId == sender);
            if (conn == null)
            {

                _connections.Add(new UnityNetworkConnection()
                {
                    ConnectionId = sender,
                    LobbyId = PlayFabMultiplayerAgentAPI.SessionConfig.SessionId
                });
                
            }
        }



        private void OnReceiveAuthenticate(ulong sender, Stream payload)
        {
            
            //var conn = _connections.Find(c => c.ConnectionId == netMsg.conn.connectionId);
            var conn = _connections.Find(c => c.ConnectionId == sender);
            if (conn != null)
            {
                //var message = netMsg.ReadMessage<ReceiveAuthenticateMessage>();
                using (var reader = PooledNetworkReader.Get(payload))
                {
                    string message = reader.ReadString().ToString();
                
                    conn.PlayFabId = message;
                    conn.IsAuthenticated = true;
                    OnPlayerAdded.Invoke(message);
                }
            }
        }
        private void OnServerError(ulong sender, Stream payload)
        {
            try
            {
                //var error = netMsg.ReadMessage<ErrorMessage>();
                using (var reader = PooledNetworkReader.Get(payload))
                {       
                    // var error = (ErrorMessage)reader.ReadObjectPacked(typeof(ErrorMessage));         
                    // if (error != null)
                    // {
                    //     Debug.Log(string.Format("Unity Network Connection Status: code - {0}", error.errorCode));
                    // }
                }
            }
            catch (Exception)
            {
                Debug.Log("Unity Network Connection Status, but we could not get the reason, check the Unity Logs for more info.");
            }
        }

        public void Send(string msgType, ulong clientID, string message)
        {            
           CustomMessagingManager.SendNamedMessage(msgType, clientID, new MemoryStream(System.Text.Encoding.UTF8.GetBytes(message)));                       
        }

        private void OnServerDisconnect(ulong sender, Stream payload)
        {
            var conn = _connections.Find(c => c.ConnectionId == sender);
            if (conn != null)
            {
                if (!string.IsNullOrEmpty(conn.PlayFabId))
                {
                    OnPlayerRemoved.Invoke(conn.PlayFabId);
                }
                _connections.Remove(conn);
            }
        }

        private void OnApplicationQuit()
        {
            //NetworkServer.Shutdown();
            _netManager.StopServer();
        }

    }

    [Serializable]
    public class UnityNetworkConnection
    {
        public bool IsAuthenticated;
        public string PlayFabId;
        public string LobbyId;
        //public int ConnectionId;
        public ulong ConnectionId;
        
    }

    public class CustomGameServerMessageTypes
    {
        public const short ReceiveAuthenticate = 900;
        public const short ShutdownMessage = 901;
        public const short MaintenanceMessage = 902;
    }

    public class ReceiveAuthenticateMessage 
    {
        public string PlayFabId;
    }

    public class ShutdownMessage  { }

    [Serializable]
    public class MaintenanceMessage  
    {
        public DateTime ScheduledMaintenanceUTC;

        public void Deserialize(PooledNetworkReader reader)
        {
            var json = new MultiplayerAgent.Helpers.SimpleJsonInstance();
            ScheduledMaintenanceUTC = json.DeserializeObject<DateTime>(reader.ReadString().ToString());
        }

        public void Serialize(PooledNetworkWriter writer)
        {
            var json = new MultiplayerAgent.Helpers.SimpleJsonInstance();
            var str = json.SerializeObject(ScheduledMaintenanceUTC);
            writer.WriteString(str);
        }
    }
}