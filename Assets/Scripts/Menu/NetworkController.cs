using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.UI;

public class NetworkController : MonoBehaviour {

    private Button buttonPrefab;
    public GameObject networkLobbyManagerPrefab;


    private void Start() {
        GameObject[] networkObjects = GameObject.FindGameObjectsWithTag("Network");
        if(networkObjects.Length == 1) {
            return;
        } else if(networkObjects.Length == 0) {
            Debug.Log("Note: Network Object has been created.");
            Instantiate(networkLobbyManagerPrefab);
        } else {
            Debug.Log("Warning: There are multiple network Objects.");
        }
    }

    public void SetButtonPrefab(Button buttonPrefab) {
        this.buttonPrefab = buttonPrefab;
    }

    public void FindNetworkManagerAndHost(Text roomName) {
        NetworkLobbyController networkController = FindNetworkLobbyController();
        if(networkController == null) { return; }
        networkController.HostLan(roomName.text);
    }

    public void FindNetworkManagerAndJoin(Text textIP) {
        NetworkLobbyController networkController = FindNetworkLobbyController();
        if(networkController == null) { return; }
        networkController.JoinLan(textIP.text);
    }

    public void SearchLanNetwork() {
        NetworkLobbyController networkController = FindNetworkLobbyController();
        if(networkController == null) { return; }
        networkController.SearchLan();
    }

    public void StopSearchLanNetwork() {
        NetworkLobbyController networkController = FindNetworkLobbyController();
        if(networkController == null) { return; }
        networkController.StopBroadcast();
    }

    public void RefreshLanNetwork(RectTransform parentContainer) {
        NetworkLobbyController networkController = FindNetworkLobbyController();
        if(networkController == null) { return; }

        Dictionary<string, string> lobbiesDictionary = networkController.getLobbiesDictionary();

        GameObject[] buttons = GameObject.FindGameObjectsWithTag("Button");
        if(buttons != null) {
            foreach(GameObject button in buttons) {
                Destroy(button);
            }
        }

        foreach(KeyValuePair<string, string> lobby in lobbiesDictionary) {
            Button buttonObject = Instantiate(buttonPrefab);
            buttonObject.GetComponent<LobbyButton>().setLobbyButtonText(lobby.Key, lobby.Value);
            buttonObject.transform.SetParent(parentContainer, false);
        }
    }

    public void Ready(Text buttonReady) {
        GameObject[] players = GameObject.FindGameObjectsWithTag("Player");
        for(int i=0; i<players.Length; i++) {
            NetworkLobbyPlayer networkLobbyPlayer = players[i].GetComponent<NetworkLobbyPlayer>();
            if(networkLobbyPlayer.isLocalPlayer) {
                if(networkLobbyPlayer.readyToBegin) {
                    networkLobbyPlayer.SendNotReadyToBeginMessage();
                    buttonReady.text = "Ready";
                } else {
                    networkLobbyPlayer.SendReadyToBeginMessage();
                    buttonReady.text = "Cancel";
                }
                break;
            }
        }
    }

    public void AddRemovePlayer(Text buttonText) {
        GameObject[] networkObjects = GameObject.FindGameObjectsWithTag("Network");
        if(networkObjects.Length == 1) {
            NetworkLobbyManager networkLobbyManager =  networkObjects[0].GetComponent<NetworkLobbyManager>();
            bool isLocalPlayerFound = false;

            GameObject[] players = GameObject.FindGameObjectsWithTag("Player");
            for(int i = 0; i < players.Length; i++) {
                NetworkLobbyPlayer networkLobbyPlayer = players[i].GetComponent<NetworkLobbyPlayer>();
                if(networkLobbyPlayer.isLocalPlayer) {
                    isLocalPlayerFound = true;
                    if(networkLobbyPlayer.readyToBegin == false) {
                        ClientScene.RemovePlayer(networkLobbyPlayer.gameObject.GetComponent<NetworkIdentity>().playerControllerId);
                        buttonText.text = "Add Player";
                    }
                    break;
                }
            }

            if(!isLocalPlayerFound) {
                networkLobbyManager.TryToAddPlayer();
                buttonText.text = "Remove Player";
            }
        } else if(networkObjects.Length == 0) {
            Debug.Log("Warning: Network Object doesn't Exist.");
        } else {
            Debug.Log("Warning: There are multiple network Objects.");
        }
    }

    public void Leave(GameObject leaveDialogBoxPrefab) {
        Instantiate(leaveDialogBoxPrefab);
    }

    private NetworkLobbyController FindNetworkLobbyController() {
        GameObject[] networkObjects = GameObject.FindGameObjectsWithTag("Network");
        if(networkObjects.Length == 1) {
            return networkObjects[0].GetComponent<NetworkLobbyController>();
        } else if(networkObjects.Length == 0) {
            Debug.Log("Warning: Network Object doesn't Exist.");
        } else {
            Debug.Log("Warning: There are multiple network Objects.");
        }
        return null;
    }
}
