using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Respawn : MonoBehaviour
{
    public Transform player;
    public Transform respawn_point;
    public BossLevelOne level;
    void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.tag == "Player")
        {
            level.audio_source.PlayOneShot(level.lava_sizzle_sfx);
            player.GetComponent<CharacterController>().enabled = false;
            player.transform.position = respawn_point.transform.position;
            player.GetComponent<CharacterController>().enabled = true;
        }
    }
}
