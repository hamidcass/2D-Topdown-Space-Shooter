using UnityEngine;

public class Asteroid : MonoBehaviour
{

    private GameObject asteroid;
    private GameObject player;

    private float asteroid_damage = 100; //insta kills player

    private Vector3 direction;

    private Vector3 bottom_left, top_right;
    private float min_x, max_x, min_y, max_y;
    
    void Start(){

        //get screen edges
        bottom_left = Camera.main.ViewportToWorldPoint(new Vector3(0, 0, 0));
        top_right = Camera.main.ViewportToWorldPoint(new Vector3(1, 1, 0));
        min_x = bottom_left.x;
        max_x = top_right.x;
        min_y = bottom_left.y;
        max_y = top_right.y;
        
        //initialize direction from asteroid to player
        player = GameObject.Find("Ship2");
        if (player != null){
            direction = getDirection();
        }  
    }

    //get direction from asteroid to player
    Vector3 getDirection(){

        //position of player
        Vector3 player_position = player.transform.position;

        //position of asteroid
        Vector3 asteroid_position = this.transform.position;

        //direction
        Vector3 direction = (player_position - asteroid_position).normalized;

        return direction;


    }

    //move asteroid towards player based on direction
    void moveToPlayer(){
        
        //get rigidbody
        Rigidbody2D asteroid_move = this.GetComponent<Rigidbody2D>();
     
        if (asteroid_move != null){
            asteroid_move.linearVelocity = direction * 5f;
        }
        else{
            Debug.Log("Asteroid has no rigidbody");
        }

        //destory asteroid if it leaves screen
        if (this.transform.position.x > max_x ||
            this.transform.position.x < min_x ||
            this.transform.position.y > max_y ||
            this.transform.position.y < min_y){
                Debug.Log("Asteroid left screen");
                Destroy(this.gameObject); //destroy asteroid if it leaves screen
        }

    }

    //rotates asteroid
    void spin(){
        //rotate asteroid
        this.transform.Rotate(Vector3.forward, 50 * Time.deltaTime);

    }

    //check if player is hit by asteroid
    void checkIfHitPlayer(){
        
        if (player != null){
            float distance = Vector3.Distance(player.transform.position, this.transform.position);

            //if player hit, deal damage to player and destroy asteroid
            if (distance < 0.5f){
                player.GetComponent<Player>().health -= asteroid_damage;
                Destroy(this.gameObject);
            }
        }

    }

    // Update is called once per frame
    void Update()
    {
        moveToPlayer();
        spin();
        checkIfHitPlayer();
        
    }
}
