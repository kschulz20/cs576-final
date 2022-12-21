using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class StartGame : MonoBehaviour
{
    public Button start_button;
    // Start is called before the first frame update
    void Start()
    {
        start_button.onClick.AddListener(StartGameButton);
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    void StartGameButton()
    {
        SceneManager.LoadScene("Exploration 1");
    }
}
