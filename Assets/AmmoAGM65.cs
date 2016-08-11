using UnityEngine;
using System.Collections;

public class AmmoAGM65 : Ammo
{
    public float propulsion;

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
        data.ammoPool.turretMissile.Recovery(gameObject);
    }
    void Update()
    {
        if (Time.time > timeRecovery)
            Recovery();
    }
    void FixedUpdate()
    {
        myRigidbody.AddForce(myTransform.forward * propulsion, ForceMode.Acceleration);
        myRigidbody.AddForce(-myRigidbody.velocity, ForceMode.Acceleration);
    }
    void OnTriggerEnter(Collider hit)
    {
        if (hit.transform.tag == "Fuselage")
        {
            hit.transform.parent.parent.GetComponent<HelicopterController>().Shake(myTransform.forward,7);
            sfxPool.expo2.Reuse(hit.transform.position);
            Recovery();
        }
    }
}
