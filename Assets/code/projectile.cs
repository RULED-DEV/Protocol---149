using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class projectile : MonoBehaviour
{
    public Rigidbody RB;
    public weapon_script weapon;
    public GameObject target;

    public void launch(){
        RB.AddForce(transform.forward * weapon.knockback,ForceMode.Impulse); // recoil
        Invoke("tracer",0.001f);
        Invoke("clutter",5);
    }

    void clutter(){
        Destroy(gameObject);
    }

    void tracer(){
        transform.GetChild(0).gameObject.SetActive(true);
    }
}
