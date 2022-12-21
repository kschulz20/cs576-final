using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.SceneManagement;

public class nextQuestion : MonoBehaviour
{
    public GameObject currentQuestion;
    public GameObject nxtQuestion;
    public bool[] correct = { false, false, false, false, false, false };
    private float timer;
    
    public void next()
    {
        currentQuestion.SetActive(false);
        nxtQuestion.SetActive(true);

    }
    // Start is called before the first frame update
    void Start()
    {
    }

    // Update is called once per frame
    void Update()
    {

    }
}
