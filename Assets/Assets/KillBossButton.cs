using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class KillBossButton : MonoBehaviour
{
    public GameObject turret_to_destroy;
    public BossLevelOne level;

    //Track when player stepped on button
    void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.tag == "Player")
        {
            level.audio_source.PlayOneShot(level.turret_death_sfx);
            //Destroy the turret
            Object.Destroy(turret_to_destroy);
        }
    }
}
