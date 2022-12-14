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
    public void submit()
    {


        if (ans.text == correctAns)
        {
            correctOrWrong.text = "Correct!";
        }
        else
        {
            correctOrWrong.text = "Wrong!";
        }
        nxt.SetActive(true);
    }
    // Start is called before the first frame update
    void Start()
    {
        correctAns = "hi";
        //correctAns = DogName[Random.Range(0, 10)];
        //RandomQuestion.text = correctAns;
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
