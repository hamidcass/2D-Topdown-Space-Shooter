using UnityEngine;
using System.Collections.Generic;
using UnityEngine.UI;
using TMPro;

public class Level : MonoBehaviour
{
    private float ship_counter = 0; //used to control ship spawning
    private float ast_counter = 0; //used to control asteroid spawning
 

    //we can set these in the unity editor
    public GameObject player;
    public GameObject enemyShip; 
    public GameObject asteroid; //this is the prefab
    private GameObject new_asteroid; //tracks spawned asteroid

    //for setting boundaries of the screen
    Vector3 bottom_left, top_right;
    float min_x, max_x, min_y, max_y;

    private int game_score = 0;
    public TMP_Text score_text;


    void Start(){

        //init variables
        player = GameObject.Find("Ship2");

        bottom_left = Camera.main.ViewportToWorldPoint(new Vector3(0, 0, 0));
        top_right = Camera.main.ViewportToWorldPoint(new Vector3(1, 1, 0));
        min_x = bottom_left.x;
        max_x = top_right.x;
        min_y = bottom_left.y;
        max_y = top_right.y;

        //turn gamescore to string to show on screen
        score_text.text = game_score.ToString();

   }

   public void setScore(int score){
        game_score += score;
   }

    //spawn an enemy in the game
    void spawnEnemy(){
        if (player == null) return;

        //get player pos and radius
        Vector3 player_position = player.transform.position;
        float player_radius = player.GetComponent<Player>().player_radius;

        Vector3 enemySpawnPos;

        do {
        // pick a random angle to spawn
        float randAngle = Random.Range(0, 360);

        //pick a random distance outside player radius
        float randDistance = Random.Range(player_radius + 0.5f, player_radius + 0.5f);

        //turn into coordinates to spawn
        float spawn_x = player_position.x + randDistance * Mathf.Cos(randAngle);
        float spawn_y = player_position.y + randDistance * Mathf.Sin(randAngle);

        enemySpawnPos = new Vector3(spawn_x, spawn_y, 0f);

        //Debug.Log("Generated spawn position: " + enemySpawnPos);

        //check if spawn position is within screen bounds
         } while (enemySpawnPos.x < min_x || 
                  enemySpawnPos.x > max_x || 
                  enemySpawnPos.y < min_y || 
                  enemySpawnPos.y > max_y);

        
        //spawn enemy
        GameObject enemy_instance = (GameObject)Instantiate(enemyShip, enemySpawnPos, Quaternion.identity);

        //make enemy visible
        SpriteRenderer enemyRenderer = enemy_instance.GetComponent<SpriteRenderer>();
        if (enemyRenderer != null){
            enemyRenderer.sortingOrder = 1; 
        }
        //Debug.Log("Enemy spawned at " + enemySpawnPos);
    }

    //spawn asteroid into game
    void spawnAsteroid(){
        if (player == null) return;


            //get player pos
            Vector3 player_position = player.transform.position;
 
            Vector3 spawn_pos = new Vector3(0,0,0);

            //pick a random edge of screen 
            int rand_border = Random.Range(0,4);
            switch (rand_border){
                case 0: 
                    spawn_pos = new Vector3(Random.Range(min_x, max_x), min_y, 0);
                    break;
                case 1: 
                    spawn_pos = new Vector3(Random.Range(min_x, max_x), max_y, 0);
                    break;
                case 2: 
                    spawn_pos = new Vector3(min_x, Random.Range(min_y, max_y), 0);
                    break;
                case 3: 
                    spawn_pos = new Vector3(max_x, Random.Range(min_y, max_y), 0);
                    break;
            }

            Debug.Log("Spawn position: " + spawn_pos);

            Vector3 direction = (player_position - spawn_pos).normalized;

            //Spawn asteroid
            new_asteroid = (GameObject)Instantiate(asteroid, spawn_pos, Quaternion.identity);
    }

    public GameObject getAsteroidObject(){
        return new_asteroid;
    }
 
   
    void Update()
    {
        GameObject player = GameObject.Find("Ship2");
        if (player == null) return;

        //update game scrore
        score_text.text = game_score.ToString();
        
        ship_counter += Time.deltaTime;
        ast_counter += Time.deltaTime;

        //ship spawns every 3 seconds
        if (ship_counter >= 3) {
            spawnEnemy();
            ship_counter = 0;
        }

        //asteroid spawns every 6 seconds
        if (ast_counter >= 6) {
            Debug.Log("Spawn asteroid");
            spawnAsteroid();
            ast_counter = 0;
        }
    }
}
