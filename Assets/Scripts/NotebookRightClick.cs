using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class NotebookRightClick : MonoBehaviour, IPointerClickHandler
{
    private ExplorationManager exploration_manager;
    private GameObject notebook_text;
    private GameObject notebook_left_button;
    private GameObject notebook_right_button;
    private int notebook_page;
    private List<string> notebook_notes;

    void Start()
    {
        GameObject exp_manag_obj = GameObject.FindGameObjectWithTag("ExpManager");
        exploration_manager = exp_manag_obj.GetComponent<ExplorationManager>();
        if (exploration_manager == null)
        {
            Debug.LogError("Internal error: could not find the ExplorationManager object - did you remove its 'ExpManager' tag?");
            return;
        }
        notebook_text = exploration_manager.notebook_text;
        notebook_left_button = exploration_manager.notebook_left_button;
        notebook_right_button = exploration_manager.notebook_right_button;
        notebook_notes = exploration_manager.notebook_notes;
    }

    public void OnPointerClick(PointerEventData pointerEventData)
    {
        notebook_page = exploration_manager.notebook_page;
        notebook_page++;
        exploration_manager.UpdateNotebookPage(notebook_page);
        Debug.Log(notebook_page);
        notebook_right_button.SetActive(false);
        notebook_left_button.SetActive(true);
        string new_notebook_text = "";
        int counter = notebook_page * 3 - 2;
        for (int i = notebook_page * 3 - 3; i <= notebook_page * 3 - 1; i++)
        {
            if (i >= 0 && i < notebook_notes.Count)
            {
                new_notebook_text += counter.ToString() + ". " + exploration_manager.stage_one_item_to_info[exploration_manager.notebook_notes[i]] + "\n";
                counter++;
            }
        }
        notebook_text.GetComponent<Text>().text = new_notebook_text;
    }
}
