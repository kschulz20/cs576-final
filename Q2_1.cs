using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class Q2_1 : MonoBehaviour
{
    public InputField ans;
    public Text correctOrWrong;
    private int y;
    private int loopTime;
    private int increase;
    public TextMeshProUGUI RandomQuestion;
    public TextMeshProUGUI RandomQuestion2;
    public TextMeshProUGUI RandomQuestion3;
    public GameObject nxt;
    public void submit()
    {


        if (ans.text == (y + loopTime * increase).ToString())
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
        y = Random.Range(0, 20);
        loopTime = Random.Range(5, 15);
        increase = Random.Range(5, 10);
        RandomQuestion.text = y.ToString();
        RandomQuestion2.text = loopTime.ToString();
        RandomQuestion3.text = increase.ToString();
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
