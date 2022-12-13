using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class Claire : MonoBehaviour {

    private Animator animation_controller;
    private CharacterController character_controller;
    public Vector3 movement_direction;
    public float walking_velocity;
    public float walking_backwards_velocity;
    public float crouching_forward_velocity;
    public float crouching_backwards_velocity;
    public float running_velocity;
    public float jumping_velocity;
    public float walk_startup_velocity;
    public float walk_backwards_startup_velocity;
    public float crouch_startup_velocity;
    public float crouch_backwards_startup_velocity;
    public float run_startup_velocity;
    public float jump_startup_velocity;
    public float rotation_speed;
    //public Text text;    
    public float velocity;
    public int num_lives;
    public bool has_won;
    private List<string> transition_parameters;
    private float time_of_death;
    public float gravity;
    // public EndScreen game_over_screen;
    // public EndScreen win_screen;
    
	// Use this for initialization
	void Start ()
    {
        animation_controller = GetComponent<Animator>(); 
        character_controller = GetComponent<CharacterController>();
        movement_direction = new Vector3(0.0f, 0.0f, 0.0f);

        rotation_speed = 0.4f;
        velocity = 0.0f;

        //Velocity limits
        walking_velocity = 8.0f;
        walking_backwards_velocity = walking_velocity / 1.5f;
        crouching_forward_velocity = walking_velocity / 2.0f;
        crouching_backwards_velocity = walking_velocity / 2.0f;
        running_velocity = walking_velocity * 2.0f;
        jumping_velocity = walking_velocity * 3.0f;

        //Movement startup velocity increments
        walk_startup_velocity = 0.1f;
        walk_backwards_startup_velocity = walk_startup_velocity * 0.75f;
        crouch_backwards_startup_velocity = 0.0375f;
        crouch_startup_velocity = walk_startup_velocity / 2.0f;
        run_startup_velocity = walk_startup_velocity * 1.25f;
        jump_startup_velocity = jump_startup_velocity * 1.5f;

        num_lives = 5;
        has_won = false;

        transition_parameters = new List<string>();
        transition_parameters.Add("isWalkingForwards");
        transition_parameters.Add("isWalkingBackwards");
        transition_parameters.Add("isRunning");
        transition_parameters.Add("isCrouchingForwards");
        transition_parameters.Add("isCrouchingBackwards");
        transition_parameters.Add("isJumping");

        time_of_death = float.PositiveInfinity;
        gravity = 0.0f;
    }

    // Update is called once per frame
    void Update()
    {
        //text.text = "Lives left: " + num_lives;

        // //Since animation for death has ended, stop all animation from occurring
        // if (Time.time - time_of_death > 4.0f)
        // {
        //     animation_controller.enabled = false;
        //     game_over_screen.Display();
        // }

        // //If player has won we want to be in the idle state
        // if (has_won)
        // {
        //     foreach (string transition in transition_parameters)
        //     {
        //         animation_controller.SetBool(transition, false);
        //     }
        //     win_screen.Display();
        // }

        ////////////////////////////////////////////////
        // WRITE CODE HERE:
        // (a) control the animation controller (animator) based on the keyboard input. Adjust also its velocity and moving direction. 
        // (b) orient (i.e., rotate) your character with left/right arrow [do not change the character's orientation while jumping]
        // (c) check if the character is out of lives, call the "death" state, let the animation play, and restart the game
        // (d) check if the character reached the target (display the message "you won", freeze the character (idle state), provide an option to restart the game
        // feel free to add more fields in the class        
        ////////////////////////////////////////////////

        // gravity -= 1f * Time.deltaTime;
        // if (!character_controller.isGrounded)
        // {
        //     Debug.Log("boing");
        //     character_controller.Move( new Vector3(transform.position.x, gravity, transform.position.z) );
        // }
        // else
        //     gravity = 0.0f;


        // you don't need to change the code below (yet, it's better if you understand it). Name your FSM states according to the names below (or change both).
        // do not delete this. It's useful to shift the capsule (used for collision detection) downwards. 
        // The capsule is also used from turrets to observe, aim and shoot (see Turret.cs)
        // If the character is crouching, then she evades detection. 
        bool is_crouching = false;
        if ( (animation_controller.GetCurrentAnimatorStateInfo(0).IsName("CrouchForwards"))
         ||  (animation_controller.GetCurrentAnimatorStateInfo(0).IsName("CrouchBackwards")) )
        {
            is_crouching = true;
        }

        if (is_crouching)
        {
            GetComponent<CapsuleCollider>().center = new Vector3(GetComponent<CapsuleCollider>().center.x, 0.0f, GetComponent<CapsuleCollider>().center.z);
        }
        else
        {
            GetComponent<CapsuleCollider>().center = new Vector3(GetComponent<CapsuleCollider>().center.x, 0.9f, GetComponent<CapsuleCollider>().center.z);
        }


        // you will use the movement direction and velocity in Turret.cs for deflection shooting 
        float xdirection = Mathf.Sin(Mathf.Deg2Rad * transform.rotation.eulerAngles.y);
        float zdirection = Mathf.Cos(Mathf.Deg2Rad * transform.rotation.eulerAngles.y);
        movement_direction = new Vector3(xdirection, 0.0f, zdirection);
        Vector3 opp_direction = new Vector3(-xdirection, 0.0f, -zdirection);
        
        if (!animation_controller.GetCurrentAnimatorStateInfo(0).IsName("Death"))
        {
            //Movement
            if (Input.GetKey(KeyCode.UpArrow) && !Input.GetKey(KeyCode.LeftShift))
            {
                handle_movement(walk_startup_velocity, walking_velocity, "isWalkingForwards");
                disable_other_transitions("isWalkingForwards");
            }
            if (Input.GetKey(KeyCode.UpArrow) && Input.GetKey(KeyCode.LeftShift))
            {
                handle_movement(run_startup_velocity, running_velocity, "isRunning");
                disable_other_transitions("isRunning");
            }
            if (Input.GetKey(KeyCode.Space) && !animation_controller.GetCurrentAnimatorStateInfo(0).IsName("JumpOver") && !animation_controller.GetCurrentAnimatorStateInfo(0).IsName("WalkForwards"))
            {
                //This only runs once which isn't intended but hacky solution below fixes jumping
                handle_movement(jump_startup_velocity, jumping_velocity, "isJumping");
                disable_other_transitions("isJumping");
            }
            if (Input.GetKey(KeyCode.UpArrow) && Input.GetKey(KeyCode.LeftControl))
            {
                handle_movement(crouch_startup_velocity, crouching_forward_velocity, "isCrouchingForwards");
                disable_other_transitions("isCrouchingForwards");
            }
            if (Input.GetKey(KeyCode.DownArrow) && !animation_controller.GetCurrentAnimatorStateInfo(0).IsName("JumpOver"))
            {
                handle_movement(walk_backwards_startup_velocity, walking_backwards_velocity, "isWalkingBackwards");
                disable_other_transitions("isWalkingBackwards");
            }
            if (Input.GetKey(KeyCode.DownArrow) && Input.GetKey(KeyCode.LeftControl) && !animation_controller.GetCurrentAnimatorStateInfo(0).IsName("JumpOver"))
            {
                handle_movement(crouch_backwards_startup_velocity, crouching_backwards_velocity, "isCrouchingBackwards");
                disable_other_transitions("isCrouchingBackwards");
            }
            if (!Input.GetKey(KeyCode.UpArrow) && !Input.GetKey(KeyCode.DownArrow))
            {
                velocity = 0.0f;
                foreach (string transition in transition_parameters)
                {
                    animation_controller.SetBool(transition, false);
                }
            }

            //Hacky solution to getting jump to ignore velocity changes from walking forward/running forward
            if (animation_controller.GetCurrentAnimatorStateInfo(0).IsName("JumpOver"))
            {
                velocity = jumping_velocity;
            }

            if (!animation_controller.GetCurrentAnimatorStateInfo(0).IsName("JumpOver"))
            {
                //Rotate character
                if (Input.GetKey(KeyCode.RightArrow))
                {
                    transform.Rotate(new Vector3(0.0f, rotation_speed, 0.0f));
                }
                else if (Input.GetKey(KeyCode.LeftArrow))
                {
                    transform.Rotate(new Vector3(0.0f, -rotation_speed, 0.0f));
                }
            }

            if (num_lives <= 0)
            {
                if (!animation_controller.GetCurrentAnimatorStateInfo(0).IsName("Death"))
                {
                    time_of_death = Time.time;
                    animation_controller.SetTrigger("death");
                } 
            }
        
            
            //If the player is trying to walk backward, reverse the movement direction
            if (animation_controller.GetBool("isWalkingBackwards") || animation_controller.GetBool("isCrouchingBackwards"))
                movement_direction = opp_direction;

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

    //Generalized method for handling walking, crouching, and sprinting
    void handle_movement(float startup_velocity, float velocity_limit, string animation_boolean)
    {
        velocity += startup_velocity;
        if (velocity > velocity_limit)
            velocity = velocity_limit;
        animation_controller.SetBool(animation_boolean, true);
    }

    void disable_other_transitions(string keep_transition)
    {
        List<string> copy_list = new List<string>(transition_parameters);
        copy_list.Remove(keep_transition);
        foreach (string transition in copy_list)
        {
            animation_controller.SetBool(transition, false);
        }
    }
}
