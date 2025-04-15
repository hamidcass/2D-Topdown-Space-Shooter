using UnityEngine;
using System.Collections.Generic;

public class Player : MonoBehaviour
{
    private PlayerState currentState;
    private Animator playerAnimator;

    public PlayerHealthBar healthBar;
    public float health = 100f;
    public float max_health = 100f;

    public float player_radius = 1f;
    public float rotate_speed = 300f;
    public float move_speed = 5f;

    public float minX, maxX, minY, maxY;

    public float bulletSpeed = 10f;
    public float bulletDamage = 50f; //set to 50 so enemies die when shot twice
    public GameObject player_bullet;
    public Transform bullet_start_pos;

    public GameObject player;
    public GameObject flame;
    public GameObject bulletStartPoint;
    private List<GameObject> bulletsToTrack = new List<GameObject>();
    Vector3 bulletDirection;

    public List<GameObject> enemies = new List<GameObject>();


    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start(){
        Vector3 screenBottomLeft = Camera.main.ViewportToWorldPoint(new Vector3(0, 0, 0));
        Vector3 screenTopRight = Camera.main.ViewportToWorldPoint(new Vector3(1, 1, 0));

        //init variables
        player = GameObject.Find("Player");
        flame = GameObject.Find("playerFlame");
        bulletStartPoint = GameObject.Find("BulletStartPoint");
        
        minX = screenBottomLeft.x;
        maxX = screenTopRight.x;
        minY = screenBottomLeft.y;
        maxY = screenTopRight.y;

        playerAnimator = GetComponent<Animator>();
        healthBar = GetComponentInChildren<PlayerHealthBar>();

        //default player state is alive
        ChangeState(new PlayerStateAlive());
        
    }

    //changes player state
    public void ChangeState(PlayerState newState){
        currentState = newState;
    } 

    //move player based off keyboard input
    void movePlayer(){

        float rotation = 0f;
        //Debug.Log("RS: " + rotate_speed);

        //TURNING
        if (Input.GetKey(KeyCode.A)){
            rotation = rotate_speed * Time.deltaTime;
        }
        else if (Input.GetKey(KeyCode.D)){
            rotation = -rotate_speed * Time.deltaTime;
        }

        //perform the rotation
        transform.Rotate(0, 0, rotation);

        //MOVING
        if (Input.GetKey(KeyCode.W)){
            transform.Translate(Vector3.down * Time.deltaTime * move_speed);        
        }
        else if (Input.GetKey(KeyCode.S)){
            transform.Translate(Vector3.up * Time.deltaTime * move_speed);
        }

        //bound player to screen and perform transform
        Vector3 bounded_position = transform.position;
        bounded_position.x = Mathf.Clamp(bounded_position.x, minX, maxX);
        bounded_position.y = Mathf.Clamp(bounded_position.y, minY, maxY);
        transform.position = bounded_position;
        
    }

    //shoot bullet based off user input
    void shootBullet(){

        //player presses space key
        if (Input.GetKeyDown(KeyCode.Space)){
            Debug.Log("Shoot");

            //make bullet in game
            GameObject bullet = (GameObject)Instantiate(player_bullet, bullet_start_pos);

            //detach it from parent
            bullet.transform.SetParent(null);

            //change 'order in layer' to mkae bullet visible
            SpriteRenderer bulletRenderer = bullet.GetComponent<SpriteRenderer>();
            if (bulletRenderer != null){
                bulletRenderer.sortingOrder = 1; 
            }

            //change bullet direction
            flame = GameObject.Find("playerFlame");
            bulletStartPoint = GameObject.Find("BulletStartPoint");
            bulletDirection = (bulletStartPoint.transform.position - flame.transform.position).normalized;

            //add bullet to list to track
            bulletsToTrack.Add(bullet);

            //make bullet move on screen
            Rigidbody2D bullet_move = bullet.GetComponent<Rigidbody2D>();
            if (bullet_move != null){
                bullet_move.linearVelocity = bulletDirection * bulletSpeed;
            }
            else{
                Debug.Log("Bullet has no rigidbody");
            }
        }     
    }

    //manages destruction of bullets when leaving screen view
    void checkIfBulletOnScreen(){

        for (int i = 0; i < bulletsToTrack.Count; i++){
            GameObject bullet = bulletsToTrack[i];
            if (bullet!=null){
                if (bullet.transform.position.x > maxX ||
                    bullet.transform.position.x < minX ||
                    bullet.transform.position.y > maxY ||
                    bullet.transform.position.y < minY){
                        //Debug.Log("Bullet left screen");
                        Destroy(bullet);
                        bulletsToTrack.RemoveAt(i); 
                }
            }
        }
    }


    void checkIfBulletHitEnemy(){
        //search all enemies in game and add to list if not added
        enemies = new List<GameObject>(GameObject.FindGameObjectsWithTag("enemy"));
        //Debug.Log(enemies.Count); 
        for (int i = 0; i < enemies.Count; i++){
            for (int j = 0; j < bulletsToTrack.Count; j++){
                //check if any bullet has hit an enemy
                GameObject bullet = bulletsToTrack[j];
                GameObject enemy = enemies[i];

                if (bullet!=null && enemy!=null){
                    float distance = Vector3.Distance(enemy.transform.position, bullet.transform.position);
                    if (distance < 0.5f){
                        Debug.Log("Bullet hit enemy");
                        Destroy(bullet);
                        bulletsToTrack.RemoveAt(j); 

                        //deal damage to enemy
                        enemy.GetComponent<Enemy>().health -= bulletDamage;
                      

                    }
                }
            }
        }
    }

    //Default state: Alive (player is free to move and shoot)
    public void BeAlive(){
        healthBar.updateHealthBar(health, max_health);
        movePlayer();
        shootBullet();
        checkIfBulletOnScreen();
        checkIfBulletHitEnemy();
        
    }

    //check if player is dead
    public bool IsDead(){
        if (health <= 0f){
            return true;
        }
        else{
            return false;
        }
    }

    //method called in animation window on last keyframe
    //destorys player object
    public void destroySelf(){
        Destroy(this.gameObject);
    }

    //begins the animation of exploding
    public void StartExplosion(){
     
        if (playerAnimator == null){
            playerAnimator = GetComponent<Animator>();
        }
        playerAnimator.SetBool("expl", true);
    }
  
    //State: Dead (player is dead and cannot move or shoot)
    public void BeDead(){
        healthBar.updateHealthBar(health, max_health);
        player = GameObject.Find("Ship2");
        if (player!=null){
            //play animation
            StartExplosion();
        }
    }

    // Update is called once per frame
    void Update(){
        currentState.Execute(this);   
   
    }
}
