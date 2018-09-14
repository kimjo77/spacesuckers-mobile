using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

public class Asteroid : NetworkBehaviour {

    public int health = 250,
               damage = 750;

    public float stun = 3;

    public int shardCount = 0;

    public float debrisCount,
                 speed = 0,
                 drag = 0.1f,
                 explosionStrength = 10f;

    public Vector3 bulletDirectionVector,
                   directionVector;

    public GameObject explosionPrefab;
    public GameObject debrisPrefab;
    public GameObject shardPrefab;


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
            Bullet    bulletEntity    = other.gameObject.GetComponent<Bullet>();
            Spaceship spaceshipEntity = other.gameObject.GetComponent<Spaceship>();

            if(bulletEntity != null) {
                if((health -= bulletEntity.damage) <= 0) {
                    GameObject explosion = Instantiate(explosionPrefab, transform.position, Quaternion.identity);

                    if(isServer) {
                        NetworkServer.Spawn(explosion);
                    }

                    if(debrisPrefab != null) {
                        bulletDirectionVector = bulletEntity.directionVector;
                        float bulletAngle = Mathf.Atan2(bulletDirectionVector.y, bulletDirectionVector.x) * Mathf.Rad2Deg;
                        //Debug.Log("Angle:" + bulletAngle);
                        float angleInterval = 120 / debrisCount;
                        float angleOffset = angleInterval / 2;
                        float angleInitial = bulletAngle + 60 - angleOffset;

                        for(int i = 0; i < debrisCount; i++) {
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

                    ProduceAndScatterShards(bulletEntity.owner);

                    Destroy(this.gameObject);
                }
                Destroy(other.gameObject);
            } else if(spaceshipEntity != null) {
                spaceshipEntity.Damage(damage);
                GameObject explosion = Instantiate(explosionPrefab, transform.position, Quaternion.identity);
                if(isServer) {
                    NetworkServer.Spawn(explosion);
                }
                Destroy(this.gameObject);
            }
        }
    }

    private void ProduceAndScatterShards(GameObject shardOwner) {
        for(int i=0; i<shardCount; i++) {
            GameObject shard = Instantiate(shardPrefab, transform.position, Quaternion.identity);
            float angle = Random.Range(-180, 180);

            Vector3 direction = new Vector2(Mathf.Cos(angle * Mathf.Deg2Rad),
                                            Mathf.Sin(angle * Mathf.Deg2Rad));

            shard.GetComponent<Shard>().directionVector = direction;
            shard.GetComponent<Shard>().owner = shardOwner.GetComponent<Player>().playerPrefab;

            NetworkServer.Spawn(shard);
        }
    }
}
