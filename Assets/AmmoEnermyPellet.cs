using UnityEngine;
using System.Collections;

public class AmmoEnermyPellet : MonoBehaviour
{
    public float life;
    public GameObject test;
    public float ammoSpeed;
    private AmmoData data;
    private Transform myTransform;
    private Rigidbody myRigidbody;
    private Vector3 lastPosition;
    private RaycastHit hit;
    private float distance;
    public Vector3 inertia;
    private float timeEnd;

    void Awake()
    {
        data = GetComponent<AmmoData>();
        myTransform = transform;
        myRigidbody = GetComponent<Rigidbody>();
    }
    void OnEnable()
    {
        myRigidbody.AddForce(myTransform.forward * ammoSpeed, ForceMode.Impulse);
        timeEnd = Time.time + life;
    }
    void Update()
    {
        if (Time.time > timeEnd)
        {
            myRigidbody.Sleep();
            data.ammoPool.turretPellet.Recovery(gameObject);

        }
    }
    void FixedUpdate()
    {
        distance = Vector3.Distance(lastPosition, myTransform.position);
        if (Physics.Raycast(lastPosition, myTransform.forward, out hit, distance))
        {
            if (hit.collider.transform.name == "fuselage")
            {
                hit.transform.GetComponent<HelicopterController>().Shake(myTransform.forward,3.7f);
                Instantiate(test, hit.point, Quaternion.identity);
                myRigidbody.Sleep();
                data.ammoPool.turretPellet.Recovery(gameObject);
            }
        }
        lastPosition = myTransform.position;
    }
}
