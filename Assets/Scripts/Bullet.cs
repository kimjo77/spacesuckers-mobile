using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bullet : MonoBehaviour {

    public float speed, maxRadius;
    public Vector3 directionVector;

    void Update () {
        transform.Translate(directionVector * speed * Time.deltaTime, Space.World);

        BulletBounds();
    }

    public void BulletBounds() {
        float distance = Vector2.Distance(Vector2.zero, transform.position);
        if(maxRadius < distance) {
            Destroy(this.gameObject);
        }
    }
}
