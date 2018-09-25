using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

public class Blackhole : NetworkBehaviour {

    public float gravity = 1;
    public float radius = 200;
    public int damage = 5000;

    // Use this for initialization
    void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
        #region REAL PHYSICS
        GameObject[] entities = GameObject.FindGameObjectsWithTag("Entity");
        if(entities != null) {
            foreach(GameObject entity in entities) {
                Vector3 directionVector = (entity.transform.position - transform.position);
                float dist = Vector2.Distance(transform.position, entity.transform.position);
                float dx, dy, angle;
                dx = transform.position.x - entity.transform.position.x;
                dy = transform.position.y - entity.transform.position.y;
                angle = Mathf.Atan2(dy, dx);
                Vector3 pullVector = new Vector2(Mathf.Cos(angle) * gravity,
                                                 Mathf.Sin(angle) * gravity);

                entity.transform.Translate(pullVector, Space.World);
            }
        }
        #endregion
    }

    private void OnTriggerEnter2D(Collider2D other) {
        if(other.gameObject.tag == "Entity") {
            Spaceship spaceshipEntity = other.GetComponent<Spaceship>();
            if(spaceshipEntity != null) {
                spaceshipEntity.Damage(damage);
            } else {
                Destroy(other.gameObject);
            }
        }
    }
}
