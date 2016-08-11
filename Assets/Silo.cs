using UnityEngine;
using System.Collections;

public class Silo : WeaponSystem
{
    private Vector3 trajectory;
    private GameObject target;

    void Awake()
    {
        target = GameObject.FindGameObjectWithTag("Fuselage");
        Initialize();
    }
    void Update()
    {
        myTransform.LookAt(target.transform);
        if (Input.GetKeyDown(KeyCode.M))
            Launch();
    }
    void Launch()
    {
        trajectory = transform.forward;
        ammoPool.turretMissile.Reuse(transform.position, trajectory, Vector3.zero, target.transform);
    }
}
