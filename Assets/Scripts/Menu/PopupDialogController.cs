using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.Networking;

[Serializable]
public class PopupDialogController : MonoBehaviour {

    public void Cancel(GameObject dialogBox) {
        Destroy(dialogBox);
    }

    public void Leave(string screenName) {
        GameObject[] networkObjects = GameObject.FindGameObjectsWithTag("Network");
        if(networkObjects.Length == 1) {
            NetworkLobbyManager networkLobbyManager = networkObjects[0].GetComponent<NetworkLobbyManager>();
            NetworkLobbyController networkLobbyController = networkObjects[0].GetComponent<NetworkLobbyController>();

            networkLobbyController.StopBroadcast();

            GameObject[] players = GameObject.FindGameObjectsWithTag("Player");
            for(int i = 0; i < players.Length; i++) {
                NetworkLobbyPlayer networkLobbyPlayer = players[i].GetComponent<NetworkLobbyPlayer>();
                if(networkLobbyPlayer.isLocalPlayer) {
                    if(networkLobbyPlayer.isServer) {
                        Time.timeScale = 1;
                        networkLobbyManager.StopServer();
                        networkLobbyManager.StopClient();
                        Destroy(networkLobbyManager.gameObject);
                    } else if(!networkLobbyPlayer.isServer) {
                        networkLobbyManager.StopClient();
                        Time.timeScale = 1;
                        Destroy(networkLobbyManager.gameObject);
                    } else {
                        Debug.Log("Warning: NetworkLobbyPlayer is Neither Server nor Client");
                    }
                    break;
                }
            }

        } else if(networkObjects.Length == 0) {
            Debug.Log("Warning: Network Object doesn't Exist.");
        } else {
            Debug.Log("Warning: There are multiple network Objects.");
        }

        GameObject[] gameObjects = GameObject.FindGameObjectsWithTag("ScreenController");
        Debug.Log("Count: " + gameObjects.Length );
        for(int i=0; i<gameObjects.Length; i++) {
            ScreenController screenController = gameObjects[i].GetComponent<ScreenController>();
            if(screenController != null) {
                screenController.TransitionToScene(screenName);
                break;
            }
        }
    }
}
