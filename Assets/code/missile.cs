using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class missile : MonoBehaviour{

    public GameObject target;
    public GameObject compass;
    public GameObject contact_indicator;
    public GameObject lock_icon;

    public GameObject FX;

    Rigidbody RB;

    public float HP;

    public float rot_speed;
    public float acceleration_speed;
    public float drag;
    public float max_velocity;
    public float control_ang;

    public float fuel;
    public float acceleration;

    public bool conserve_thrust; // only thrusts when missile is pointing at the player
    public bool predict_target_pos; // predicts where the playewr will be
    public bool aim_at_player_on_spawn; // aims at player when it spawns


    void Awake(){
        RB = gameObject.GetComponent<Rigidbody>();
        target = GameObject.Find("player");
        lock_icon.transform.parent.GetComponent<Canvas>().worldCamera = target.transform.GetChild(1).GetComponent<Camera>();
        if(aim_at_player_on_spawn){
            Vector3 predict = target.transform.position;
            transform.forward = (predict - transform.position);
        }
    }

    void Update(){
        RB.drag = 0;
        if(HP <= 0){
            fuel = 0;
        }
        if(fuel > 0 && HP > 0){
            control();
            compass_cont();
        }
        else{
            FX.SetActive(false);
        }
        
    }

    void compass_cont(){
        // T = D / V
        Vector3 predict = target.transform.position;
        if(predict_target_pos){//  && Vector3.Distance(transform.position,target.transform.position) > 120 || Vector3.Magnitude(target.GetComponent<Rigidbody>().velocity) < 30 || Vector3.Magnitude(RB.velocity) < 30
            predict = target.transform.position + target.GetComponent<Rigidbody>().velocity;
            float time_collision = Vector3.Distance(target.transform.position,predict) / Vector3.Magnitude(RB.velocity); 
            predict = target.transform.position + (target.GetComponent<Rigidbody>().velocity * time_collision);
        }
        if(Vector3.Distance(transform.position,target.transform.position) < Vector3.Distance(transform.position,predict) + 100){
            predict = target.transform.position;
        }
        compass.transform.forward = (predict - transform.position);
    }

    void control(){
        float angle = Vector3.Distance(transform.eulerAngles,compass.transform.localEulerAngles);
        if((angle < control_ang || conserve_thrust == false) && fuel > 0){
            RB.drag = 0.0f;
            //RB.AddForce(transform.forward * acceleration_speed);
            RB.velocity = transform.forward * max_velocity * acceleration;
            
            fuel-- ;
        }
        if(angle < control_ang){
            if(acceleration < 1){
                acceleration += acceleration_speed;
            }
            else{
                acceleration += acceleration_speed / 100;
            }
        }
        else{
            if(acceleration > 0){
                acceleration -= drag;
            }
            else{
                acceleration = 0;
            }
        }
        RB.MoveRotation(Quaternion.Lerp(transform.rotation, compass.transform.rotation, rot_speed * Time.deltaTime));
    }

    void OnTriggerStay(Collider col){
        if(col.gameObject.tag == "weapon"){
            Debug.Log("strike");
            HP--;
        }
    }
}