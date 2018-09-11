using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class NetworkController : MonoBehaviour {

    private Button buttonPrefab;

    public void SetButtonPrefab(Button buttonPrefab) {
        this.buttonPrefab = buttonPrefab;
    }

    public void FindNetworkManagerAndHost(Text roomName) {
        GameObject[] networkObjects = GameObject.FindGameObjectsWithTag("Network");
        if(networkObjects.Length == 1) {
            NetworkLobbyController networkController = networkObjects[0].GetComponent<NetworkLobbyController>();
            networkController.HostLan(roomName.text);
        } else if(networkObjects.Length == 0) {
            Debug.Log("Warning: Network Object doesn't Exist.");
        } else {
            Debug.Log("Warning: There are multiple network Objects.");
        }
    }

    public void FindNetworkManagerAndJoin(Text textIP) {
        GameObject[] networkObjects = GameObject.FindGameObjectsWithTag("Network");
        if(networkObjects.Length == 1) {
            NetworkLobbyController networkController = networkObjects[0].GetComponent<NetworkLobbyController>();
            networkController.JoinLan(textIP.text);
        } else if(networkObjects.Length == 0) {
            Debug.Log("Warning: Network Object doesn't Exist.");
        } else {
            Debug.Log("Warning: There are multiple network Objects.");
        }
    }

    public void SearchLanNetwork() {
        GameObject[] networkObjects = GameObject.FindGameObjectsWithTag("Network");
        if(networkObjects.Length == 1) {
            NetworkLobbyController networkController = networkObjects[0].GetComponent<NetworkLobbyController>();
            networkController.SearchLan();
        } else if(networkObjects.Length == 0) {
            Debug.Log("Warning: Network Object doesn't Exist.");
        } else {
            Debug.Log("Warning: There are multiple network Objects.");
        }
    }

    public void StopSearchLanNetwork() {
        GameObject[] networkObjects = GameObject.FindGameObjectsWithTag("Network");
        if(networkObjects.Length == 1) {
            NetworkLobbyController networkController = networkObjects[0].GetComponent<NetworkLobbyController>();
            networkController.StopBroadcast();
        } else if(networkObjects.Length == 0) {
            Debug.Log("Warning: Network Object doesn't Exist.");
        } else {
            Debug.Log("Warning: There are multiple network Objects.");
        }
    }

    public void RefreshLanNetwork(RectTransform parentContainer) {
        GameObject[] networkObjects = GameObject.FindGameObjectsWithTag("Network");
        if(networkObjects.Length == 1) {
            NetworkLobbyController networkController = networkObjects[0].GetComponent<NetworkLobbyController>();
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
        } else if(networkObjects.Length == 0) {
            Debug.Log("Warning: Network Object doesn't Exist.");
        } else {
            Debug.Log("Warning: There are multiple network Objects.");
        }
    }
}
