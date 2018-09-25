using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

public class Global : NetworkBehaviour {

    private float StartCountdownInitial;
    [SerializeField]
    private float StartCountdownTimer = 3;
    private float StartCountdownTime;

    private float GameCountdownInitial;
    [SerializeField]
    private float GameCountdownTimer_Minutes = 5;
    private float GameCountdownTimer;

    public bool isCountDown = false,
                isTimer = false,
                isGameBegin = false,
                isGameEnd = false;

    private NetworkStartPosition[] spawnPoints;
    private Player[] players;
    //private int[] playerPoints;

    [SyncVar]
    private int player1 = 0, 
                player2 = 0, 
                player3 = 0, 
                player4 = 0;

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
        /*playerPoints = new int[players.Length];
        for(int i=0; i<playerPoints.Length; i++) {
            playerPoints[i] = 0;
        }*/

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
                GameOver();
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
        for(int i=0; i<players.Length; i++) {
            if(players[i].netId == player.netId) {
                switch(i) {
                    case 0:
                        return player1;
                    case 1:
                        return player2;
                    case 2:
                        return player3;
                    case 3:
                        return player4;
                }
            }
        }
        return -1;
    }

    public void AddPoints(Player player, int value) {
        for(int i = 0; i < players.Length; i++) {
            if(players[i].netId == player.netId) {
                //playerPoints[i] += value;
                //break;
                switch(i) {
                    case 0:
                        player1 += value;
                        break;
                    case 1:
                        player2 += value;
                        break;
                    case 2:
                        player3 += value; ;
                        break;
                    case 3:
                        player4 += value;
                        break;
                }
                break;
            }
        }
    }

    public void DeductPoints(Player player, int value) {
        for(int i = 0; i < players.Length; i++) {
            if(players[i].netId == player.netId) {
                //playerPoints[i] += value;
                //break;
                switch(i) {
                    case 0:
                        if((player1 -= value) < 0) {
                            player1 = 0;
                        }
                        break;
                    case 1:
                        if((player2 -= value) < 0) {
                            player2 = 0;
                        }
                        break;
                    case 2:
                        if((player3 -= value) < 0) {
                            player3 = 0;
                        }
                        break;
                    case 3:
                        if((player4 -= value) < 0) {
                            player4 = 0;
                        }
                        break;
                }
                break;
            }
        }
    }

    public void GameOver() {
        //pause all entities.
        //pause all entities.
        /*
         * player object
         * blackhole
         * spaceship
         * asteroids
         * small asteroids
         * shards
         * bullets
         */

        Time.timeScale = 0;

        GameObject[] gameObjects = FindObjectsOfType<GameObject>();
        foreach(GameObject gameObject in gameObjects) {
            Player    playerEntity    = gameObject.GetComponent<Player>();
            Blackhole blackholeEntity = gameObject.GetComponent<Blackhole>();
            Asteroid  asteroidEntity  = gameObject.GetComponent<Asteroid>();
            Shard     shardEntity     = gameObject.GetComponent<Shard>();
            Bullet    bulletEntity    = gameObject.GetComponent<Bullet>();

            if(playerEntity != null) {
                playerEntity.enabled = false;

            } else if(blackholeEntity != null) {
                blackholeEntity.enabled = false;

            } else if(asteroidEntity != null) {
                asteroidEntity.enabled = false;

            } else if(shardEntity != null) {
                shardEntity.enabled = false;

            } else if(bulletEntity != null) {
                bulletEntity.enabled = false;
            }
        }
    }
}
