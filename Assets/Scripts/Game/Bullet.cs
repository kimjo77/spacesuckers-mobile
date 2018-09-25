using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

public class Bullet : NetworkBehaviour {

    public int damage = 50;

    public float speed, maxRadius;

    [SyncVar]
    public int ownerID;


    public Vector3 directionVector;

    private Rigidbody2D rb2D;

    void Start() {
        rb2D = GetComponent<Rigidbody2D>();
    }

    void Update () {

        if(rb2D.velocity == Vector2.zero) {
            float angleRad = (transform.rotation.eulerAngles.z + 90) * Mathf.Deg2Rad;
            Vector3 velocityVector = new Vector2(Mathf.Cos(angleRad) * speed,
                                                 Mathf.Sin(angleRad) * speed);
            rb2D.velocity = velocityVector;
            directionVector = velocityVector;
        }

        BulletBounds();
    }

    public void BulletBounds() {
        float distance = Vector2.Distance(Vector2.zero, transform.position);
        if(maxRadius < distance) {
            Destroy(this.gameObject);
        }
    }
}