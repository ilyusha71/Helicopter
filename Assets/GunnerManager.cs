using UnityEngine;
using System.Collections;

public class GunnerManager : MonoBehaviour
{
    [HideInInspector]
    public GunnerManager instance;
    public GameObject[] sight;
    public Transform[] LOS;

    private int nowViewpoint = 0;

    void Awake()
    {
        instance = this;
    }

    void LateUpdate()
    {
        Ray();
    }

    private void Ray()
    {
        if ((int)WakakaKocmoca.instance.viewpoint == 0 || (int)WakakaKocmoca.instance.viewpoint == 4)
        {
            for (int i = 0; i < sight.Length; i++)
            {
                sight[i].SetActive(false);
            }
            return;
        }
        else if ((int)WakakaKocmoca.instance.viewpoint < 4)
        {
            for (int i = 0; i < sight.Length; i++)
            {
                if (i % 2 == 1)
                {
                    sight[i].SetActive(true);

                    Vector3 rect = Camera.main.WorldToScreenPoint(LOS[i].position);
                    if (rect.z >= 1000)
                        rect.z = 999;
                    if (rect.z > 0)
                        sight[i].transform.position = rect;
                }
                else
                    sight[i].SetActive(false);
            }
        }
        else if ((int)WakakaKocmoca.instance.viewpoint > 4)
        {
            for (int i = 0; i < sight.Length; i++)
            {
                if (i % 2 == 0)
                {
                    sight[i].SetActive(true);

                    Vector3 rect = Camera.main.WorldToScreenPoint(LOS[i].position);
                    if (rect.z >= 1000)
                        rect.z = 999;
                    if (rect.z > 0)
                        sight[i].transform.position = rect;
                }
                else
                    sight[i].SetActive(false);
            }
        }
    }
}
