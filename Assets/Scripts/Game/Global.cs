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

    private float RestartCountdownInitial;
    [SerializeField]
    private float RestartCountdownTimer = 15;
    private float RestartCountdownTime;

    public bool isCountDown = false,
                isTimer = false,
                isGameBegin = false,
                isGameEnd = false,
                isRestartCountdown = false;

    [SyncVar]
    public bool isRestart = false;

    private NetworkStartPosition[] spawnPoints;
    private Player[] players;

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
            players[i].playerPrefab.SetActive(true);
            players[i].playerPrefab.GetComponent<Spaceship>().ReInitialize();
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
                isGameBegin = false;
                isGameEnd = true;
                initializeRestartTimer();
                GameOver();
            }
        }

        if(isGameEnd) {
            if(Time.time >= RestartCountdownTime) {
                isGameEnd = false;
                isRestart = true;
                Rematch();
                RestartSyncVars();
                Restart();

                //Restart Asteroid
                AsteroidSpawner asteroidSpawner = FindObjectOfType<AsteroidSpawner>();
                if(isRestart && asteroidSpawner != null) {
                    asteroidSpawner.RpcRestart();
                    Debug.Log("Spawner finished Restart!");
                }else {
                    Debug.Log("Spawner Not Found!");
                }
            }
        }
	}

    public void Restart() {

        isCountDown = false;
        isTimer = false;
        isGameBegin = false;
        isGameEnd = false;

        //find all spawn points
        spawnPoints = FindObjectsOfType<NetworkStartPosition>();
        //find all players
        players = FindObjectsOfType<Player>();

        //setpositions for every spaceship
        for(int i = 0; i < players.Length; i++) {
            players[i].playerPrefab.transform.position = spawnPoints[i].transform.position;
            players[i].playerPrefab.SetActive(true);
            players[i].playerPrefab.GetComponent<Spaceship>().ReInitialize();
        }

        //start timer for timer to start
        StartCountdownInitial = Time.time;
        StartCountdownTime = StartCountdownInitial + StartCountdownTimer;
        isCountDown = true;

        Debug.Log("Restart Set!");
    }

    public void RestartSyncVars() {
        player1 = 0;
        player2 = 0;
        player3 = 0;
        player4 = 0;

        Debug.Log("SyncVars Set!");
    }

    public void initializeTimer() {
        //start timer countdown
        isTimer = true;
        GameCountdownInitial = Time.time;
        float minutesInSeconds = Mathf.Floor(GameCountdownTimer_Minutes) * 60;
        float leftSeconds = (GameCountdownTimer_Minutes - Mathf.Floor(GameCountdownTimer_Minutes)) * 60;
        GameCountdownTimer = GameCountdownInitial + minutesInSeconds + leftSeconds; //Seconds
    }

    public void initializeRestartTimer() {
        RestartCountdownInitial = Time.time;
        RestartCountdownTime = RestartCountdownInitial + RestartCountdownTimer;
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

    public string getRestartTimerTime() {
        float remainingTime = RestartCountdownTime - Time.time;
        float seconds = Mathf.Ceil(remainingTime);

        if(seconds < 0) {
            seconds = 0;
        }

        return ("Restart in... " + seconds);
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
        /*
         * player object
         * blackhole
         * spaceship
         * asteroids
         * small asteroids
         * shards
         * bullets
         */

        GameObject[] gameObjects = FindObjectsOfType<GameObject>();
        foreach(GameObject gameObject in gameObjects) {
            Player    playerEntity    = gameObject.GetComponent<Player>();
            Blackhole blackholeEntity = gameObject.GetComponent<Blackhole>();
            Asteroid  asteroidEntity  = gameObject.GetComponent<Asteroid>();
            Shard     shardEntity     = gameObject.GetComponent<Shard>();
            Bullet    bulletEntity    = gameObject.GetComponent<Bullet>();

            if(playerEntity != null && playerEntity.isLocalPlayer) {
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

        players = FindObjectsOfType<Player>();

        //Gameover
        foreach(Player player in players) {
            player.Gameover();
        }
    }

    public void Rematch() {
        //remove all unnecessary entities.
        /*
         * player object
         * blackhole
         * spaceship
         * asteroids
         * small asteroids
         * shards
         * bullets
         */

        GameObject[] gameObjects = FindObjectsOfType<GameObject>();
        foreach(GameObject gameObject in gameObjects) {
            Player playerEntity = gameObject.GetComponent<Player>();
            Blackhole blackholeEntity = gameObject.GetComponent<Blackhole>();
            Asteroid asteroidEntity = gameObject.GetComponent<Asteroid>();
            Shard shardEntity = gameObject.GetComponent<Shard>();
            Bullet bulletEntity = gameObject.GetComponent<Bullet>();

            if(playerEntity != null && playerEntity.isLocalPlayer) {
                playerEntity.enabled = true;

            } else if(blackholeEntity != null) {
                blackholeEntity.enabled = true;

            } else if(asteroidEntity != null) {
                Destroy(asteroidEntity.gameObject);

            } else if(shardEntity != null) {
                Destroy(shardEntity.gameObject);

            } else if(bulletEntity != null) {
                Destroy(bulletEntity.gameObject);
            }
        }
    }
}
