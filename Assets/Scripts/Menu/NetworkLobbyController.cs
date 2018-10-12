using System.Collections;
using System.Collections.Generic;
using System.Net;
using System.Net.Sockets;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

[RequireComponent(typeof(NetworkLobbyManager))]
public class NetworkLobbyController : NetworkDiscovery {

    public NetworkLobbyManager manager;
    private Dictionary<string, string> lobbies_dictionary = new Dictionary<string, string>();

    void Awake() {
        manager = GetComponent<NetworkLobbyManager>();
    }

    public void HostLan(string roomName) {
        manager.networkAddress = GetIPAddress();
        manager.StartHost();
        Debug.Log(manager.lobbyScene);
        string scene = manager.lobbyScene + "";
        SceneManager.LoadScene(scene);
        broadcastData = roomName; 
        Initialize();
        StartAsServer();
    }

    public void JoinLan(string ip) {
        manager.networkAddress = ip;
        manager.StartClient();
        string scene = manager.lobbyScene + "";
        SceneManager.LoadScene(scene);
        StopBroadcast();
    }

    public void SearchLan() {
        Initialize();
        StartAsClient();
    }

    public override void OnReceivedBroadcast(string fromAddress, string data) {
        Debug.Log("Server Found: " + fromAddress + ", " + data);
        if(!lobbies_dictionary.ContainsKey(fromAddress)) {
            lobbies_dictionary.Add(fromAddress, data);
        }
    }

    public Dictionary<string,string> getLobbiesDictionary() {
        return lobbies_dictionary;
    }

    private string GetIPAddress() {
        string deviceHostName = Dns.GetHostName();
        var ipCount = Dns.GetHostAddresses(deviceHostName);
        for(int i = 0; i < ipCount.Length; i++) {
            IPAddress deviceIP = ipCount[i];
            if(deviceIP.AddressFamily == AddressFamily.InterNetwork) {
                Debug.Log(deviceIP.ToString());
                return deviceIP.ToString();
            }
        }
        return "localhost";
    }
}
