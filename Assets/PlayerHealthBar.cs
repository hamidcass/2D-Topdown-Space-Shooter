using UnityEngine;
using UnityEngine.UI;

public class PlayerHealthBar : MonoBehaviour
{

    public Slider health_slider;
    public Camera game_camera;
    public Transform player_transform;
    public Vector3 offset;

    private GameObject health_bar_canvas;
    private GameObject player_ship;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        //init parent variables
        health_bar_canvas = transform.parent.gameObject;
        player_ship = health_bar_canvas.transform.parent.gameObject;
        
    }

    public void updateHealthBar(float health, float max_health){
        health_slider.value = health/max_health;
    } 

    // Update is called once per frame
    void Update()
    {
        //modify position of health bar to always be above enemy ship:  
        transform.position = player_transform.position + offset;
        
        //modify rotation of health bar:

        //makes canvas rotation indepenent of parent rotation
        health_bar_canvas.transform.rotation = Quaternion.identity;

        //make canvas rotation = 0
        health_bar_canvas.transform.rotation = Quaternion.Euler(0, 0, 0);
        
    }
}
