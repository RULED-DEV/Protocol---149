using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class ship_script : MonoBehaviour
{
    public float nav_scale;
    public float sensitivity;
    float rot_x;
    float rot_Y;

    float sprint;
    public bool active;
    public float max_velocity;
    public float acceleration_speed;
    public float rot_vel;
    public float ship_rotate_speed;
    float rot_vel_app;

    float fuel_max;
    public float fuel;
    public float battery_max;
    public float battery;
    public float overhead;

    public GameObject cam;
    public GameObject cont_point;
    public GameObject contact_pointer;
    public GameObject contact_indicator;
    public GameObject[] UI_element;
    public GameObject weapon_slot;
    public GameObject weapon_compass;
    weapon_script weapon;
    public GameObject locked_target;
    public TMP_Text ammo_count;

    Rigidbody RB; 

    bool move;
    float fire_timer;
    public float ammo;

    public missile[] contact = new missile[1];
    public Material[] material_list = new Material[1];

    bool target_lock = true;

    public AudioSource source_thrust;
    public AudioSource source_gun;
    public AudioSource ambient_source;
    public AudioClip target_ping;
    
    int thrusters;

    void Awake(){
        Screen.lockCursor = true;
        RB = gameObject.GetComponent<Rigidbody>();
        menu menu = GameObject.Find("start menu").GetComponent<menu>();
        weapon = Instantiate(menu.selected_weapon,weapon_slot.transform.position,transform.rotation).GetComponent<weapon_script>();
        ammo = menu.ammo_perc * (weapon.max_ammo / 100);
        weapon.transform.parent = weapon_slot.transform;
        fuel_max = menu.fuel_perc * 1000;
        battery_max = menu.battery_perc * 120;
        RB.mass = (menu.fuel_perc * 100 + menu.battery_perc * 60 + ammo * weapon.weight_per_bullet) / 6000;
        fuel_max = fuel;
        gameObject.name = "player";
    }

    void Update(){
        overhead = 0;
        if(active){
            if(ambient_source.isPlaying == false){
                ambient_source.Play();
            }
        }
        else{
            ambient_source.Stop();
        }
        cam_control();
        //jiggle_phys_cam_control();
        player_cont();
        navigator();
        battery_cont();
        weapons();
        if(Input.GetKeyDown(KeyCode.P)){
            // quit
            Debug.Log("break");
            Application.Quit();
        }
        UI();
    }

    void weapons(){
        if(Input.GetKeyDown(KeyCode.R)){
            // target lock
            if(target_lock){
                target_lock = false;
            }
            else{
                target_lock = true;
            }
        }
        weapon = weapon_slot.transform.GetChild(0).GetComponent<weapon_script>();
        if(target_lock){
            for(int I = 0; I < contact.Length; I ++){ // targeting
                if(locked_target != null){
                    weapon_compass.transform.forward = contact[I].transform.position - weapon_compass.transform.position;
                    float angleA = Vector3.Distance(weapon_compass.transform.eulerAngles,cam.transform.eulerAngles);
                    weapon_compass.transform.forward = locked_target.transform.position - weapon_compass.transform.position;
                    float angleB = Vector3.Distance(weapon_compass.transform.eulerAngles,cam.transform.eulerAngles);
                    if(angleA < angleB && contact[I].fuel > 0){
                        locked_target.GetComponent<missile>().lock_icon.SetActive(false);
                        locked_target = contact[I].gameObject;
                        locked_target.GetComponent<missile>().lock_icon.SetActive(true);
                    }
                }
                else{
                    locked_target = contact[I].gameObject;
                    locked_target.GetComponent<missile>().lock_icon.SetActive(true);
                }
            }
        } 
        // UI for targeting
            if(locked_target != null){
                GameObject icon = locked_target.GetComponent<missile>().lock_icon;
                if(target_lock){
                    icon.transform.GetChild(0).gameObject.SetActive(false);
                }
                else{
                    if(locked_target.GetComponent<missile>().fuel < 0 && locked_target.GetComponent<missile>().HP <= 0){
                        icon.SetActive(false);
                    }
                    icon.transform.GetChild(0).gameObject.SetActive(true);
                }
                icon.transform.forward = transform.position - icon.transform.position;
                icon.transform.localScale = Vector3.one *  (1 + Vector3.Distance(icon.transform.position,transform.position) * 0.03f);
            }
        // UI for targeting
        if(Input.GetMouseButton(0)){
            if(fire_timer <= Time.time && ammo != 0){
                fire_timer = Time.time + weapon.fire_interval;
                projectile proj = Instantiate(weapon.projectile,weapon.transform.position,weapon.transform.rotation).GetComponent<projectile>();
                if(locked_target != null){
                    proj.transform.forward = locked_target.transform.position - transform.position;
                    if(Vector2.Distance(weapon.transform.eulerAngles,proj.transform.eulerAngles) > weapon.rot_angle){
                        proj.transform.forward = transform.forward;
                    }
                    proj.target = locked_target;
                }
                proj.enabled = true;
                proj.GetComponent<Rigidbody>().velocity = RB.velocity;
                proj.weapon = weapon;
                proj.launch();
                RB.AddForce(-weapon_slot.transform.forward * weapon.knockback,ForceMode.Impulse); // recoil
                ammo-- ;
                source_gun.PlayOneShot(weapon.fire_noise);
            }
        }
    }

    void UI(){
        GameObject obj;
        if(Input.GetKeyDown(KeyCode.Alpha1)){ // lock on
            obj = locked_target.GetComponent<missile>().lock_icon;
            if(obj.activeInHierarchy){
                obj.SetActive(false);
            }   
            else{
                obj.SetActive(true);
            }
        }
        if(Input.GetKeyDown(KeyCode.Alpha2)){ // UI
            obj = UI_element[0].transform.parent.parent.gameObject;
            if(obj.activeInHierarchy){
                obj.SetActive(false);
            }   
            else{
                obj.SetActive(true);
            }
        }
        if(Input.GetKeyDown(KeyCode.Alpha3)){ // compass
            obj = contact_pointer.transform.parent.gameObject;
            if(obj.activeInHierarchy){
                obj.SetActive(false);
            }   
            else{
                obj.SetActive(true);
            }
        }
        // generator
            if(overhead > 100){
                overhead = 100;
            }
            float percent = overhead / 100;
            UI_element[0].transform.localPosition = Vector3.Lerp(UI_element[0].transform.localPosition,new Vector3(percent,0,0),1 * Time.deltaTime);
            UI_element[1].transform.localScale = Vector3.Lerp(UI_element[1].transform.localScale,new Vector3(percent,1,1),1 * Time.deltaTime);
            Color hold = UI_element[1].GetComponent<SpriteRenderer>().color;
            hold.r = Mathf.Lerp(hold.r,percent,Time.deltaTime);
            UI_element[1].GetComponent<SpriteRenderer>().color = hold;
        
        // battery
            percent = battery / battery_max;
            UI_element[2].transform.localScale = Vector3.Lerp(UI_element[2].transform.localScale,new Vector3(percent,1,1),1 * Time.deltaTime);
            hold = UI_element[2].GetComponent<SpriteRenderer>().color;
            hold.r = Mathf.Lerp(hold.r,percent,Time.deltaTime);
            UI_element[2].GetComponent<SpriteRenderer>().color = hold;
        
        // fuel
            percent = fuel / fuel_max;
            UI_element[3].transform.localScale = Vector3.Lerp(UI_element[3].transform.localScale,new Vector3(percent,1,1),1 * Time.deltaTime);
            hold = UI_element[3].GetComponent<SpriteRenderer>().color;
            hold.r = Mathf.Lerp(hold.r,percent,Time.deltaTime);
            UI_element[3].GetComponent<SpriteRenderer>().color = hold;
        // weapons
            ammo_count.text = ammo.ToString();
            if(ammo == 0){
                ammo_count.text = "OUT";
            }
    }

    void battery_cont(){
        if(battery <= battery_max){
            if((100 - overhead) > 0){
                battery += (100 - overhead);
            }
        }
        if(battery >= battery_max){
            battery = battery_max;
        }
    }
    
    void player_cont(){
        if (active && fuel > 0){
            thrusters = 0;
            
            move = false;
            sprint = 0;
            if (Input.GetKey(KeyCode.LeftShift) && battery > battery_max * 0.05){
                sprint = 1.5f;
            }
            if(Input.GetKey(KeyCode.A)){ // movement
                // if (Vector3.Magnitude(RB.velocity + -cont_point.transform.right) < max_velocity * (1 + sprint)){
                    overhead += 15 * (1 + sprint);
                    if(battery >= 15 * (1 + sprint) && fuel > 1 * (1 + sprint)){
                        fuel -= 1 * (1 + sprint);
                        battery -= 15;
                        RB.AddForce(-cont_point.transform.right * acceleration_speed * (1 + sprint) * 0.5f);
                        move = true;
                        thrusters++ ;
                    }
                // }
            }
            if(Input.GetKey(KeyCode.S)){
                // if (Vector3.Magnitude(RB.velocity + -cont_point.transform.forward) < max_velocity * (1 + sprint)){
                    overhead += 10 *  (1 + sprint);
                    if(battery >= 15 * (1 + sprint) && fuel > 1 * (1 + sprint)){
                        fuel -= 1 * (1 + sprint);
                        battery -= 10 * (1 + sprint);
                        RB.AddForce(-cont_point.transform.forward * acceleration_speed * (1 + sprint) * 0.3f);
                        move = true;
                        thrusters++ ;
                    }
                // }
            }
            if(Input.GetKey(KeyCode.D)){
                // if (Vector3.Magnitude(RB.velocity + cont_point.transform.right) < max_velocity * (1 + sprint)){
                    if(battery >= 15 * (1 + sprint)  || overhead + 15 * (1 + sprint) < 100 && fuel > 1 * (1 + sprint)){
                        overhead += 15 *  (1 + sprint);
                        fuel -= 1 * (1 + sprint);
                        battery -= 15 * (1 + sprint);
                        RB.AddForce(cont_point.transform.right * acceleration_speed * (1 + sprint) * 0.5f);
                        move = true;
                        thrusters++ ;
                    }
                // }
            }
            if(Input.GetKey(KeyCode.W)){
                //if (Vector3.Magnitude(RB.velocity + cont_point.transform.forward) < max_velocity * (1 + sprint)){
                    if(battery >= 25 * (1 + sprint)  || overhead + 25 * (1 + sprint) < 100 && fuel > 1 * (1 + sprint)){
                        overhead += 25 *  (1 + sprint);
                        fuel -= 1 * (1 + sprint);
                        battery -= 25 * (1 + sprint);
                        RB.AddForce(cont_point.transform.forward * acceleration_speed * (1 + sprint) * 1.6f);
                        move = true;
                        thrusters++ ;
                    }
                //}
            }
            if(Input.GetKey(KeyCode.Q) && RB.angularDrag == 0.05f){
                // roll right
                overhead += 8 *  (1 + sprint);
                if(battery >= 8 * (1 + sprint) && fuel > 1 * (1 + sprint)){
                    fuel -= 1 * (1 + sprint);
                    battery -= 8 * (1 + sprint);
                    rot_vel_app += rot_vel * (1 + sprint);
                }
            }
            if(Input.GetKey(KeyCode.E) && RB.angularDrag == 0.05f){
                // roll left
                overhead += 8 *  (1 + sprint);
                if(battery >= 8 * (1 + sprint) && fuel > 1 * (1 + sprint)){
                    fuel -= 1 * (1 + sprint);
                    battery -= 8 * (1 + sprint);
                    rot_vel_app -= rot_vel * (1 + sprint);
                }
            } 
            if(Input.GetKey(KeyCode.Space)){
                // if (Vector3.Magnitude(RB.velocity + cont_point.transform.up) < max_velocity * (1 + sprint)){
                    overhead += 10 *  (1 + sprint);
                    if(battery >= 15 * (1 + sprint) && fuel > 1 * (1 + sprint)){
                        fuel -= 1 * (1 + sprint);
                        battery -= 10 * (1 + sprint);
                        RB.AddForce(cont_point.transform.up * acceleration_speed * (1 + sprint));
                        thrusters++ ;
                        move = true;
                    }
                // }
            }
            if(Input.GetKey(KeyCode.LeftControl)){
                // if (Vector3.Magnitude(RB.velocity + -cont_point.transform.up) < max_velocity * (1 + sprint)){
                    overhead += 10 *  (1 + sprint);
                    if(battery >= 15 * (1 + sprint) && fuel > 1 * (1 + sprint)){
                        fuel -= 1 * (1 + sprint);
                        battery -= 10 * (1 + sprint);
                        RB.AddForce(-cont_point.transform.up * acceleration_speed * (1 + sprint));
                        thrusters++ ;
                        move = true;
                    }
                // }
            }
            if(Input.GetKey(KeyCode.B)){
                // brake
                overhead += 10 *  (1 + sprint);
                if(battery >= 15 * (1 + sprint) && fuel > 1 * (1 + sprint)){
                    battery -= 10;
                    fuel -= 1 * (1 + sprint);
                    RB.AddForce(-RB.velocity * 0.6f);
                    RB.angularDrag = 5;
                    rot_vel_app = rot_vel_app / 1.5f;
                }
            }
            else{
                RB.drag = 0.05f;
                RB.angularDrag = 0.05f;
                rot_vel_app = rot_vel_app / 1.0005f;
            }
        }
            if(rot_vel_app > 7){
                rot_vel_app = 7;
            }
            if(rot_vel_app < -7){
                rot_vel_app = -7;
            }
            transform.eulerAngles = new Vector3(transform.eulerAngles.x,transform.eulerAngles.y,transform.eulerAngles.z + rot_vel_app);
        // audio
        source_thrust.volume = (0.6f + (thrusters * 0.1f) * (sprint + 1 * 0.1f));
        if(move){
            if(source_thrust.isPlaying == false){
                source_thrust.Play();
            }
        }
        else{
            source_thrust.Stop();
        }
    }

    void cam_control(){
        rot_x = Input.GetAxis("Mouse X") * sensitivity;
        rot_Y = Input.GetAxis("Mouse Y") * -sensitivity;
        
        if(cam.transform.localEulerAngles.y < 130 || cam.transform.localEulerAngles.y > 230 || cam.transform.localEulerAngles.x < 75 || cam.transform.localEulerAngles.y > 230){
            
        }
        else{
            rot_x = rot_x / 4;
            rot_Y = rot_Y / 4;
        }
        cam.transform.Rotate(rot_Y,rot_x,0,Space.Self);
        cam.transform.rotation = Quaternion.Lerp(cam.transform.rotation, cont_point.transform.rotation, ship_rotate_speed * Time.deltaTime);

        // rotate ship toward camera
        cont_point.transform.rotation = Quaternion.Lerp(cont_point.transform.rotation, cam.transform.rotation, ship_rotate_speed * Time.deltaTime);
    }

    void navigator(){
        contact = gameObject.transform.parent.gameObject.GetComponentsInChildren<missile>();
        for(int I = 0; I < contact.Length; I++){
            if(contact[I] != null && contact[I].fuel > 0){
                if(contact[I].contact_indicator == null){ // creates contact indicator
                    contact[I].contact_indicator = Instantiate(contact_indicator,contact_pointer.transform.position,transform.rotation);
                    contact[I].contact_indicator.transform.parent = contact_pointer.transform.parent;
                }
                // puts contact on the surface of the nav ball
                contact_pointer.transform.forward = contact_pointer.transform.position - contact[I].transform.position;
                contact[I].contact_indicator.transform.localPosition = Vector3.zero;
                contact[I].contact_indicator.transform.Translate(-contact_pointer.transform.forward * 0.5f,Space.World); 
                
                // sets red line
                GameObject point = contact[I].contact_indicator.transform.GetChild(0).gameObject;
                point.transform.localPosition = Vector3.zero;
                point.transform.forward = contact_pointer.transform.position - point.transform.position ;
                point.transform.localScale = new Vector3(point.transform.localScale.x,point.transform.localScale.y,Vector3.Distance(point.transform.position,contact_pointer.transform.position) * 20);
                point.transform.Translate(point.transform.forward * Vector3.Distance(point.transform.position,contact_pointer.transform.position) * 0.5f,Space.World);
            
                // changes material
                float distance = Vector3.Distance(transform.position,contact[I].transform.position);
                if(distance < 120){
                    // red danger
                    contact[I].contact_indicator.GetComponent<MeshRenderer>().material = material_list[0];
                }
                else if(distance < 350){
                    // orange danger
                    contact[I].contact_indicator.GetComponent<MeshRenderer>().material = material_list[1];
                }
                else{
                    // yellow danger
                    contact[I].contact_indicator.GetComponent<MeshRenderer>().material = material_list[2];
                }
                if(distance > 800){
                    // out of sensor range
                    contact[I].contact_indicator.SetActive(false);
                }
                else{
                    if(contact[I].contact_indicator.activeInHierarchy == false){
                        source_gun.PlayOneShot(target_ping);
                    }
                    contact[I].contact_indicator.SetActive(true);
                }
                // debug and temporary
                if(distance < 2){
                    Debug.Log("break");
                    Application.Quit();
                }
            }
            else if(contact[I].fuel <= 0){
                Destroy(contact[I].contact_indicator);
            }
        }
    }

    void OnTriggerStay(Collider col){
        if(col.gameObject.tag == "enemy"){
            Debug.Log("break");
            Destroy(gameObject);
        }
    }

    // void jiggle_phys_cam_control(){
    //     rot_x = Input.GetAxis("Mouse X") * sensitivity;
    //     rot_Y = Input.GetAxis("Mouse Y") * -sensitivity;
    //     cam.transform.Rotate(rot_Y,rot_x,0,Space.World);
        
    //     cam.transform.eulerAngles = new Vector3(cam.transform.localEulerAngles.x,cam.transform.localEulerAngles.y,cam.transform.localEulerAngles.z);

    //     cam.transform.rotation = Quaternion.Lerp(cam.transform.rotation, transform.rotation, ship_rotate_speed /2 * Time.deltaTime);

    //     // rotate ship toward camera
    //     transform.rotation = Quaternion.Lerp(transform.rotation, cam.transform.rotation, ship_rotate_speed * Time.deltaTime);
    // }
}