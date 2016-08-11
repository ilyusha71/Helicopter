using UnityEngine;
using System.Collections;

public class WeaponSystem : MonoBehaviour
{
    public Transform[] muzzle;
    protected Transform myTransform;
    protected AmmoPool ammoPool;

    public float rpm;
    protected float delayRound; // Loading time
    protected float timeNextRound; // Next round of gametime
    protected void Initialize() // Awake
    {
        myTransform = transform;
        ammoPool = FindObjectOfType<AmmoPool>();
        delayRound = 60 / rpm;
    }
}
