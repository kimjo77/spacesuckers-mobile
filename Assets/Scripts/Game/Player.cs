using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.SceneManagement;

public class Player : NetworkBehaviour {

    public int shardLimit = 20;
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
                 canFire = 0.0f,
                 respawnTime = 3;

    [SyncVar]
    public string spriteString = "";
    public bool spriteSet = false,
                spriteSetInitial = true,
                isLobbyObject = false,
                isDead = false,
                isInGame = false;

    public GameObject cursor;

    public GameObject playerPrefab,
                      bulletPrefab,
                      shardPrefab;

    public GameObject blueBulletPrefab, 
                      redBulletPrefab, 
                      greenBulletPrefab, 
                      orangeBulletPrefab;

    public CameraFollow camerafollow;

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

    public Global global = null;

    public override void OnStartLocalPlayer() {
        CmdUpdateSprite();
    }

    // Update is called once per frame
    void Update() {
        if(isInGame) {
            if(global == null) {
                //find global
                global = FindObjectOfType<Global>();
            }
        } else {
            if(SceneManager.GetActiveScene().name == "Game_Multiplayer") {
                isInGame = true;
            }
        }

        if(!isDead) {
            MovementJoystick();
            ShootingJoyStick();
        }
        
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

    public void SetSpawnPoint() {
        NetworkStartPosition[] spawnPoints = FindObjectsOfType<NetworkStartPosition>();
        Spaceship[] spaceships = FindObjectsOfType<Spaceship>();
        float[] shortestDistance = new float[spawnPoints.Length];

        if(spaceships.Length != 0) {
            for(int i=0; i<shortestDistance.Length; i++) {
                shortestDistance[i] = Vector2.Distance(spawnPoints[i].transform.position, spaceships[0].transform.position);
            }

            //foreach point - find the shortest distance between all spaceships - save sa shortest distance
            for(int i=0; i<spawnPoints.Length; i++) {
                for(int j=0; j<spaceships.Length; j++) {
                    float distance = Vector2.Distance(spawnPoints[i].transform.position, spaceships[j].transform.position);
                    if(distance < shortestDistance[i]) {
                        shortestDistance[i] = distance;
                    }
                }
            }

            //foreach shortest distance - index of the farthest distance - save the index
            int index = 0;
            for(int i=0; i<shortestDistance.Length; i++) {
                if(shortestDistance[index] < shortestDistance[i]) {
                    index = i;
                }
            }

            //spawn the player on the location of the point in the said index
            playerPrefab.transform.position = spawnPoints[index].transform.position;
        } else {
            //spawn the player on the location of the point in the random index
            playerPrefab.transform.position = spawnPoints[Random.Range(0,spawnPoints.Length)].transform.position;
        }

        
    }

    public void ScatterShards(int shardOwnerID, float shardDeductionPercentage) {
        //Compute how many to produce
        int shardCount, shardValue = shardPrefab.GetComponent<Shard>().shardValue;
        int deductionValue = (int) (GetPoints() * shardDeductionPercentage);
        DeductPoints(deductionValue);
        shardCount = deductionValue / shardValue;

        if(shardCount > shardLimit) {
            shardValue = deductionValue / shardLimit;
            shardCount = 20;
        }

        CmdProduceAndScatterShards(shardOwnerID, shardCount, shardValue);
    }

    public void AddPoints(int value) {
        FindObjectOfType<Global>().AddPoints(this, value);
    }

    public int GetPoints() {
        return FindObjectOfType<Global>().GetScore(this);
    }

    public void DeductPoints(int value) {
        FindObjectOfType<Global>().DeductPoints(this, value);
    }

    //--Respawing
    public void Die() {
        isDead = true;
        playerPrefab.gameObject.SetActive(false);
        StartCoroutine(DieRoutine());
    }
    public IEnumerator DieRoutine() {
        yield return new WaitForSeconds(respawnTime);
        SetSpawnPoint();
        playerPrefab.gameObject.SetActive(true);
        isDead = false;
    }

    [Command]
    private void CmdShoot(GameObject owner) {
        GameObject bullet = GameObject.Instantiate(bulletPrefab);
        Vector3 directionVector = shootDirection.normalized;

        Bullet bulletEntity = bullet.GetComponent<Bullet>();

        bulletEntity.ownerID = GetComponent<NetworkLobbyPlayer>().slot;
        bulletEntity.directionVector = directionVector;

        bullet.transform.SetPositionAndRotation(cursor.transform.position, cursor.transform.rotation);
        bullet.GetComponent<Rigidbody2D>().velocity = directionVector * bulletEntity.speed;
        NetworkServer.Spawn(bullet);
        Debug.Log("Someone shot!");
    }

    [Command]
    public void CmdUpdateSprite() {


        /*GameObject[] gameObjects = GameObject.FindGameObjectsWithTag("Player");
        for(int i = 0; i < gameObjects.Length; i++) {
            Player player = gameObjects[i].GetComponent<Player>();
            player.CmdUpdateAllSprites();
        }*/
        Player[] players = FindObjectsOfType<Player>();
        for(int i = 0; i < players.Length; i++) {
            players[i].CmdUpdateAllSprites();
        }

        RpcUpdateSprite(netId);
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
                playerEntity.bulletPrefab                                       = blueBulletPrefab;
                playerEntity.cursor.GetComponent<SpriteRenderer>().sprite       = blueCursorSprite;
                break;
            case 1:
                playerEntity.playerPrefab.GetComponent<SpriteRenderer>().sprite = redPlayerSprite;
                playerEntity.bulletPrefab                                       = redBulletPrefab;
                playerEntity.cursor.GetComponent<SpriteRenderer>().sprite       = redCursorSprite;
                break;
            case 2:
                playerEntity.playerPrefab.GetComponent<SpriteRenderer>().sprite = orangePlayerSprite;
                playerEntity.bulletPrefab                                       = orangeBulletPrefab;
                playerEntity.cursor.GetComponent<SpriteRenderer>().sprite       = orangeCursorSprite;
                break;
            case 3:
                playerEntity.playerPrefab.GetComponent<SpriteRenderer>().sprite = greenPlayerSprite;
                playerEntity.bulletPrefab                                       = greenBulletPrefab;
                playerEntity.cursor.GetComponent<SpriteRenderer>().sprite       = greenCursorSprite;
                break;
        }
    }

    [Command]
    private void CmdProduceAndScatterShards(int shardOwnerID, int shardCount, int shardValue) {
        //Produce and Scatter Shards
        for(int i = 0; i < shardCount; i++) {
            GameObject shard = Instantiate(shardPrefab, playerPrefab.transform.position, Quaternion.identity);
            float angle = Random.Range(-180, 180);

            Vector3 direction = new Vector2(Mathf.Cos(angle * Mathf.Deg2Rad),
                                            Mathf.Sin(angle * Mathf.Deg2Rad));

            Shard shardEnity = shard.GetComponent<Shard>();

            shardEnity.directionVector = direction;
            shardEnity.ownerID = shardOwnerID;
            shardEnity.shardValue = shardValue;

            NetworkServer.Spawn(shard);
        }
    }

    [Command]
    public void CmdDestroyObject(GameObject obj) {
        //RpcGetPlayerDuplicateProperties();
        Destroy(obj);
    }
}
