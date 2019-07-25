using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GalagaPlayer : MonoBehaviour
{
    public string Team;
    public float Speed = 1f;
    public float Damage = 1f;
    public float FireRate = 1f;
    public float Health = 10f;

    public GameObject Ammo;

    private float _lastFired;

    private void Fire()
    {
        if (Time.time - _lastFired <= FireRate)
        {
            _lastFired = Time.time;
        }
    }

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        Fire();
    }
}
