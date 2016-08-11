using UnityEngine;
using System.Collections;

public class AmmoBullet : Ammo
{
    void Awake()
    {
        Initialize();
    }
    void OnEnable()
    {
        Reset();
    }
    public override void Recovery()
    {
        myRigidbody.Sleep();
        data.ammoPool.bullet.Recovery(gameObject);
    }
    void Update()
    {
        if (Time.time > timeRecovery)
            Recovery();
    }
    void FixedUpdate()
    {
        Flying();
    }
}
