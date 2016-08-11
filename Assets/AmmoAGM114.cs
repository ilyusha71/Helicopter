using UnityEngine;
using System.Collections;

public class AmmoAGM114 : Ammo
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
        data.ammoPool.missile.Recovery(gameObject);
    }
    void Update ()
    {
        if (Time.time > timeRecovery)
            Recovery();
    }
    void FixedUpdate()
    {
        myRigidbody.AddForce(myTransform.forward * propulsion, ForceMode.Acceleration);
        myRigidbody.AddForce(-myRigidbody.velocity , ForceMode.Acceleration);
    }
    void OnTriggerEnter(Collider hit)
    {
        if (hit.transform.tag == "Opposing Force")
        {
            sfxPool.expo2.Reuse(hit.transform.position);
            hit.transform.parent.gameObject.SetActive(false);
            Recovery();
        }
    }
}
