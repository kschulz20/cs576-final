using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class ChecklistRightClick : MonoBehaviour, IPointerClickHandler
{

    private ExplorationManager exploration_manager;
    private GameObject to_do_text;
    private GameObject checklist_left_button;
    private GameObject checklist_right_button;
    private int checklist_page;
    private List<string> needed_items;

    void Start()
    {
        GameObject exp_manag_obj = GameObject.FindGameObjectWithTag("ExpManager");
        exploration_manager = exp_manag_obj.GetComponent<ExplorationManager>();
        if (exploration_manager == null)
        {
            Debug.LogError("Internal error: could not find the ExplorationManager object - did you remove its 'ExpManager' tag?");
            return;
        }
        to_do_text = exploration_manager.to_do_text;
        checklist_left_button = exploration_manager.checklist_left_button;
        checklist_right_button = exploration_manager.checklist_right_button;
        needed_items = exploration_manager.needed_items;
    }

    public void OnPointerClick(PointerEventData pointerEventData)
    {
        checklist_page = exploration_manager.checklist_page;
        checklist_page++;
        exploration_manager.UpdateCheckListPage(checklist_page);
        Debug.Log(checklist_page);
        checklist_right_button.SetActive(false);
        checklist_left_button.SetActive(true);
        string to_do_string = "";
        int to_do_counter = checklist_page * 3 - 2;
        for (int i = checklist_page * 3 - 3; i <= checklist_page * 3 - 1; i++)
        {
            if (i >= 0 && i < needed_items.Count)
            {
                to_do_string += to_do_counter.ToString() + ". " + needed_items[i] + "\n";
                to_do_counter++;
            }
        }
        to_do_text.GetComponent<Text>().text = to_do_string;
    }
}
