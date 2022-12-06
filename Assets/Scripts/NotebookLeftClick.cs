using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class NotebookLeftClick : MonoBehaviour, IPointerClickHandler
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
        Debug.Log("Notebook left button clicked!");
    }
}
