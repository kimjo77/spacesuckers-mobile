using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

public class Asteroid : NetworkBehaviour {

    public float debrisCount,
                 speed = 0,
                 drag = 0.1f,
                 explosionStrength = 10f;

    public Vector3 bulletDirectionVector,
                   directionVector;

    public GameObject explosionPrefab;
    public GameObject debrisPrefab;


    private void Update() {
        speed -= drag;
        if(speed <= 0) {
            speed = 0;
        } else {
            transform.Translate(directionVector * speed * Time.deltaTime, Space.World);
        }
    }

    private void OnTriggerEnter2D(Collider2D other) {
        if(other.gameObject.tag == "Entity") {
            Bullet bulletEntity = other.gameObject.GetComponent<Bullet>();

            if(bulletEntity != null) {
                GameObject explosion = Instantiate(explosionPrefab, transform.position, Quaternion.identity);
                if(isServer) {
                    NetworkServer.Spawn(explosion);
                }
                if(debrisPrefab != null) {
                    bulletDirectionVector = bulletEntity.directionVector;
                    float bulletAngle     = Mathf.Atan2(bulletDirectionVector.y, bulletDirectionVector.x) * Mathf.Rad2Deg;
                    //Debug.Log("Angle:" + bulletAngle);
                    float angleInterval   = 120 / debrisCount;
                    float angleOffset     = angleInterval / 2;
                    float angleInitial    = bulletAngle + 60 - angleOffset;

                    for(int i=0; i<debrisCount; i++) {
                        GameObject debris = Instantiate(debrisPrefab, transform.position, Quaternion.identity);
                        Asteroid debrisAsteroidComponent = debris.GetComponent<Asteroid>();
                        debrisAsteroidComponent.speed = explosionStrength;
                        float directionAngle = angleInitial - (angleInterval * i);

                        if(directionAngle > 180) {
                            directionAngle = -360 - directionAngle;
                        } else if(directionAngle < -180) {
                            directionAngle = 360 - directionAngle;
                        }

                        Vector3 direction = new Vector2(Mathf.Cos(directionAngle * Mathf.Deg2Rad), 
                                                        Mathf.Sin(directionAngle * Mathf.Deg2Rad));
                        debrisAsteroidComponent.directionVector = direction;

                        if(isServer) {
                            NetworkServer.Spawn(debris);
                        }
                    }
                }
                //NetworkServer.Destroy(other.gameObject);
                //NetworkServer.Destroy(this.gameObject);
                Destroy(other.gameObject);
                Destroy(this.gameObject);
            }
        }
    }
}
