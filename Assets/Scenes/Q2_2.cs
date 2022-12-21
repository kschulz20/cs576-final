using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class Q2_2 : MonoBehaviour
{
    public InputField ans;
    public Text correctOrWrong;
    private string correctAns;
    public TextMeshProUGUI RandomQuestion;
    
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
        nxt.SetActive(true);
    }
    // Start is called before the first frame update
    void Start()
    {
        correctAns = "i love unity";
        //correctAns = DogName[Random.Range(0, 10)];
        //RandomQuestion.text = correctAns;
        GameObject game_manager_obj = GameObject.FindGameObjectWithTag("GameManager");
        manager = game_manager_obj.GetComponent<nextQuestion>();
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
