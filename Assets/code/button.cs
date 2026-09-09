using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class button : MonoBehaviour
{
    public int mouse_over;
    public int click;

    public int pos;
    public string description;

    void Awake(){
        mouse_over = 0;
        click = 0;
    }

    void Update(){
        click = 0;
        if (mouse_over == 1){
            if (Input.GetMouseButton(0)){
                click = 1;
                
            }
        }
    }

    void OnMouseEnter(){
        mouse_over = 1;
    }

    void OnMouseExit(){
        mouse_over= 0;
    }
}