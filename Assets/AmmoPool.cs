using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class AmmoPool : MonoBehaviour
{
    public Transform missileCamera;
    [System.Serializable]
    public class Ammo
    {
        public GameObject ammo;
        public Transform container;
        public int count;
        private Queue<GameObject> pool = new Queue<GameObject>();
        private AmmoPool ammoPool;
        internal void Initialize(AmmoPool ammoPool)
        {
            this.ammoPool = ammoPool;
        }
        internal void CreatePool()
        {
            for (int i = 0; i < count; i++)
            {
                GameObject go = Instantiate(ammo);
                pool.Enqueue(go);
                go.transform.parent = container;
                go.GetComponent<AmmoData>().ammoPool = ammoPool;
                go.GetComponent<AmmoData>().container = container;
                go.SetActive(false);
            }
        }
        internal void Reuse(Vector3 position, Vector3 trajectory)
        {
            GameObject go = pool.Dequeue();
            go.transform.position = position;
            go.transform.forward = trajectory;
            go.SetActive(true);
        }
        internal void Reuse(Vector3 position, Vector3 trajectory, Vector3 inertia)
        {
            GameObject go = pool.Dequeue();
            go.transform.position = position;
            go.transform.forward = trajectory;
            go.GetComponent<Rigidbody>().velocity = inertia;
            go.SetActive(true);
        }
        internal void Reuse(Vector3 position, Vector3 trajectory, Vector3 inertia, Transform target)
        {
            GameObject go = pool.Dequeue();
            go.transform.position = position;
            go.transform.forward = trajectory;
            go.GetComponent<Rigidbody>().velocity = inertia;
            go.GetComponent<MissileRadar>().target = target;
            go.SetActive(true);
            ammoPool.missileCamera.SetParent(go.transform);
            ammoPool.missileCamera.localPosition = new Vector3(0, 1, -2.5f);
            ammoPool.missileCamera.localEulerAngles = new Vector3(11, 0, 0);
        }
        internal void Recovery(GameObject go)
        {
            pool.Enqueue(go);
            go.SetActive(false);
            go.transform.position = Vector3.zero;
        }
    }
    public Ammo bullet = new Ammo();
    public Ammo missile = new Ammo();
    public Ammo turretPellet = new Ammo();
    public Ammo turretMissile = new Ammo();
    void Awake()
    {
        bullet.Initialize(this);
        missile.Initialize(this);
        turretPellet.Initialize(this);
        turretMissile.Initialize(this);
    }
	void Start ()
    {
        bullet.CreatePool();
        missile.CreatePool();
        turretPellet.CreatePool();
        turretMissile.CreatePool();
    }
}
