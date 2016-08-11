using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class OpposingForce : WeaponSystem
{
    public enum TargetState
    {
        Fobbiden,
        Ready,
        Locked,
    }
    public TargetState targetState;
    public Transform markUI;
    public GameObject markObj;
    public Vector3 markOffset;
    public Image mark;
    private GameObject target;
    private float distance;

    public Collider shield;
    private Vector3 trajectory; // Direction of the trajectory
    private Vector3 offset;
    void Awake ()
    {
        target = GameObject.FindGameObjectWithTag("Fuselage");
        Initialize();
        mark = Instantiate(markObj).GetComponent<Image>();
        mark.transform.SetParent(markUI);
        mark.rectTransform.localScale = new Vector3(1, 1, 1);
    }
    void OnEnable()
    {
        targetState = TargetState.Ready;
    }
    void OnDisable()
    {
        targetState = TargetState.Fobbiden;
        if(mark)
            mark.gameObject.SetActive(false);

    }
    void Update ()
    {
        distance = Vector3.Distance(myTransform.position, target.transform.position);
        if (Time.time > timeNextRound && distance < 273 && distance > 5)
        {
            timeNextRound = Time.time + delayRound;
            Shoot();
        }
        myTransform.LookAt(target.transform);

        if (WakakaKocmoca.instance.client.udp == SocketUDP.UDP.Client)
        {
            if (targetState == TargetState.Ready)
                mark.gameObject.SetActive(false);
            else if (targetState == TargetState.Locked)
            {
                mark.gameObject.SetActive(true);
                mark.rectTransform.position = Navigator();
            }
        } 
    }

    private void Shoot()
    {
        int rand = Random.Range(0, 10);
        offset = new Vector3(rand, rand, rand);
        trajectory = target.transform.position - muzzle[0].position + offset;
        ammoPool.turretPellet.Reuse(muzzle[0].position, trajectory, Vector3.zero);
    }
    public void LockOn()
    {
        targetState = TargetState.Locked;
    }
    Vector3 Navigator()
    {
        Vector3 rect = Camera.main.WorldToScreenPoint(myTransform.position+markOffset);
        if (rect.z > 0)
        {
            //if (rect.x < 27.5f)
            //    rect.x = 27.5f;
            //else if (rect.x > Screen.width - 27.5f)
            //    rect.x = Screen.width - 27.5f;
            //if (rect.y < 27.5f)
            //    rect.y = 27.5f;
            //else if (rect.y > Screen.height - 27.5f)
            //    rect.y = Screen.height - 27.5f;
        }
        else
        {
            //if (rect.x < Screen.width * 0.5f)
            //    rect.x = Screen.width - 27.5f;
            //else
            //    rect.x = 27.5f;
            //if (rect.y < Screen.height * 0.5f)
            //    rect.y = Screen.height - 27.5f;
            //else
            //    rect.y = 27.5f;
            rect = new Vector2(-100, -100);
        }
        return rect;
    }
}
