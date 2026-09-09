using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class menu : MonoBehaviour
{

    public Camera cam;

    public button[] menu_select;
    public GameObject[] menus;

    public button[] list_o_buttons;
    public button[] scenario_buttons;
    public GameObject[] scenarios_list;

    public int liscence_level;

    public button fuel_slider;
    public GameObject fuel_background;
    public button battery_slider;
    public GameObject battery_background;
    public button ammo_slider;
    public GameObject ammo_background;

    int menu_screen;

    public bool player_click_battery;
    public bool player_click_fuel;
    public bool player_click_ammo;

    public float fuel_perc = 40f;
    public float battery_perc = 35f;
    public float ammo_perc = 0f;

    public float total_percent = 75;

    public GameObject selected_weapon;

    public TMP_Text fuel_show;
    public TMP_Text battery_show;
    public TMP_Text ammo_show;
    public TMP_Text total_percent_show;
    public TMP_Text weight;

    public bool phase;

    int decide_scenario = -1;
    public int scenario_level_select = 1;
    public button play_button;
    public TMP_Text scenario_desc;

    void Update(){
        if(Input.GetKeyDown(KeyCode.P)){
            // quit
            Debug.Log("break");
            Application.Quit();
        }
        if(phase == false){
            if(Screen.lockCursor == true){
                Screen.lockCursor = false;
            }
            for(int I = 0; I < menu_select.Length; I ++){
                if(menu_select[I].click == 1){
                    menu_screen = I;
                    I ++ ;
                }
            }
            for(int I = 0; I < menu_select.Length; I ++){
                if(I == menu_screen){
                    menus[I].SetActive(true);
                }
                else{
                    menus[I].SetActive(false);
                }
            }
            if(menu_screen == 0){ // introduction
            }
            if(menu_screen == 1){ // scenarios
                scenarios();
            }
            if(menu_screen == 2){ // controls
            }
            if(menu_screen == 3){ // garage
                garage();
            }
        }
        else{
            if(Input.GetKeyDown(KeyCode.Escape) || GameObject.Find("player") == null){
                // return 
                transform.GetChild(0).gameObject.SetActive(true);
                transform.GetChild(1).gameObject.SetActive(true);
                phase = false;
                for(int I = 0; I < list_o_buttons.Length; I++){
                    list_o_buttons[I].click = 0;
                    list_o_buttons[I].mouse_over = 0;
                }
            }
        }
    }

    void Awake(){
        list_o_buttons = transform.GetChild(0).transform.GetChild(1).transform.GetChild(1).gameObject.GetComponentsInChildren<button>();
    }

    void garage(){
        // slider shit
            if(fuel_slider.click == 1){
                player_click_fuel = true;
            }
            if(battery_slider.click == 1){
                player_click_battery = true;
            }
            if(ammo_slider.click == 1){
                player_click_ammo = true;
            }
            if(Input.GetMouseButtonUp(0)){
                player_click_battery = false;
                player_click_fuel = false;
                player_click_ammo = false;
            }
            if(player_click_fuel){
                float rot_x = Input.GetAxis("Mouse X") * 30;
                if(fuel_slider.transform.localPosition.x <= 1 && fuel_slider.transform.localPosition.x >= 0){
                    fuel_slider.transform.position += new Vector3(rot_x,0,0);
                    fuel_slider.transform.localPosition = new Vector3(fuel_slider.transform.localPosition.x - fuel_slider.transform.localPosition.x % 0.01f,0,0);
                }
                else{
                    if(fuel_slider.transform.localPosition.x > 1){
                        fuel_slider.transform.localPosition = new Vector3(0.99f,0,0);
                    }
                    if(fuel_slider.transform.localPosition.x < 0){
                        fuel_slider.transform.localPosition = new Vector3(0.01f,0,0);
                    }
                }
                fuel_perc = fuel_slider.transform.localPosition.x * 100;
                total_percent = battery_perc + fuel_perc + ammo_perc;
                if(total_percent > 100){
                    float cream = total_percent % 100;
                    if(battery_slider.transform.localPosition.x >= ammo_slider.transform.localPosition.x){
                        cream = (battery_perc - cream) / 100;
                        battery_slider.transform.localPosition = new Vector3(cream,0,0);
                    }
                    else{
                        cream = (ammo_perc - cream) / 100;
                        ammo_slider.transform.localPosition = new Vector3(cream,0,0);
                    }
                }
                
            }
            if(player_click_battery){
                float rot_x = Input.GetAxis("Mouse X") * 30;
                if(battery_slider.transform.localPosition.x <= 1 && battery_slider.transform.localPosition.x >= 0){
                    battery_slider.transform.position += new Vector3(rot_x,0,0);
                    battery_slider.transform.localPosition = new Vector3(battery_slider.transform.localPosition.x - battery_slider.transform.localPosition.x % 0.01f,0,0);
                }
                else{
                    if(battery_slider.transform.localPosition.x > 1){
                        battery_slider.transform.localPosition = new Vector3(0.99f,0,0);
                    }
                    if(battery_slider.transform.localPosition.x < 0){
                        battery_slider.transform.localPosition = new Vector3(0.01f,0,0);
                    }
                }
                battery_perc = battery_slider.transform.localPosition.x * 100;
                total_percent = battery_perc + fuel_perc + ammo_perc;
                if(total_percent > 100){
                    float cream = total_percent % 100;
                    if(fuel_slider.transform.localPosition.x >= ammo_slider.transform.localPosition.x){
                        cream = (fuel_perc - cream) / 100;
                        fuel_slider.transform.localPosition = new Vector3(cream,0,0);
                    }
                    else{
                        cream = (ammo_perc - cream) / 100;
                        ammo_slider.transform.localPosition = new Vector3(cream,0,0);
                    }
                }
                
            }
            if(player_click_ammo){
                float rot_x = Input.GetAxis("Mouse X") * 30;
                if(ammo_slider.transform.localPosition.x <= 1 && ammo_slider.transform.localPosition.x >= 0){
                    ammo_slider.transform.position += new Vector3(rot_x,0,0);
                    ammo_slider.transform.localPosition = new Vector3(ammo_slider.transform.localPosition.x - ammo_slider.transform.localPosition.x % 0.01f,0,0);
                }
                else{
                    if(ammo_slider.transform.localPosition.x > 1){
                        ammo_slider.transform.localPosition = new Vector3(0.99f,0,0);
                    }
                    if(ammo_slider.transform.localPosition.x < 0){
                        ammo_slider.transform.localPosition = new Vector3(0.01f,0,0);
                    }
                }
                ammo_perc = ammo_slider.transform.localPosition.x * 100;
                total_percent = ammo_perc + fuel_perc + battery_perc;
                if(total_percent > 100){
                    float cream = total_percent % 100;
                    cream = (fuel_perc + battery_perc - cream) / 100;
                    if(fuel_slider.transform.localPosition.x >= battery_slider.transform.localPosition.x){
                        cream = (fuel_perc - cream) / 100;
                        fuel_slider.transform.localPosition = new Vector3(cream,0,0);
                    }
                    else{
                        cream = (battery_perc - cream) / 100;
                        battery_slider.transform.localPosition = new Vector3(cream,0,0);
                    }
                }
                
            }
            battery_perc = battery_slider.transform.localPosition.x * 100;
            fuel_perc = fuel_slider.transform.localPosition.x * 100;
            ammo_perc = ammo_slider.transform.localPosition.x * 100;
            total_percent = battery_perc + fuel_perc + ammo_perc;
            
            fuel_background.transform.localScale = new Vector3(fuel_perc / 100,1,1);
            battery_background.transform.localScale = new Vector3(battery_perc / 100,1,1);
            ammo_background.transform.localScale = new Vector3(ammo_perc / 100,1,1);
        // slider shit
        fuel_show.text = (fuel_perc * 1000).ToString();
        battery_show.text = (battery_perc * 120).ToString();
        ammo_show.text = (ammo_perc * (selected_weapon.GetComponent<weapon_script>().max_ammo / 100)).ToString();
        total_percent_show.text = total_percent.ToString();
        weight.text = (fuel_perc * 100 + battery_perc * 60 + ammo_perc * (selected_weapon.GetComponent<weapon_script>().max_ammo / 100) * (selected_weapon.GetComponent<weapon_script>().weight_per_bullet)).ToString();
    }

    void scenarios(){
        for(int I = 0; I < scenario_buttons.Length; I++){
            if((I + 1 <= liscence_level && scenario_buttons[I].click == 1) || I + 1 == scenario_level_select){
                scenario_level_select = I + 1;
                scenario_buttons[I].click = 0;
                scenario_buttons[I].mouse_over = 0;
                if(GameObject.Find("lv " + (I + 1).ToString()) != null){
                    GameObject.Find("lv " + (I + 1).ToString()).transform.GetChild(0).gameObject.SetActive(true);
                }
            }
            else{
                if(GameObject.Find("lv " + (I + 1).ToString()) != null){
                    GameObject.Find("lv " + (I + 1).ToString()).transform.GetChild(0).gameObject.SetActive(false);
                }
                
            }
        }
        for(int I = 0; I < list_o_buttons.Length; I++){
            if(list_o_buttons[I].click == 1){
                decide_scenario = list_o_buttons[I].pos;
                scenario_desc.text = list_o_buttons[I].description;
            }
        }
        if(play_button.click == 1 && decide_scenario != -1){
            play_button.click = 0;
            play_button.mouse_over = 0;
            phase = true;
            transform.GetChild(0).gameObject.SetActive(false);
            transform.GetChild(1).gameObject.SetActive(false);
            Instantiate(scenarios_list[decide_scenario]);
        }
    }
}
