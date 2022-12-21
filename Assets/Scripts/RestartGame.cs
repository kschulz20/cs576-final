using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class RestartGame : MonoBehaviour
{
    public Button restart_button;
    // Start is called before the first frame update
    void Start()
    {
        restart_button.onClick.AddListener(RestartGameButton);
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    void RestartGameButton()
    {
        SceneManager.LoadScene("Main");
    }
}
