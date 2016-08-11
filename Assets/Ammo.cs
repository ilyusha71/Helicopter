using UnityEngine;
using System.Collections;

public class Ammo : MonoBehaviour
{
    public float speedAmmo;
    public float durationFlying;
    public float damageBase;

    protected AmmoData data;
    protected Transform myTransform;
    protected Rigidbody myRigidbody;
    protected Vector3 lastPosition;
    protected SFXPool sfxPool;
    protected float distance;
    protected float instantaneousSpeed;
    protected float timeRecovery;
    protected float damage;
    protected bool checkShield;
    protected bool checkCollision;

    protected void Initialize() // Awake
    {
        data = GetComponent<AmmoData>();
        myTransform = transform;
        myRigidbody = GetComponent<Rigidbody>();
        lastPosition = Vector3.zero;
        sfxPool = FindObjectOfType<SFXPool>();
    }
    protected void Reset() // Enable
    {
        lastPosition = myTransform.position;
        myRigidbody.AddForce(myTransform.forward * speedAmmo, ForceMode.Impulse);
        timeRecovery = Time.time + durationFlying;
        damage = damageBase;
        checkShield = false; // Check different frame
        checkCollision = false;
    }
    protected void Flying() // FixedUpdate
    {
        if (!checkCollision)
        {
            if (lastPosition != myTransform.position)
            {
                float distance = Vector3.Distance(lastPosition, myTransform.position);
                RaycastHit[] hits = Physics.RaycastAll(lastPosition, myTransform.forward, distance);
                CollisionDetect(hits);
            }
            lastPosition = myTransform.position;
        }
        //instantaneousSpeed = myRigidbody.velocity.magnitude;
    }
    protected void CollisionDetect(RaycastHit[] hits)
    {
        for (int i = 0; i < hits.Length; i++)
        {
            if (hits[i].transform.gameObject != gameObject) // 不判斷自己
            {
                if (!checkCollision)
                {
                    CollisionProcessing(hits[i].transform, hits[i].point);
                }
            }
        }
    }
    protected void CollisionProcessing(Transform hitObj, Vector3 hitPoint) // OnCollisionEnter
    {
        TriggerSFX(hitPoint);
    }
    public virtual void TriggerSFX(Vector3 hitPoint)
    {
        checkCollision = true;
        sfxPool.expo.Reuse(hitPoint);
        Recovery();
    }
    protected void ExplosionDamage(float radius)
    {
        Collider[] cover = Physics.OverlapSphere(myTransform.position, radius);
        Transform coverObj;
        for (int i = 0; i < cover.Length; i++)
        {
            coverObj = cover[i].transform;
            float distance = Vector3.Distance(coverObj.transform.position, myTransform.position);
            damage = damageBase * Mathf.Log10(radius + 2 - distance);
            if (!(damage > 0))
                damage = 0;
        }
        Recovery();
    }
    public virtual void Recovery() { }
}
