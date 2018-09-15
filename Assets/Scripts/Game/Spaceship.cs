using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

public class Spaceship : MonoBehaviour {

    public Player player;
    public int health = 1000;

    public void Damage(int damage) {
        if((health -= damage) <= 0) {
            //Destroy(this.gameObject);
            player.Die();
        }
    }

    public void AddPoints(int value) {
        player.AddPoints(value);
    }

    public void OnTriggerEnter2D(Collider2D other) {
        if(other.tag == "Entity") {
            Bullet bulletEntity = other.GetComponent<Bullet>();

            if(bulletEntity != null) {
                Damage(bulletEntity.damage);
                Destroy(other.gameObject);
            }
        }
    }
}
