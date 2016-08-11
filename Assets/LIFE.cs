using UnityEngine;
using System.Collections;

public class LIFE : MonoBehaviour
{
    private float life = 1.0f;
    private float end;
	void OnEnable ()
    {
        end = Time.time + life;
    }
	
	void Update ()
    {
        if (Time.time > end)
            Destroy(gameObject);
	}
}
