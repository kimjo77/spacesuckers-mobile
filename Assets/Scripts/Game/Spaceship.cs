using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

public class Spaceship : MonoBehaviour {

    public Player player;

    public int health = 1000;
    public float shardDeductionPercentage = 0.25f;

    public void Damage(int damage) {
        if((health -= damage) <= 0) {
            player.ScatterShards(-1, shardDeductionPercentage);
            player.Die();
        }
    }

    public void Damage(int shardOwnerID, int damage) {
        if((health -= damage) <= 0) {
            player.ScatterShards(shardOwnerID, shardDeductionPercentage);
            player.Die();
        }
    }

    public void AddPoints(int value) {
        player.AddPoints(value);
    }

    public int GetPoints() {
        return player.GetPoints();
    }

    public void DeductPoints(int value) {
        player.DeductPoints(value);
    }


    public void OnTriggerEnter2D(Collider2D other) {
        if(other.tag == "Entity") {
            Bullet bulletEntity = other.GetComponent<Bullet>();

            if(bulletEntity != null) {
                Damage(bulletEntity.ownerID, bulletEntity.damage);
                Destroy(other.gameObject);
            }
        }
    }


}
