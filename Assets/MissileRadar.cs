/*******************************************************************
* Last Updated: 2016.05.05
* Initial: 2016.05.05
* Author: iLYuSha KocmocA
*******************************************************************/
using UnityEngine;
using System.Collections;

public class MissileRadar : MonoBehaviour
{
    public enum MissileMode
    {
        Physics,
        Hit,
    }
    public MissileMode missileMode;
    public Transform target;
    public float angleScan; // cosine value : 60 degrees = 0.5f
    public float delaySearch;
    public float revolve;

    private Transform myTransform;
    private Vector3 intDirection;
    private Vector3 fnlDirection;
    private float distanceTarget;
    private float radianRadar;
    public float radianTarget;
    private float timeLocked;

    void Awake()
    {
        myTransform = transform;
        radianRadar = Mathf.Cos(angleScan);
    }
    void OnEnable()
    {
        timeLocked = Time.time + delaySearch;
    }
    void Update ()
    {
        if (target != null)
        {
            if (Time.time > timeLocked)
            {
                intDirection = myTransform.forward;
                fnlDirection = target.position - myTransform.position;
                distanceTarget = fnlDirection.magnitude;
                radianTarget = Vector3.Dot(intDirection.normalized, fnlDirection.normalized);
                if (missileMode == MissileMode.Physics)
                {
                    if (radianTarget > radianRadar)
                    {
                        //revolve = radianTarget * 7.653f - 6.7f;
                        revolve = Mathf.Pow(6.78473f, radianTarget) - 5.28973f;
                        myTransform.forward = Vector3.Lerp(intDirection, fnlDirection, Time.deltaTime * revolve);
                    }
                }
                else
                {
                    if (radianTarget > radianRadar)
                    {
                        //revolve = radianTarget * 7.653f - 6.7f;
                        revolve = Mathf.Pow(6.78473f, radianTarget) - 5.28973f;
                        myTransform.forward = Vector3.Lerp(intDirection, fnlDirection, Time.deltaTime * revolve*1.37f);
                    }
                }
            }
        }
    }
}
