using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ChangeColor : MonoBehaviour
{
    float duration = 1.5f;
    private float t = 0;
    public bool change_color;

    void Start()
    {
        change_color = false;
    }

    void Update()
    {
        ColorChangerr();
    }

    void ColorChangerr()
    {

        GetComponent<Renderer>().material.color = Color.Lerp(Color.green, Color.red, t);

        if (t < 1){ 
            t += Time.deltaTime/duration;
        } 
    }
}
