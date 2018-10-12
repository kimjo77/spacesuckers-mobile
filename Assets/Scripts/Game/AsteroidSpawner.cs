using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

public class AsteroidSpawner : NetworkBehaviour {

    public Global global;
    public GameObject asteroidPrefab;
    public int initialSpawnCount = 1;
    public float maxDistance = 22f,
                 minDistance = 18f,
                 asteroidInitialTime,
                 asteroidSpawnTime = 0.0f,
                 asteroidSpawnRate = 2.0f,
                 asteroidSpawnCount = 1f,
                 spawnCountRate = 0.2f,
                 difficultyTime = 1f,
                 difficultyRate = 3f;
    private bool isInitial = false;

    private void Start() {
        isInitial = false;
        asteroidSpawnTime = Time.time + 0;
        asteroidInitialTime = asteroidSpawnTime;
    }

    // Update is called once per frame
    void Update () {
        if(global.isRestart) {
            RpcRestart();
            global.isRestart = false;
        }

        if(global.isGameEnd) {
            return;
        }

        if(!global.isGameBegin) {
            return;
        } else if(!isInitial) {
            isInitial = true;
            asteroidSpawnTime = Time.time + 0;
            asteroidInitialTime = asteroidSpawnTime;
        }

        if(Time.time >= asteroidSpawnTime) {
            //Debug.Log(Mathf.FloorToInt(asteroidSpawnCount));
            for(int i=0; i< Mathf.FloorToInt(asteroidSpawnCount); i++) {
                Vector2 location = GetRandomLocation();
                GameObject asteroid = (GameObject) Instantiate(asteroidPrefab, location, Quaternion.identity);
                NetworkServer.Spawn(asteroid);
                asteroidSpawnTime = Time.time + asteroidSpawnRate;
            }
        }
        asteroidSpawnCount = ((Time.time - asteroidInitialTime) * spawnCountRate);
        asteroidSpawnCount += initialSpawnCount;
    }

    private Vector2 GetRandomLocation() {
        float angle = Random.Range(-180, 180);
        float radius = Random.Range(minDistance, maxDistance);
        float x = Mathf.Sin(Mathf.Deg2Rad * angle) * radius;
        float y = Mathf.Cos(Mathf.Deg2Rad * angle) * radius;
        return new Vector2(x,y);
    }

    [ClientRpc]
    public void RpcRestart() {
        isInitial = false;
        asteroidSpawnTime = Time.time + 0;
        asteroidInitialTime = asteroidSpawnTime;
    }
}
