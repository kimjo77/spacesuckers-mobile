using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

public class Spaceship : MonoBehaviour {

    public int health = 1000;

    public void Damage(int damage) {
        if((health -= damage) <= 0) {
            Destroy(this.gameObject);
        }
    }
}
