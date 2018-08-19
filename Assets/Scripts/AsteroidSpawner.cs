using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AsteroidSpawner : MonoBehaviour {

    public GameObject asteroidPrefab;

    public float maxDistance,
                 minDistance,
                 asteroidSpawnTime = 0.0f,
                 asteroidSpawnRate = 2.0f,
                 asteroidSpawnCount = 1f,
                 spawnCountRate = 0.2f,
                 difficultyTime = 1f,
                 difficultyRate = 3f;
                 

	// Use this for initialization
	void Start () {
        
	}
	
	// Update is called once per frame
	void Update () {
        if(Time.time > asteroidSpawnTime) {
            //Debug.Log(Mathf.FloorToInt(asteroidSpawnCount));
            for(int i=0; i< Mathf.FloorToInt(asteroidSpawnCount); i++) {
                Vector2 location = GetRandomLocation();
                GameObject.Instantiate(asteroidPrefab, location, Quaternion.identity);
                asteroidSpawnTime = Time.time + asteroidSpawnRate;
            }
        }

        asteroidSpawnCount = Time.time * spawnCountRate;
        /*if(Time.time > difficultyTime) {
            
            difficultyTime = Time.time * difficultyRate;
        }*/
	}

    private Vector2 GetRandomLocation() {
        float angle = Random.Range(-180, 180);
        float radius = Random.Range(15f,20f);
        float x = Mathf.Sin(Mathf.Deg2Rad * angle) * radius;
        float y = Mathf.Cos(Mathf.Deg2Rad * angle) * radius;
        return new Vector2(x,y);
    }
}
