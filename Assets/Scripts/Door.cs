using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class Door : MonoBehaviour
{

    private ExplorationManager exp_manager;
    private GameObject player;
    private float radius_notify_player = 5.0f;

    // Start is called before the first frame update
    void Start()
    {
        GameObject exp_manager_obj = GameObject.FindGameObjectWithTag("ExpManager");
        exp_manager = exp_manager_obj.GetComponent<ExplorationManager>();
        if (exp_manager == null)
        {
            Debug.LogError("Internal error: could not find the Exploration Manager object - did you remove its 'Exploration Manager' tag?");
            return;
        }
        player = GameObject.FindGameObjectWithTag("AJ");
        if (player == null)
        {
            Debug.LogError("Internal error: could not find the player object - did you remove its 'AJ' tag?");
            return;
        }
    }

    // Update is called once per frame
    void Update()
    {
        if (Vector3.Distance(transform.position, player.transform.position) <= radius_notify_player)
        {
            exp_manager.interact_box.SetActive(true);
        }
        bool show_interact_button = false;
        string near_object = "";
        List<string> needed_items = exp_manager.needed_items;
        if(needed_items.Count == 0)
        {
            if (Vector3.Distance(transform.position, player.transform.position) <= radius_notify_player)
            {
                exp_manager.interact_box.SetActive(true);
                if (Input.GetKey(KeyCode.E))
                {
                    SceneManager.LoadScene("SampleScene");
                }
            }
        }
    }
}

