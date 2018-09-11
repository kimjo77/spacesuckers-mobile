using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

public class Player : NetworkBehaviour {

    //public float moveSpeed = 8f;
    public float acceleration = 0.5f,
                 drag = 0.1f,
                 speed = 0,
                 maxSpeed = 8f,
                 cursorRadius = 2f,
                 moveSpeed = 3f,
                 velX,
                 velY,
                 dir,
                 maxRadius = 14f,
                 fireRate = 0.2f,
                 canFire = 0.0f;

    public GameObject cursor;

    public GameObject playerPrefab,
                      bulletPrefab;

    public Joystick moveStick, 
                    shootStick;

    public Vector3 pullVector = Vector3.zero;

    private Vector3 moveVector,
                    moveVectorPrev,
                    shootVector,
                    shootDirection;

    // Update is called once per frame
    void Update () {
        if(!isLocalPlayer) {
            return;
        }

        MovementJoystick();
        ShootingJoyStick();
        PlayerBounds();
    }

    private void MovementJoystick() {
        moveVector = (Vector3.right * moveStick.Horizontal + Vector3.up * moveStick.Vertical);

        if(moveVector != Vector3.zero) {
            speed += acceleration;

            if(speed > maxSpeed) {
                speed = maxSpeed;
            }

            playerPrefab.transform.rotation = Quaternion.LookRotation(Vector3.forward, moveVector);
            playerPrefab.transform.Translate(moveVector * speed * Time.deltaTime, Space.World);
            moveVectorPrev = moveVector;
        } else {
            speed -= drag;

            if(speed < 0) {
                speed = 0;
            }

            playerPrefab.transform.Translate(moveVectorPrev * speed * Time.deltaTime, Space.World);
        }
    }

    private void ShootingJoyStick() {
        shootVector = (Vector3.right * shootStick.Horizontal + Vector3.up * shootStick.Vertical);
        shootDirection = shootStick.Direction;

        if(shootVector != Vector3.zero) {
            cursor.transform.rotation = Quaternion.LookRotation(Vector3.forward, shootVector);

            cursor.transform.position = playerPrefab.transform.position;
            cursor.transform.Translate(shootDirection.normalized * cursorRadius, Space.World);

            if(Time.time > canFire) {
                CmdShoot();
                canFire = Time.time + fireRate;
            }
        }
    }
 
    private void PlayerBounds() {
        float distance = Vector2.Distance(Vector2.zero, playerPrefab.transform.position);
        if(maxRadius < distance) {
            Vector2 newVector = playerPrefab.transform.position - Vector3.zero;
            newVector = Vector2.ClampMagnitude(newVector, maxRadius);
            playerPrefab.transform.position = Vector2.zero + newVector;
        }
    }
        
    [Command]
    private void CmdShoot() {
        GameObject bullet = GameObject.Instantiate(bulletPrefab);
        Vector3 directionVector = shootDirection.normalized;
        bullet.GetComponent<Bullet>().directionVector = directionVector;
        bullet.transform.SetPositionAndRotation(cursor.transform.position, cursor.transform.rotation);
        bullet.GetComponent<Rigidbody2D>().velocity = directionVector * bullet.GetComponent<Bullet>().speed;
        
        NetworkServer.Spawn(bullet);
        Debug.Log("Someone shot!");
    }
}
