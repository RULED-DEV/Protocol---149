using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class weapon_script : MonoBehaviour
{
    public float fire_interval;
    public float rot_angle;
    public float knockback;
    public float max_ammo;
    public float weight_per_bullet;
    public GameObject projectile;

    public AudioClip fire_noise;
}
