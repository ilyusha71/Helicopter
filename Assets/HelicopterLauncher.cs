using UnityEngine;
using UnityEngine.UI;

using System.Collections;

public class HelicopterLauncher : WeaponSystem
{
    public Text textcount;
    private int count;

    public float delayRecharge;
    private int orderMuzzle;
    private Vector3 trajectory; // Direction of the trajectory
    private float[] timeRecharge;

    private Transform target;
    private int orderTarget;
    void Awake()
    {
        Initialize();
        timeRecharge = new float[muzzle.Length];
    }
    void Update ()
    {
        if (WakakaKocmoca.instance.role == WakakaKocmoca.Role.Launcher)
        {
            if (Input.GetKeyDown(KeyCode.T))
            {
                orderTarget++;
                if (orderTarget == WakakaPlanet.instance.nowStage.enermyList.Length)
                    orderTarget = 0;
                target = WakakaPlanet.instance.nowStage.enermyList[orderTarget];
                //target.GetComponent<OpposingForce>().targetState = OpposingForce.TargetState.Locked;

                if (WakakaKocmoca.instance.gameState == WakakaKocmoca.GameState.Play)
                {
                    string msg = "Launcher/Lock/" + orderTarget + "/";
                    WakakaKocmoca.instance.client.SocketSend(msg);
                }
            }
            if (Input.GetMouseButton(1))
            {
                if (Time.time > timeNextRound)
                {
                    timeNextRound = Time.time + delayRound;
                    Shoot();
                }
            }
            for (int i = 0; i < muzzle.Length; i++)
            {
                if (Time.time > timeRecharge[i])
                {
                    muzzle[i].gameObject.SetActive(true);
                }
            }
        }
        if (target)
        {
            target.GetComponent<OpposingForce>().LockOn();
            if (!target.gameObject.activeSelf)
                target = null;
        }
    }

    private void Shoot()
    {
        if (muzzle[orderMuzzle].gameObject.activeSelf)
        {
            count++;
            textcount.text = "" + count;

            timeRecharge[orderMuzzle] = Time.time + delayRecharge;
            trajectory = muzzle[orderMuzzle].forward;
            ammoPool.missile.Reuse(muzzle[orderMuzzle].position, trajectory, WakakaPlanet.instance.helicopter.velocity,target);
            if (WakakaKocmoca.instance.gameState == WakakaKocmoca.GameState.Play)
            {
                string msg = "Launcher/Shoot/" + orderMuzzle+"/";
                WakakaKocmoca.instance.client.SocketSend(msg);
            }
            muzzle[orderMuzzle].gameObject.SetActive(false);
            orderMuzzle++;
            if (orderMuzzle == muzzle.Length)
                orderMuzzle = 0;


        }
    }
    public void Shoot(int order)
    {
        count++;
        textcount.text = "" + count;

        trajectory = muzzle[order].forward;
        ammoPool.missile.Reuse(muzzle[order].position, trajectory, WakakaPlanet.instance.helicopter.velocity, target);


    }

    public void SynchonizeLock(int order)
    {
        target = WakakaPlanet.instance.nowStage.enermyList[order];
    }
}
