using UnityEngine;
using UnityEngine.UI;

public class EnemyHealthBar : MonoBehaviour
{
    public Slider health_slider;
    public Camera game_camera;
    public Transform enemy_transform;
    public Vector3 offset;

    private GameObject health_bar_canvas;
    private GameObject enemy_ship;

    void Start()
    {
        //initialize parent variables
        health_bar_canvas = transform.parent.gameObject;
        enemy_ship = health_bar_canvas.transform.parent.gameObject;
   
    }

    public void updateHealthBar(float health, float max_health){
        health_slider.value = health/max_health;

    } 

    // Update is called once per frame
    void Update()
    {

        //modify position of health bar to always be above enemy ship:  
        transform.position = enemy_transform.position + offset;
        
    
        //modify rotation of health bar:

        //makes canvas rotation indepenent of parent rotation
        health_bar_canvas.transform.rotation = Quaternion.identity;

        //make canvas rotation = 0
        health_bar_canvas.transform.rotation = Quaternion.Euler(0, 0, 0);
     
        
    }
}
