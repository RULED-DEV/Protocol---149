using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class scenario : MonoBehaviour
{
    public GameObject[] cage;

    public GameObject[] munitions;

    public float reload_time;
    public float reload_decriment;

    public bool endless;
    bool spawn = false;
    int count;

    int spawn_limit = 1000;

    float timer = 5;

    int pos;
    Vector3 spawn_pos;
    GameObject anchor;
    GameObject player;

    void Awake(){
        timer = Time.time + timer;
    }

    void Update(){
        player = GameObject.Find("player");
        if(Input.GetKeyDown(KeyCode.Escape) || player == null){
            Destroy(gameObject);
        }
        if(count >= munitions.Length && endless){
            count = 0;
        }
        if(count < munitions.Length || endless){
            if(timer <= Time.time && spawn == false){
                reload_time -= reload_decriment;

                // spawn missile
                spawn_pos = new Vector3(Random.Range(-spawn_limit,spawn_limit),Random.Range(-spawn_limit,spawn_limit),Random.Range(-spawn_limit,spawn_limit));
                pos = Random.Range(0,6);
                // cage method
                    // if(pos == 0){
                    //     spawn_pos = new Vector3(Random.Range(900,900),1000,Random.Range(-900,900)); // up
                    //     anchor = GameObject.Find("up");
                    // }
                    // if(pos == 1){
                    //     spawn_pos = new Vector3(Random.Range(-900,900),-1000,Random.Range(-900,900)); // down
                    //     anchor = GameObject.Find("down");
                    // }
                    // if(pos == 2){
                    //     spawn_pos = new Vector3(-1000,Random.Range(-900,900),Random.Range(-900,900)); // left
                    //     anchor = GameObject.Find("left");
                    // }
                    // if(pos == 3){
                    //     spawn_pos = new Vector3(1000,Random.Range(-900,900),Random.Range(-900,900)); // right
                    //     anchor = GameObject.Find("right");
                    // }
                    // if(pos == 4){
                    //     spawn_pos = new Vector3(Random.Range(-900,900),Random.Range(-900,900),1000); // forward
                    //     anchor = GameObject.Find("forward");
                    // }
                    // if(pos >= 5){
                    //     spawn_pos = new Vector3(Random.Range(-900,900),Random.Range(-900,900),-1000); // back
                    //     anchor = GameObject.Find("back");
                    // }
                    // spawn = true;
                    // }
                    // if(spawn){
                    //     cage[pos].transform.position = Vector3.Lerp(cage[pos].transform.position,spawn_pos,8 * Time.deltaTime);
                    //     // set beam pos
                    //         if(pos == 0){ // up
                    //             cage[pos].transform.GetChild(0).position = new Vector3(cage[pos].transform.position.x,anchor.transform.position.y,anchor.transform.position.z); // y beam
                    //             cage[pos].transform.GetChild(1).position = new Vector3(anchor.transform.position.x,anchor.transform.position.y,cage[pos].transform.position.z); // x beam
                    //         }
                    //         if(pos == 1){ // down
                    //             cage[pos].transform.GetChild(0).position = new Vector3(cage[pos].transform.position.x,anchor.transform.position.y,anchor.transform.position.z); // y beam
                    //             cage[pos].transform.GetChild(1).position = new Vector3(anchor.transform.position.x,anchor.transform.position.y,cage[pos].transform.position.z); // x beam
                    //         }
                    //         if(pos == 2){ // left
                    //             cage[pos].transform.GetChild(0).position = new Vector3(anchor.transform.position.x,cage[pos].transform.position.y,anchor.transform.position.z); // y beam
                    //             cage[pos].transform.GetChild(1).position = new Vector3(anchor.transform.position.x,anchor.transform.position.y,cage[pos].transform.position.z); // x beam
                    //         }
                    //         if(pos == 3){ // right
                    //             cage[pos].transform.GetChild(0).position = new Vector3(anchor.transform.position.x,cage[pos].transform.position.y,anchor.transform.position.z); // y beam
                    //             cage[pos].transform.GetChild(1).position = new Vector3(anchor.transform.position.x,anchor.transform.position.y,cage[pos].transform.position.z); // x beam
                    //         }
                    //         if(pos == 4){ // forward
                    //             cage[pos].transform.GetChild(0).position = new Vector3(anchor.transform.position.x,cage[pos].transform.position.y,cage[pos].transform.position.z); // y beam
                    //             cage[pos].transform.GetChild(1).position = new Vector3(cage[pos].transform.position.x,anchor.transform.position.y,cage[pos].transform.position.z); // x beam
                    //         }
                    //         if(pos == 5){ // back
                    //             cage[pos].transform.GetChild(0).position = new Vector3(anchor.transform.position.x,cage[pos].transform.position.y,cage[pos].transform.position.z); // y beam
                    //             cage[pos].transform.GetChild(1).position = new Vector3(cage[pos].transform.position.x,anchor.transform.position.y,cage[pos].transform.position.z); // x beam
                    //         }
                    //     if(Vector3.Distance(cage[pos].transform.position,spawn_pos) < 120){ // spawn missile
                    //         timer = reload_time + Time.time;
                    //         Instantiate(munitions[count],cage[pos].transform.position,cage[pos].transform.rotation).transform.parent = gameObject.transform;
                    //         count ++;
                    //         spawn = false;
                    //     }
                // cage method 
                // set beam pos
                 // spawn missile
                timer = reload_time + Time.time;
                Instantiate(munitions[count],player.transform.position + spawn_pos,transform.rotation).transform.parent = gameObject.transform;
                count ++;
                spawn = false;
            }
        }
    }
}
