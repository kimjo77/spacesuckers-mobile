using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class LobbyButton : MonoBehaviour {

    public Text textIP, textRoomName;

    private void Start() {
        GetComponent<Button>().onClick.AddListener(delegate { GetNetworkController().FindNetworkManagerAndJoin(textIP); });
    }

    public void setLobbyButtonText(string ip, string roomName) {
        textIP.text = ip;
        textRoomName.text = roomName;
    }

    public NetworkController GetNetworkController() {
        GameObject[] networkObjects = GameObject.FindGameObjectsWithTag("NetworkController");
        if(networkObjects.Length == 1) {
            NetworkController networkController = networkObjects[0].GetComponent<NetworkController>();
            return networkController;
        } else if(networkObjects.Length == 0) {
            Debug.Log("Warning: Network Object doesn't Exist.");
        } else {
            Debug.Log("Warning: There are multiple network Objects.");
        }
        return null;
    }
}
