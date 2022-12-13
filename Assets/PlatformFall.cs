using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlatformFall : MonoBehaviour
{
    bool jumped_on = false;
    float time_elapsed = 0.0f;
    float time_since_jumped_on = 0.0f;
    public float fall_speed;
    public float duration;

    void Start()
    {
        //How fast the platform falls after it's been jumped on
        fall_speed = 14.0f;
        //The duration of time for the platform to go from its normal color to red after being jumped on
        duration = 90.0f;
    }

    void OnTriggerEnter(Collider other)
    {
        jumped_on = true;
        Destroy(gameObject, 5);
    }
    // Update is called once per frame
    void Update()
    {
        if (jumped_on)
        {
            time_since_jumped_on += Time.deltaTime;

            //Change color of platform to red over time
            GetComponent<Renderer>().material.color = Color.Lerp(GetComponent<Renderer>().material.color, Color.red, time_since_jumped_on / duration);
        }

        if (time_since_jumped_on > 0.5f)
        {
            time_elapsed += Time.deltaTime / fall_speed;
            transform.position = new Vector3(transform.position.x, transform.position.y - time_elapsed, transform.position.z);
        }
    }
}
