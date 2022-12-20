using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Item : MonoBehaviour
{

    private ExplorationManager exp_manager;
    private GameObject player;
    private float radius_notify_player = 5.0f;
    private Dictionary<string, bool> is_near;

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
        is_near = exp_manager.stage_one_item_near;
    }

    // Update is called once per frame
    void Update()
    {
        bool show_interact_button = false;
        string near_object = "";
        if (Vector3.Distance(transform.position, player.transform.position) <= radius_notify_player)
        {
            is_near[gameObject.name] = true;
        }
        else
        {
            is_near[gameObject.name] = false;
            exp_manager.interact_box.SetActive(false);
        }
        foreach(KeyValuePair<string, bool> entry in is_near)
        {
            if(entry.Value)
            {
                near_object = entry.Key;
                if(!exp_manager.prof_talked_to.Contains(near_object))
                {
                    Debug.Log(exp_manager.prof_talked_to.Count);
                    show_interact_button = true;
                }
            }
        }
        if(show_interact_button)
        {
            exp_manager.interact_box.SetActive(true);
            if (Input.GetKey(KeyCode.E))
            {
                exp_manager.dialogue_text.GetComponent<Text>().text = exp_manager.stage_one_item_to_descrip[near_object];
                RemoveItemFromNeeded(near_object);
                AddItemToCollected(near_object);
                ReconstructChecklist();
                ReconstructNotebook();
                foreach (GameObject go in UnityEngine.Object.FindObjectsOfType(typeof(GameObject)))
                {
                    if (go.name == near_object)
                    {
                        if(go.tag == "Professor")
                        {
                            exp_manager.prof_talked_to.Add(near_object);
                        }
                        else
                        {
                            go.SetActive(false);
                        }
                    }
                }
                is_near[near_object] = false;
                exp_manager.interact_box.SetActive(false);
            }
        }
        else
        {
            exp_manager.interact_box.SetActive(false);
        }

        if(gameObject.tag == "Professor")
        {
            Vector3 targetPostition = new Vector3(player.transform.position.x,
                                        this.transform.position.y,
                                        player.transform.position.z);
            this.transform.LookAt(targetPostition);
        }
    }

    void RemoveItemFromNeeded(string item)
    {
        int needed_items_count = exp_manager.needed_items.Count;
        for(int i = 0; i < needed_items_count; i++)
        {
            if(exp_manager.needed_items[i] == item)
            {
                exp_manager.needed_items.RemoveAt(i);
                break;
            }
        }
    }

    void AddItemToCollected(string item)
    {
        exp_manager.notebook_notes.Add(item);
    }

    void ReconstructChecklist()
    {
        string new_checklist_text = "";
        int rank = 1;
        if(exp_manager.needed_items.Count <= 5)
        {
            exp_manager.checklist_page = 1;
            exp_manager.checklist_left_button.SetActive(false);
            exp_manager.checklist_right_button.SetActive(false);
        }
        int page = exp_manager.checklist_page;
        for (int i = page * 5 - 5; i <= page * 5 - 1; i++)
        {
            if (i >= 0 && i < exp_manager.needed_items.Count)
            {
                new_checklist_text += rank.ToString() + ". " + exp_manager.needed_items[i] + "\n";
                rank++;
            }
        }
        exp_manager.to_do_text.GetComponent<Text>().text = new_checklist_text;
    }

    void ReconstructNotebook()
    {
        string new_notebook_text = "";
        int page = exp_manager.notebook_page;
        int counter = page * 5 - 4;
        if (exp_manager.notebook_notes.Count > 5 && page == 1)
        {
            exp_manager.notebook_left_button.SetActive(false);
            exp_manager.notebook_right_button.SetActive(true);
        }
        for (int i = page * 5 - 5; i <= page * 5 - 1; i++)
        {
            if (i >= 0 && i < exp_manager.notebook_notes.Count)
            {
                new_notebook_text += counter.ToString() + ". " + exp_manager.stage_one_item_to_info[exp_manager.notebook_notes[i]] + "\n";
                counter++;
            }
        }
        exp_manager.notebook_text.GetComponent<Text>().text = new_notebook_text;
    }
}
