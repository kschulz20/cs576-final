﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Old_Turret : MonoBehaviour
{
    private float shooting_delay; 
    private GameObject projectile_template;
    private Vector3 direction_from_turret_to_aj;
    private Vector3 shooting_direction;
    private Vector3 projectile_starting_pos;
    private float projectile_velocity;
    private bool aj_is_accessible;
    public bool cutscene_being_shown;
    private AJ aj_component;

    // Start is called before the first frame update
    void Start()
    {
        projectile_template = (GameObject)Resources.Load("Apple/Prefab/Apple", typeof(GameObject));  // projectile prefab
        if (projectile_template == null)
            Debug.LogError("Error: could not find the apple prefab in the project! Did you delete/move the prefab from your project?");
        shooting_delay = 2.0f;
        projectile_velocity = 22.0f;
        direction_from_turret_to_aj = new Vector3(0.0f, 0.0f, 0.0f);
        projectile_starting_pos = new Vector3(0.0f, 0.0f, 0.0f);
        aj_is_accessible = false;
        cutscene_being_shown = false;
        aj_component = GameObject.Find("Aj").GetComponent<AJ>();

        StartCoroutine("Spawn");
    }

    // Update is called once per frame
    void Update()
    {
        GameObject aj = GameObject.Find("Aj");
        if (aj == null)
            Debug.LogError("Error: could not find the game character 'Aj' in the scene. Did you delete the model Aj from your scene?");
        Vector3 aj_centroid = aj.GetComponent<CapsuleCollider>().bounds.center;
        Vector3 turret_centroid = GetComponent<Collider>().bounds.center;
        direction_from_turret_to_aj = aj_centroid - turret_centroid;
        direction_from_turret_to_aj.Normalize();

        RaycastHit hit;
        if (!cutscene_being_shown)
        {
            if (Physics.Raycast(turret_centroid, direction_from_turret_to_aj, out hit, Mathf.Infinity))
            {
                if (hit.collider.gameObject == aj)
                {
                    ////////////////////////////////////////////////
                    // WRITE CODE HERE:
                    // implement deflection shooting
                    //deflection_shooting(aj, aj_centroid);
                    compute_shooting_direction(aj, turret_centroid, aj_centroid);
                    //shooting_direction = direction_from_turret_to_claire; // this is a very simple heuristic for shooting, replace it
                    ////////////////////////////////////////////////

                    float angle_to_rotate_turret = Mathf.Rad2Deg * Mathf.Atan2(shooting_direction.x, shooting_direction.z);
                    transform.eulerAngles = new Vector3(0.0f, angle_to_rotate_turret, 0.0f);
                    Vector3 current_turret_direction = new Vector3(Mathf.Sin(Mathf.Deg2Rad * transform.eulerAngles.y), 1.1f, Mathf.Cos(Mathf.Deg2Rad * transform.eulerAngles.y));
                    projectile_starting_pos = transform.position + 5.5f * current_turret_direction;  // estimated position of the turret's front of the cannon
                    aj_is_accessible = true;
                }
                else
                    aj_is_accessible = false;            
            }
        }
    }

    private IEnumerator Spawn()
    {
        while (true)
        {            
            if (aj_is_accessible)
            {
                GameObject new_object = Instantiate(projectile_template, projectile_starting_pos, Quaternion.identity);
                new_object.GetComponent<Apple>().direction = shooting_direction;
                new_object.GetComponent<Apple>().velocity = projectile_velocity;
                new_object.GetComponent<Apple>().birth_time = Time.time;
                new_object.GetComponent<Apple>().birth_turret = transform.gameObject;
            }
            yield return new WaitForSeconds(shooting_delay); // next shot will be shot after this delay
        }
    }

    private void compute_shooting_direction(GameObject aj, Vector3 turret_centroid, Vector3 aj_centroid)
    {
        Vector3 v = aj_centroid - turret_centroid;
        Vector3 d = v / v.magnitude;
        Vector3 new_pos = new Vector3(d.x * (projectile_velocity * Time.deltaTime), 0.0f, d.z * (projectile_velocity * Time.deltaTime));
        shooting_direction = new_pos.normalized;
    }

    private void deflection_shooting(GameObject aj, Vector3 aj_centroid)
    {
        Vector3 future_target_pos = aj_centroid;

        float delta_pos = Mathf.Infinity;
        int iterations = 0;
        while(delta_pos > 0.1f || iterations < 10000)
        {
            float distance = (future_target_pos - projectile_starting_pos).magnitude;
            float look_ahead_time = distance / projectile_velocity;
            Vector3 last_future_target_pos = future_target_pos;

            float aj_velocity = aj_component.velocity;
            Vector3 aj_movement_direction = aj_component.movement_direction;
            
            future_target_pos = aj_centroid + look_ahead_time * aj_velocity * aj_movement_direction;
            delta_pos = (future_target_pos - last_future_target_pos).magnitude;

            iterations++;
        }

        shooting_direction = (future_target_pos - projectile_starting_pos).normalized;
    }
}
