using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Apple : MonoBehaviour
{
    public Vector3 direction;
    public float velocity;
    public float birth_time;
    public GameObject birth_turret;
    public GameObject claire;
    private BossLevelOne level;

    // Start is called before the first frame update
    void Start()
    {        
        //claire = GameObject.Find("Claire");
        GameObject level_obj = GameObject.FindGameObjectWithTag("BossLevelOne");
        if (level_obj == null)
            Debug.Log("Error: Could not find game object with tag BossLevelOne");
        level = level_obj.GetComponent<BossLevelOne>();
    }

    // Update is called once per frame
    void Update()
    {
        if (Time.time - birth_time > 30.0f)  // apples live for 30 sec
        {
            Destroy(transform.gameObject);
        }
        transform.position = transform.position + velocity * direction * Time.deltaTime;
    }

    private void OnTriggerEnter(Collider other)
    {
        ////////////////////////////////////////////////
        // WRITE CODE HERE:
        // (a) if the object collides with Claire, subtract one life from her, and destroy the apple
        // (b) if the object collides with another apple, or its own turret that launched it (birth_turret), don't do anything
        // (c) if the object collides with anything else (e.g., terrain, a different turret), destroy the apple
        ////////////////////////////////////////////////
        if (other.gameObject.name == "Aj")
        {
            //Play sfx for when apple hits the player
            level.audio_source.PlayOneShot(level.apple_hit_sfx);
            level.player_lives--;
            Destroy(transform.gameObject);
        }
        else if (birth_turret != null)
        {
            if (other.gameObject.name != birth_turret.name && other.gameObject.name != transform.name)
                Destroy(transform.gameObject);
        }
    }
}
