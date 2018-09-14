using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.SceneManagement;

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

    [SyncVar]
    public string spriteString = "";
    public bool spriteSet = false,
                spriteSetInitial = true,
                isLobbyObject = false;

    public GameObject cursor;

    public GameObject playerPrefab,
                      bulletPrefab,
                      ripplePrefab;

    public Joystick moveStick,
                    shootStick;

    public Vector3 pullVector = Vector3.zero;

    private Vector3 moveVector,
                    moveVectorPrev,
                    shootVector,
                    shootDirection;

    public Sprite bluePlayerSprite,
                  redPlayerSprite,
                  greenPlayerSprite,
                  orangePlayerSprite,
                  blueCursorSprite,
                  redCursorSprite,
                  greenCursorSprite,
                  orangeCursorSprite,
                  blueBulletSprite,
                  redBulletSprite,
                  greenBulletSprite,
                  orangeBulletSprite;

    public override void OnStartLocalPlayer() {
        if(SceneManager.GetActiveScene().name == "Screen_Lobby") {
            isLobbyObject = true;
        }

        CmdUpdateSprite();
        //CmdRemoveDuplicate();
        //Find Duplicate()

        //find localplayer;
        //get original objects [spaceship,cursor,bullet];
        //set set original objects to player [current contolling player]
        /*for(int i=0; i<playeObjects.Length; i++) {
            NetworkLobbyPlayer networkLobbyPlayer = playeObjects[i].GetComponent<NetworkLobbyPlayer>();
            Player playerEntity = playeObjects[i].GetComponent<Player>();

            if(this == playerEntity) {
                continue;
            }

            if(playerEntity.isLocalPlayer) {
                //Set positions of prefab to current position
                playerEntity.playerPrefab.transform.position = this.playerPrefab.transform.position;
                playerEntity.cursor.transform.position       = this.cursor.transform.position;
                playerEntity.bulletPrefab.transform.position = this.bulletPrefab.transform.position;

                //Set prefabs to original lobby Prefabs
                this.playerPrefab = playerEntity.playerPrefab;
                this.cursor       = playerEntity.cursor;
                this.bulletPrefab = playerEntity.bulletPrefab;

                Destroy(playeObjects[i]);
            }
        }*/
        if(!isLobbyObject) {
            Debug.Log("IM A GAME PLAYER!!!");

            GameObject[] playeObjects = GameObject.FindGameObjectsWithTag("Player");

            for(int i = 0; i < playeObjects.Length; i++) {
                Player playerEntity = playeObjects[i].GetComponent<Player>();

                if(playerEntity.isLocalPlayer && playerEntity.isLobbyObject) {
                    //Set positions of prefab to current position
                    playerEntity.playerPrefab.transform.position = this.playerPrefab.transform.position;
                    playerEntity.cursor.transform.position = this.cursor.transform.position;
                    playerEntity.bulletPrefab.transform.position = this.bulletPrefab.transform.position;

                    //Set prefabs to original lobby Prefabs
                    this.playerPrefab = playerEntity.playerPrefab;
                    this.cursor = playerEntity.cursor;
                    this.bulletPrefab = playerEntity.bulletPrefab;

                    //Delete the lobby player object
                    Debug.Log("Deleting!!!!!!!!!!!!!!!!!!!!!!!!!!!!");
                    CmdDestroyObject(playeObjects[i]);
                    break;
                }
            }
        }
    }

    private void Start() {

    }

    // Update is called once per frame
    void Update() {
        if(spriteSetInitial) {
            CmdChangeSprite();
            spriteSetInitial = true;
        }

        /*if(!spriteSet) {
            SetSpriteColor();
        }

        if(!isLocalPlayer) {
            return;
        }*/

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
                CmdShoot(this.gameObject);
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
    private void CmdShoot(GameObject owner) {
        GameObject bullet = GameObject.Instantiate(bulletPrefab);
        Vector3 directionVector = shootDirection.normalized;
        bullet.GetComponent<Bullet>().owner = owner;
        bullet.GetComponent<Bullet>().directionVector = directionVector;
        bullet.transform.SetPositionAndRotation(cursor.transform.position, cursor.transform.rotation);
        bullet.GetComponent<Rigidbody2D>().velocity = directionVector * bullet.GetComponent<Bullet>().speed;
        NetworkServer.Spawn(bullet);
        Debug.Log("Someone shot!");
    }

    [Command]
    private void CmdChangeSprite() {
        switch(GetComponent<NetworkLobbyPlayer>().slot) {
            case 0:
                spriteString = "blue";
                break;
            case 1:
                spriteString = "red";
                break;
            case 2:
                spriteString = "orange";
                break;
            case 3:
                spriteString = "green";
                break;
        }
        Debug.Log("Sprite sent! " + spriteString);
    }

    [Command]
    public void CmdUpdateSprite() {
        RpcUpdateSprite(netId);

        GameObject[] gameObjects = GameObject.FindGameObjectsWithTag("Player");
        for(int i = 0; i < gameObjects.Length; i++) {
            Player player = gameObjects[i].GetComponent<Player>();
            player.CmdUpdateAllSprites();
        }
    }

    [Command]
    public void CmdUpdateAllSprites() {
        RpcUpdateSprite(netId);
    }

    [ClientRpc]
    public void RpcUpdateSprite(NetworkInstanceId id) {
        //ClientScene.FindLocalObject(id).GetComponent<SpriteRenderer>().sprite = (Sprite) Resources.Load(spriteName);
        NetworkLobbyPlayer networkLobbyPlayer = ClientScene.FindLocalObject(id).GetComponent<NetworkLobbyPlayer>();
        Player playerEntity = ClientScene.FindLocalObject(id).GetComponent<Player>();
        switch(networkLobbyPlayer.slot) {
            case 0:
                playerEntity.playerPrefab.GetComponent<SpriteRenderer>().sprite = bluePlayerSprite;
                playerEntity.cursor.GetComponent<SpriteRenderer>().sprite       = blueCursorSprite;
                playerEntity.bulletPrefab.GetComponent<SpriteRenderer>().sprite = blueBulletSprite;
                break;
            case 1:
                playerEntity.playerPrefab.GetComponent<SpriteRenderer>().sprite = redPlayerSprite;
                playerEntity.cursor.GetComponent<SpriteRenderer>().sprite       = redCursorSprite;
                playerEntity.bulletPrefab.GetComponent<SpriteRenderer>().sprite = redBulletSprite;
                break;
            case 2:
                playerEntity.playerPrefab.GetComponent<SpriteRenderer>().sprite = orangePlayerSprite;
                playerEntity.cursor.GetComponent<SpriteRenderer>().sprite       = orangeCursorSprite;
                playerEntity.bulletPrefab.GetComponent<SpriteRenderer>().sprite = orangeBulletSprite;
                break;
            case 3:
                playerEntity.playerPrefab.GetComponent<SpriteRenderer>().sprite = greenPlayerSprite;
                playerEntity.cursor.GetComponent<SpriteRenderer>().sprite       = greenCursorSprite;
                playerEntity.bulletPrefab.GetComponent<SpriteRenderer>().sprite = greenBulletSprite;
                break;
        }
    }

    [Command]
    public void CmdDestroyObject(Object obj) {
        //RpcGetPlayerDuplicateProperties();
        Destroy(obj);
    }
}
