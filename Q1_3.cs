using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class Q1_3 : MonoBehaviour
{
    public InputField ans;
    public Text correctOrWrong;
    private int x;
    private int increase;
    public TextMeshProUGUI RandomQuestion;
    public TextMeshProUGUI RandomQuestion2;
    public GameObject nxt;
    public void submit()
    {


        if (ans.text == (x + increase).ToString())
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
        x = Random.Range(0, 100);
        increase = Random.Range(0, 100);
        RandomQuestion.text = x.ToString();
        RandomQuestion2.text = increase.ToString();
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
