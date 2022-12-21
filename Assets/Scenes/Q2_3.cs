using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.SceneManagement;

public class Q2_3 : MonoBehaviour
{
    public InputField ans;
    public Text correctOrWrong;
    private string correctAns;
    public GameObject nxt;
    public nextQuestion manager;
    public void submit()
    {


        if (ans.text == correctAns)
        {
            correctOrWrong.text = "Correct!";
            manager.correct[4] = true;
        }
        else
        {
            correctOrWrong.text = "Wrong!";
        }
        gameObject.SetActive(false);
        ans.readOnly = true;
        for(int i = 0; i < 6; i++)
        {
            if(!manager.correct[i])
            {
                SceneManager.LoadScene("FinishLose");
            }
        }
        SceneManager.LoadScene("FinishWIn");
    }
    // Start is called before the first frame update
    void Start()
    {
        correctAns = 40.ToString();
        GameObject game_manager_obj = GameObject.FindGameObjectWithTag("GameManager");
        manager = game_manager_obj.GetComponent<nextQuestion>();
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}