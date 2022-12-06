using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ExplorationManager : MonoBehaviour
{

    public GameObject notebook_text;
    public GameObject notebook_left_button;
    public GameObject notebook_right_button;
    public GameObject to_do_text;
    public GameObject checklist_left_button;
    public GameObject checklist_right_button;
    public GameObject timer_text;

    internal int checklist_page = 1;
    internal int notebook_page = 1;
    internal List<string> needed_items = new List<string> { "Cellphone", "Briefcase", "Paper 1", "Laptop", "Talk to Professor Baime Pavila", "Paper 2" };      //Items needed for collection still
    internal List<string> notebook_notes = new List<string>();
    internal Dictionary<string, string> stage_one_item_to_info = new Dictionary<string, string>();     //Maps item name to corresponding info item gives
    internal Dictionary<string, string> stage_one_item_to_descrip = new Dictionary<string, string>();      //Maps item name to corresponding item description

    private int stage = 0;      //Stage identifier
    private float timer = 180.0f;     //Timer to indicate when exploration stage will end
    private List<string> inventory = new List<string>();     //Collected items so far
    private List<string> stage_one_items = new List<string> { "Cellphone", "Briefcase", "Paper 1", "Laptop", "Talk to Professor Baime Pavila", "Paper 2" };      //Required items for stage 1
    private List<string> stage_two_items = new List<string> { "Tablet", "Pen", "Textbook 1", "Talk to Professor Maniel Bheldon", "Laptop", "Textbook 2" };      //Required items for stage 2
    private List<string> stage_one_info = new List<string> { "Variables are used to store info in computer programs", "In Java, variables can be type integer (int)", "In Java, variables can be alphabetical characters (String)", "Variables can be updated and reassigned", "To add to an integer variable, do int variable += addedNum", "To print out a String in Java with a newline, do System.out.println(printedString);" };
    private List<string> stage_one_descrip = new List<string> { "Professor Rawrrington's cellphone. It contains some insights on what will be on the exam", 
                                                                "This pen is inscribed with some info about Java variables", 
                                                                "The paper is a research article about Java String variables. Definitely useful for the test!", 
                                                                "Professor Rawrrington's laptop that contains notes about the upcoming exam.", 
                                                                "Why hello there. Are you prepared for the upcoming CS121 exam? I taught the class before, just remember how to do operations with variables and you will do great! :)",
                                                                "A highly scientific paper about printing Strings in Java. This will be beyond useful to ace the test." };

    // Start is called before the first frame update
    void Start()
    {
        notebook_left_button.SetActive(false);
        notebook_right_button.SetActive(false);
        checklist_left_button.SetActive(false);
        checklist_right_button.SetActive(true);
        timer_text.GetComponent<Text>().text = "Time Left: " + timer.ToString("0.0");

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
        for(int i = 0; i < stage_one_items.Count; i++)
        {
            stage_one_item_to_info.Add(stage_one_items[i], stage_one_info[i]);
            stage_one_item_to_descrip.Add(stage_one_items[i], stage_one_descrip[i]);
        }
        to_do_text.GetComponent<Text>().text = to_do_string;
    }

    // Update is called once per frame
    void Update()
    {
        if(timer >= 0.0f)
        {
            timer -= Time.deltaTime;
            timer_text.GetComponent<Text>().text = "Time Left: " + timer.ToString("0.0");
        }
    }

    internal void UpdateCheckListPage(int new_page)
    {
        checklist_page = new_page;
    }
}
