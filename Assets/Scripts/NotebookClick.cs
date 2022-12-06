using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class NotebookClick : MonoBehaviour, IPointerClickHandler
{

    private bool up = false;
    private bool clicked = false;

    void Update()
    {
        if(clicked)
        {
            if (up)
            {
                Vector3 notebook_pos = transform.position;
                notebook_pos.y += 1.0f;
                if (notebook_pos.y <= 100)
                {
                    transform.position = new Vector3(notebook_pos.x, notebook_pos.y, notebook_pos.z);
                }
                else
                {
                    clicked = false;
                }
            }
            else
            {
                Vector3 notebook_pos = transform.position;
                Debug.Log(notebook_pos);
                notebook_pos.y -= 1.0f;
                if (notebook_pos.y >= -46)
                {
                    transform.position = new Vector3(notebook_pos.x, notebook_pos.y, notebook_pos.z);
                }
                else
                {
                    clicked = false;
                }
            }
        }
    }

    public void OnPointerClick(PointerEventData pointerEventData)
    {
        up = !up;
        clicked = true;
    }
}
