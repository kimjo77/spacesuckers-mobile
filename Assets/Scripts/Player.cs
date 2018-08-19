using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : MonoBehaviour {

    //public float moveSpeed = 8f;
    public float acceleration = 0.5f,
                 drag = 0.1f,
                 speed = 0,
                 maxSpeed = 8f,
                 cursorRadius = 2f,
                 moveSpeed = 3f,
                 velX,
                 velY,
                 dir;

    public Camera camera;

    public GameObject cursor;

    public Joystick moveStick, 
                    shootStick;

    public Vector3 pullVector = Vector3.zero;

    private Vector3 moveVector,
                    moveVectorPrev,
                    shootVector,
                    shootDirection;
	
	// Update is called once per frame
	void Update () {
        moveVector = (Vector3.right * moveStick.Horizontal + Vector3.up * moveStick.Vertical);

        if(moveVector != Vector3.zero) {
            speed += acceleration;

            if(speed > maxSpeed) {
                speed = maxSpeed;
            }

            transform.rotation = Quaternion.LookRotation(Vector3.forward, moveVector);
            transform.Translate( moveVector * speed * Time.deltaTime, Space.World);
            moveVectorPrev = moveVector;
        } else {
            speed -= drag;

            if(speed < 0) {
                speed = 0;
            }

            transform.Translate(moveVectorPrev * speed * Time.deltaTime, Space.World);
        }

        shootVector = (Vector3.right * shootStick.Horizontal + Vector3.up * shootStick.Vertical);
        shootDirection = shootStick.Direction;

        if(shootVector != Vector3.zero) {
            cursor.transform.rotation = Quaternion.LookRotation(Vector3.forward, shootVector);

            cursor.transform.position = transform.position;      
            cursor.transform.Translate(shootDirection.normalized * cursorRadius, Space.World);
            //cursor.transform.Translate(shootVector.normalized * cursorRadius);
            //cursor.transform.TransformDirection(shootStick.Direction.normalized * cursorRadius);
            //cursor.transform.position = shootStick.Direction.normalized * cursorRadius;

        }

        camera.transform.position = transform.position;
        camera.transform.rotation = Quaternion.identity;
    }

    private void LateUpdate() {
        camera.transform.SetPositionAndRotation(transform.position + new Vector3(0,0,-10) , Quaternion.identity);
    }
}
