using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TeleportButton : MonoBehaviour
{
    public GameObject player;
    public GameObject respawn_point;

    //Track when player stepped on button
    void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.tag == "Player")
        {
            //Move player to the respawn point
            player.GetComponent<CharacterController>().enabled = false;
            player.transform.position = respawn_point.transform.position;
            player.GetComponent<CharacterController>().enabled = true;
        }
    }
}
