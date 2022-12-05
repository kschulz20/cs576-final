using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class AJ : MonoBehaviour
{
    private Animator animation_controller;
    private CharacterController character_controller;
    public Vector3 movement_direction;
    public float walking_velocity;
    public Text text;
    public float velocity;
    public int num_lives;
    public bool has_won;
    public float time_of_death;
    public GameObject panel;
    public Text state;
    // Start is called before the first frame update
    void Start()
    {
        animation_controller = GetComponent<Animator>();
        character_controller = GetComponent<CharacterController>();
        movement_direction = new Vector3(0.0f, 0.0f, 0.0f);
        walking_velocity = 1.5f;
        velocity = 0.0f;
        num_lives = 5;
        has_won = false;
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKey(KeyCode.UpArrow))
        {

            velocity += 0.01f;
            
            if (Input.GetKey(KeyCode.RightControl) || Input.GetKey(KeyCode.LeftControl))
            {
                if (velocity > walking_velocity)
                    velocity = walking_velocity;
                animation_controller.SetBool("jumping", true);
            }
            else if (Input.GetKey(KeyCode.RightShift) || Input.GetKey(KeyCode.LeftShift))
            {
                if (velocity > walking_velocity * 3.0f)
                    velocity = walking_velocity * 3.0f;
                animation_controller.SetBool("running", true);
                animation_controller.SetBool("jumping", false);
            }
            else
            {
                if (velocity > walking_velocity)
                    velocity = walking_velocity;
                animation_controller.SetBool("running", false);
                animation_controller.SetBool("walking", true);
                animation_controller.SetBool("jumping", false);
            }


        }
        else if (Input.GetKey(KeyCode.DownArrow))
        {
            
            {
                velocity -= 0.01f;
                if (velocity < -1 * walking_velocity / 1.5f)
                    velocity = -1 * walking_velocity / 1.5f;
                animation_controller.SetBool("back", true);
                
            }

        }
        else if (Input.GetKey(KeyCode.RightControl) || Input.GetKey(KeyCode.LeftControl))
        {
            animation_controller.SetBool("jumping", true);
        }
        else
        {
            velocity = 0.0f;
            animation_controller.SetBool("walking", false);
            animation_controller.SetBool("back", false);
            animation_controller.SetBool("running", false);
            animation_controller.SetBool("jumping", false);
        }


        if (Input.GetKey(KeyCode.LeftArrow) && !animation_controller.GetCurrentAnimatorStateInfo(0).IsName("jumping"))
        {
            transform.Rotate(new Vector3(0.0f, -0.5f, 0.0f));
        }
        else if (Input.GetKey(KeyCode.RightArrow) && !animation_controller.GetCurrentAnimatorStateInfo(0).IsName("jumping"))
        {
            transform.Rotate(new Vector3(0.0f, 0.5f, 0.0f));
        }
        transform.position = new Vector3(transform.position.x, 0.0f, transform.position.z + velocity * Time.deltaTime);

        // you will use the movement direction and velocity in Turret.cs for deflection shooting 
        float xdirection = Mathf.Sin(Mathf.Deg2Rad * transform.rotation.eulerAngles.y);
        float zdirection = Mathf.Cos(Mathf.Deg2Rad * transform.rotation.eulerAngles.y);
        movement_direction = new Vector3(xdirection, 0.0f, zdirection);

        // character controller's move function is useful to prevent the character passing through the terrain
        // (changing transform's position does not make these checks)
        if (transform.position.y > 0.0f) // if the character starts "climbing" the terrain, drop her down
        {
            Vector3 lower_character = movement_direction * velocity * Time.deltaTime;
            lower_character.y = -100f; // hack to force her down
            character_controller.Move(lower_character);
        }
        else
        {
            character_controller.Move(movement_direction * velocity * Time.deltaTime);
        }
    }
}
