using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class SFXPool : MonoBehaviour
{
    [System.Serializable]
    public class SFX
    {
        public GameObject sfx;
        public Transform container;
        public int count;
        private Queue<GameObject> pool = new Queue<GameObject>();
        private SFXPool sfxPool;
        internal void Initialize(SFXPool sfxPool)
        {
            this.sfxPool = sfxPool;
        }
        internal void CreatePool()
        {
            for (int i = 0; i < count; i++)
            {
                GameObject go = Instantiate(sfx);
                pool.Enqueue(go);
                go.transform.parent = container;
                go.GetComponent<SFXData>().sfxPool = sfxPool;
                go.GetComponent<SFXData>().container = container;
                go.SetActive(false);
            }
        }
        internal void Reuse(Vector3 position)
        {
            GameObject go = pool.Dequeue();
            go.transform.position = position;
            go.SetActive(true);
        }
        internal void Recovery(GameObject go)
        {
            pool.Enqueue(go);
            go.SetActive(false);
            go.transform.position = Vector3.zero;
        }
    }
    public SFX expo = new SFX();
    public SFX expo2 = new SFX();
    void Awake()
    {
        expo.Initialize(this);
        expo2.Initialize(this);

    }
    void Start()
    {
        expo.CreatePool();
        expo2.CreatePool();

    }
    public void Recovery(int id, GameObject go)
    {
        if (id == 1)
            expo.Recovery(go);
        else
            expo2.Recovery(go);

    }
}