using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

public class Shard : NetworkBehaviour {

    [SyncVar]
    public int shardValue = 50;

    public float destroyTime = 10;

    public float smoothSpeed = 0.005f,
                 drag = 0.1f;
    [SyncVar]
    public Vector3 directionVector;


    public float minSpeed = 0,
                 maxSpeed = 0;

    private float speed;
    private Vector3 offset;

    [SyncVar]
    public int ownerID = -1;

    private GameObject targetGameObject = null;

    // Use this for initialization
    void Start () {
        offset = new Vector3(0, 0, transform.position.z);
        speed = Random.Range(minSpeed, maxSpeed);
        DestroyDuration();
    }
	
	// Update is called once per frame
	void Update () {

        if((speed -= drag) <= 0) {
            speed = 0;
        }

        GetComponent<Rigidbody2D>().velocity = directionVector * speed * Time.deltaTime;

        if(ownerID == -1) {
            return;
        }

        //Follow
        Vector3 desiredVector, smoothVector;
        //desiredVector = owner.transform.position + offset;
        if(targetGameObject == null) {
            Player[] players = FindObjectsOfType<Player>();
            for(int i=0; i<players.Length; i++) {
                NetworkLobbyPlayer lobbyPlayerEntity = players[i].GetComponent<NetworkLobbyPlayer>();
                if(lobbyPlayerEntity.slot == ownerID) {
                    targetGameObject = players[i].playerPrefab;
                    break;
                }
            }
        }

        desiredVector = targetGameObject.transform.position + offset;
        
        smoothVector = Vector3.Lerp(transform.position, desiredVector, smoothSpeed);
        transform.position = smoothVector;
    }


    public void DestroyDuration() {
        StartCoroutine(DestroyDurationRoutine());
    }
    public IEnumerator DestroyDurationRoutine() {
        yield return new WaitForSeconds(destroyTime);
        Destroy(this.gameObject);
    }

    private void OnTriggerEnter2D(Collider2D other) {
        if(other.gameObject.tag == "Entity") {
            Spaceship playerEntity = other.gameObject.GetComponent<Spaceship>();
            if(playerEntity != null) {
                NetworkLobbyPlayer lobbyPlayerEntity = playerEntity.player.GetComponent<NetworkLobbyPlayer>();
                if((lobbyPlayerEntity.slot == ownerID) || ownerID == -1) {
                    Debug.Log("ShardTaken");
                    playerEntity.AddPoints(shardValue);
                    Destroy(this.gameObject);
                }
            }
        }
    }
}
