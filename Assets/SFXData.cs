using UnityEngine;
using System.Collections;

public class SFXData : MonoBehaviour
{
    [HideInInspector]
    public SFXPool sfxPool;
    [HideInInspector]
    public Transform container;
    public float durationLifetime;
    protected float timeRecovery;
    void Awake()
    {
        sfxPool = FindObjectOfType<SFXPool>();
    }
    void OnEnable()
    {
        timeRecovery = Time.time + durationLifetime;
    }
    void Recovery()
    {
        if(name == "Explosion(Clone)")
            sfxPool.Recovery(100,gameObject);
        else
            sfxPool.Recovery(1, gameObject);
    }
    void Update()
    {
        if (Time.time > timeRecovery)
            Recovery();
    }
}
