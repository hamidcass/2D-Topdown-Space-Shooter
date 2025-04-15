using UnityEngine;
using System.Collections.Generic;
using UnityEngine.UI;



public class Enemy : MonoBehaviour
{
    public GameObject level;
    private EnemyState currentState;
    private Animator enemyAnimator;

    public float health = 100f;
    public float max_health = 100f;
    public EnemyHealthBar healthBar;

    public float rotate_speed = 150f;
    private float move_speed = 1f;
    public float minX, maxX, minY, maxY;

    public float bulletSpeed = 10f;
    public float bulletDamage = 10f; //set to 10 so player die when shot 10x
    public GameObject enemy_bullet;
    public Transform bullet_start_pos;

    public GameObject enemy;
    public GameObject flame;
    public GameObject bulletStartPoint;
    private List<GameObject> bulletsToTrack = new List<GameObject>();
    Vector3 bulletDirection;

    public List<GameObject> enemies = new List<GameObject>();

    private float counter = 0;

    void Start()
    {
        //initialize all variables and set enemy state to the default (Alive)
        Vector3 bottomLeft = Camera.main.ViewportToWorldPoint(new Vector3(0, 0, 0));
        Vector3 topRight = Camera.main.ViewportToWorldPoint(new Vector3(1, 1, 0));
        
        minX = bottomLeft.x;
        maxX = topRight.x;
        minY = bottomLeft.y;
        maxY = topRight.y;

        enemyAnimator = GetComponent<Animator>();
        healthBar = GetComponentInChildren<EnemyHealthBar>();

        ChangeState(new EnemyStateAlive());   
    }

    //changes state of enemy
    public void ChangeState(EnemyState newState){
        currentState = newState;
    } 

    //rotate the enemy ship based on the direction of the player ship
    void facePlayer(){
        
        GameObject player_ship = GameObject.Find("Ship2");
        if (player_ship != null){
            Transform bulletStartPos = transform.Find("BulletStartPos");
            Transform ship_flame = transform.Find("enemyFlame");
            if (bulletStartPos != null && ship_flame != null)
            {

                //gets the "normal angle", meaning the direction from back of ship to front (flame to bullet hole)
                Vector3 flame_to_bullet_direction = bulletStartPos.position - ship_flame.position;
                flame_to_bullet_direction.Normalize();
    
                //gets direction from flame to player ship
                Vector3 flame_to_player_direction = player_ship.transform.position - ship_flame.position;
                flame_to_player_direction.Normalize();

                //gets angle betwen the two to calculate the rotation needed
                float angle = Vector3.SignedAngle(flame_to_bullet_direction, flame_to_player_direction, Vector3.forward);

                //perform the rotation
                transform.Rotate(0, 0, angle);

            }
        }
    }

    //move the enemy ship towards the player ship
    void moveToPlayer(){
        GameObject player_ship = GameObject.Find("Ship2");
        if (player_ship != null)
        {
            Vector3 player_position = player_ship.transform.position;
            Vector3 enemy_position = transform.position;

            Vector3 direction_to_player = player_position - enemy_position;
            direction_to_player.Normalize();

            transform.position += direction_to_player * move_speed * Time.deltaTime;
        }
    }

    //logic for creating bullets from enemies
    void shootBullet(){
        
        
        Transform bulletStartPos = transform.Find("BulletStartPos");
        Transform ship_flame = transform.Find("enemyFlame");

        if (bulletStartPos == null || ship_flame == null){
            return;
        }

        //Spawn bullet
        GameObject bullet = (GameObject)Instantiate(enemy_bullet, bulletStartPos);
        bullet.transform.SetParent(null);
        
        //change 'order in layer' to mkae bullet visible
        SpriteRenderer bulletRenderer = bullet.GetComponent<SpriteRenderer>();
        if (bulletRenderer != null){
            bulletRenderer.sortingOrder = 1; 
        }
        
        //Move bullet:

        //get direction
        bulletDirection = (bulletStartPos.position - ship_flame.position).normalized;

        //add bullet to list to track
        bulletsToTrack.Add(bullet);
        Rigidbody2D bullet_move = bullet.GetComponent<Rigidbody2D>();
        if (bullet_move != null){
            bullet_move.linearVelocity = bulletDirection * bulletSpeed;
        }
        else{
            Debug.Log("Bullet has no rigidbody");
        }
    }

    //check if any bullet has hit the player
    void checkIfBulletHitPlayer(){

        for (int j = 0; j < bulletsToTrack.Count; j++){
                
            GameObject bullet = bulletsToTrack[j];
            GameObject player = GameObject.Find("Ship2");
            if (bullet!=null && player!=null){
                if (bullet.transform.position.x > player.transform.position.x - 0.5 &&
                    bullet.transform.position.x < player.transform.position.x + 0.5 &&
                    bullet.transform.position.y > player.transform.position.y - 0.5 &&
                    bullet.transform.position.y < player.transform.position.y + 0.5){
                        
                        Destroy(bullet);
                        bulletsToTrack.RemoveAt(j); 

                        //deal damage to player
                        player.GetComponent<Player>().health -= bulletDamage;
                        Debug.Log("Player health: " + player.GetComponent<Player>().health);
                }
            }
        }
    }

    //Default state: Alive (Can move and shoot)
    public void BeAlive(){
        healthBar.updateHealthBar(health, max_health);
        facePlayer();
        checkIfBulletHitPlayer();

        counter += Time.deltaTime;

        //reference enemy ship is off screen and i dont want it to move into screen
        //so only ships within the camera view can move
        if (this.transform.position.x < minX || 
            this.transform.position.x > maxX || 
            this.transform.position.y < minY || 
            this.transform.position.y > maxY){
                
        }else{ 
        
            moveToPlayer();

            if (counter >= 3) { //shoot every 3 secs
                GameObject player = GameObject.Find("Ship2");
                if (player != null){
                    shootBullet();
                }
                counter = 0;
            }
        }
    }

    //check if enemy is dead
    public bool IsDead(){
        
        if (health <= 0){
            return true;
        }else{
            return false;
        }
    }

    //begins animation for explosion when enemy dies
    public void StartExplosion(){
        
        if (enemyAnimator == null){
            enemyAnimator = GetComponent<Animator>();
        }
        enemyAnimator.SetBool("expl", true);
    }

    //called on last keyframe of animation in the animation window. 
    //Removes object from screen after exploded and no longer visible 
    public void destroySelf(){
        Destroy(this.gameObject);
    }

    //State: Dead (enemy is dead and no longer moves or shoots)
    public void BeDead(){
        //update health bar
        healthBar.updateHealthBar(health, max_health);
        StartExplosion();
    }

    //method called on last keyframe of enemy explosion animation
    //adds +100 to score (arbitrary number i decided)
    public void addScore(){
        level.GetComponent<Level>().setScore(100);
    }

    
    void Update(){

        currentState.Execute(this);

    }
}
