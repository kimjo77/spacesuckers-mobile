using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.UI;

public class PanelScoreboard : MonoBehaviour {

    public Text textFirst, 
                textSecond, 
                textThird, 
                textFourth;

    public Color colorBlue,
                 colorRed,
                 colorGreen,
                 colorOrange;

    private Global global = null;
	
	// Update is called once per frame
	void Update () {
        if(global != null) {

            //gameScore.text = global.GetScore(player) + "";
            //Get all players
            Player[] players = FindObjectsOfType<Player>();

            if(players.Length == 1) {
                textFirst.enabled = true;
                textSecond.enabled = false;
                textThird.enabled = false;
                textFourth.enabled = false;
            } else if(players.Length == 2) {
                textFirst.enabled = true;
                textSecond.enabled = true;
                textThird.enabled = false;
                textFourth.enabled = false;
            } else if(players.Length == 3) {
                textFirst.enabled = true;
                textSecond.enabled = true;
                textThird.enabled = true;
                textFourth.enabled = false;
            } else if(players.Length == 4) {
                textFirst.enabled = true;
                textSecond.enabled = true;
                textThird.enabled = true;
                textFourth.enabled = true;
            }

            //Get all playerpoints;
            int[] points = new int[players.Length];

            for(int i=0; i<points.Length; i++) {
                points[i] = global.GetScore(players[i]);
            }

            //Start sorting [Insertion Sort]
            for(int i=0; i<points.Length; i++) {
                int largestIndex = i;
                for(int j = i; j < points.Length; j++) {
                    if(points[largestIndex] < points[j]) {
                        largestIndex = j;
                    }
                }
                //swap point
                int temp = points[largestIndex];
                points[largestIndex] = points[i];
                points[i] = temp;

                //swap player
                Player tempPlayer = players[largestIndex];
                players[largestIndex] = players[i];
                players[i] = tempPlayer;
            }

            if(players.Length == 1) {
                //First
                textFirst.color = GetPlayerColor(players[0]);
                textFirst.text = points[0] + "";
            } else if(players.Length == 2) {
                //First
                textFirst.color = GetPlayerColor(players[0]);
                textFirst.text = points[0] + "";
                //Second
                textSecond.color = GetPlayerColor(players[1]);
                textSecond.text = points[1] + "";
            } else if(players.Length == 3) {
                //First
                textFirst.color = GetPlayerColor(players[0]);
                textFirst.text = points[0] + "";
                //Second
                textSecond.color = GetPlayerColor(players[1]);
                textSecond.text = points[1] + "";
                //Third
                textThird.color = GetPlayerColor(players[2]);
                textThird.text = points[2] + "";
            } else if(players.Length == 4) {
                //First
                textFirst.color = GetPlayerColor(players[0]);
                textFirst.text = points[0] + "";
                //Second
                textSecond.color = GetPlayerColor(players[1]);
                textSecond.text = points[1] + "";
                //Third
                textThird.color = GetPlayerColor(players[2]);
                textThird.text = points[2] + "";
                //Fourth
                textFourth.color = GetPlayerColor(players[3]);
                textFourth.text = points[3] + "";
            }
        } else {
            findGlobalGameObject();
            textFirst.enabled  = false;
            textSecond.enabled = false;
            textThird.enabled  = false;
            textFourth.enabled = false;
        }
    }

    public void findGlobalGameObject() {
        GameObject globalObject = GameObject.FindGameObjectWithTag("Global");
        if(globalObject != null) {
            global = globalObject.GetComponent<Global>();
        }
    }

    private Color GetPlayerColor(Player player) {
        NetworkLobbyPlayer networkLobbyPlayer = player.GetComponent<NetworkLobbyPlayer>();
        switch(networkLobbyPlayer.slot) {
            case 0: return colorBlue;
            case 1: return colorRed;
            case 2: return colorOrange;
            case 3: return colorGreen;
        }
        return colorBlue;
    }
}
