using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

public class Shard : NetworkBehaviour {

    public int shardValue = 25;
    public GameObject owner = null;
    public float smoothSpeed = 0.005f,
                 drag = 0.1f;
    [SyncVar]
    public Vector3 directionVector;


    public float minSpeed = 0,
                 maxSpeed = 0;

    private float speed;
    private Vector3 offset;

    // Use this for initialization
    void Start () {
        offset = new Vector3(0, 0, transform.position.z);
        speed = Random.Range(minSpeed, maxSpeed);
    }
	
	// Update is called once per frame
	void Update () {
        if((speed -= drag) <= 0) {
            speed = 0;
        }

        GetComponent<Rigidbody2D>().velocity = directionVector * speed * Time.deltaTime;

        //Follow
        Vector3 desiredVector, smoothVector;
        desiredVector = owner.transform.position + offset;
        smoothVector = Vector3.Lerp(transform.position, desiredVector, smoothSpeed);
        transform.position = smoothVector;
    }

    private void OnTriggerEnter2D(Collider2D other) {
        if(other.gameObject.tag == "Entity") {
            Spaceship playerEntity = other.gameObject.GetComponent<Spaceship>();
            if(playerEntity != null) {
                if(other.gameObject == owner) {
                    Debug.Log("ShardTaken");
                    playerEntity.AddPoints(shardValue);
                    Destroy(this.gameObject);
                }
            }
        }
    }
}
