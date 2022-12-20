using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Respawn : MonoBehaviour
{
    public Transform player;
    public Transform respawn_point;
    void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.tag == "Player")
        {
            player.GetComponent<CharacterController>().enabled = false;
            player.transform.position = respawn_point.transform.position;
            player.GetComponent<CharacterController>().enabled = true;
        }
    }
}
