using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
//using UnityEngine.UI.InputField ChatBox;

public class Q1_1 : MonoBehaviour
{
    public InputField ans;
    public Text correctOrWrong;
    private int correctAns;
    public TextMeshProUGUI RandomQuestion;
    public GameObject nxt;
    public nextQuestion manager;

    public void submit()
    {
        
        
        if(ans.text == correctAns.ToString())
        {
            correctOrWrong.text = "Correct!";
            manager.correct[0] = true;
        }
        else
        {
            correctOrWrong.text = "Wrong!";
        }
        gameObject.SetActive(false);
        ans.readOnly = true;
        nxt.SetActive(true);
    }
    
    // Start is called before the first frame update
    void Start()
    {
        correctAns = Random.Range(0, 100);
        RandomQuestion.text = correctAns.ToString();

        GameObject game_manager_obj = GameObject.FindGameObjectWithTag("GameManager");
        manager = game_manager_obj.GetComponent<nextQuestion>();
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
