using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

public class Global : MonoBehaviour {

    private float StartCountdownInitial, 
                  StartCountdownTimer = 3,
                  StartCountdownTime,
        
                  GameCountdownInitial,
                  GameCountdownTimer_Minutes = 5,
                  GameCountdownTimer;

    public bool isCountDown = false,
                isTimer = false,
                isGameBegin = false,
                isGameEnd = false;

    private NetworkStartPosition[] spawnPoints;
    private Player[] players;
    private int[] playerPoints;

    private void Start() {
        //find all spawn points
        spawnPoints = FindObjectsOfType<NetworkStartPosition>();
        //find all players
        players = FindObjectsOfType<Player>();

        //setpositions for every spaceship
        for(int i=0; i<players.Length; i++) {
            players[i].playerPrefab.transform.position = spawnPoints[i].transform.position;
        }

        //initialize score for every player
        playerPoints = new int[players.Length];
        for(int i=0; i<playerPoints.Length; i++) {
            playerPoints[i] = 0;
        }

        //start timer for timer to start
        StartCountdownInitial = Time.time;
        StartCountdownTime = StartCountdownInitial + StartCountdownTimer;
        isCountDown = true;
    }

	// Update is called once per frame
	void Update () {
        if(isCountDown) {
            if(Time.time >= StartCountdownTime) {
                isCountDown = false;
                isGameBegin = true;
                initializeTimer();
            }
        }

        if(isTimer) {
            if(Time.time >= GameCountdownTimer) {
                isTimer = false;
                isGameEnd = true;
            }
        }

	}

    public void initializeTimer() {
        //start timer countdown
        isTimer = true;
        GameCountdownInitial = Time.time;
        float minutesInSeconds = Mathf.Floor(GameCountdownTimer_Minutes) * 60;
        float leftSeconds = (GameCountdownTimer_Minutes - Mathf.Floor(GameCountdownTimer_Minutes)) * 60;
        GameCountdownTimer = GameCountdownInitial + minutesInSeconds + leftSeconds; //Seconds
    }

    public string getStartCountdownTimerTime() {
        if(isTimer || !isCountDown) {
            return "";
        }
        float remainingTime = StartCountdownTime - Time.time;
        float seconds = remainingTime % 60;
        return (Mathf.Ceil(seconds)+"");
    }

    public string getCoundownTimerTime() {
        if(isCountDown) {
            float defaultMinutes = GameCountdownTimer_Minutes;
            float defaultSeconds = (GameCountdownTimer_Minutes - Mathf.Floor(GameCountdownTimer_Minutes)) * 60;
            return (defaultMinutes + ":" + defaultSeconds);
        } else if(!isTimer) {
            return "0:0";
        }
        float remainingTime = GameCountdownTimer - Time.time;
        float minutes = Mathf.Floor(remainingTime / 60);
        float seconds = remainingTime % 60;
        return (minutes + ":" + seconds.ToString("0.00"));
    }

    public int GetScore(Player player) {
        int index;
        for(int i=0; i<players.Length; i++) {
            if(players[i].netId == player.netId) {
                return playerPoints[i];
            }
        }
        return -1;
    }

    public void AddPoints(Player player, int value) {
        int index;
        for(int i = 0; i < players.Length; i++) {
            if(players[i].netId == player.netId) {
                playerPoints[i] += value;
                break;
            }
        }
    }
}
