using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class KillBossButton : MonoBehaviour
{
    public GameObject turret_to_destroy;

    //Track when player stepped on button
    void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.tag == "Player")
        {
            //Destroy the turret
            Object.Destroy(turret_to_destroy);
        }
    }
}
