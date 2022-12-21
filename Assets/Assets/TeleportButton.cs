using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class TeleportButton : MonoBehaviour
{
    public GameObject player;
    public GameObject respawn_point;
    public BossLevelOne level;

    //Track when player stepped on button
    void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.tag == "Player")
        {
            level.audio_source.PlayOneShot(level.teleport_sfx, 0.7f);
            //Move player to the respawn point
            player.GetComponent<CharacterController>().enabled = false;
            SceneManager.LoadScene("TestQuestionScene");
        }
    }
}
