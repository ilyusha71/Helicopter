using UnityEngine;
using System.Collections;

public class HelicopterDoorGunner : WeaponSystem
{
    public enum GunnerPort
    {
        FrontLeft,
        FrontRight,
        Left,
        Right,
        RearLeft,
        RearRight,
    }
    public GunnerPort gunner;
    public GameObject fire;

    private float wpnH;
    private float wpnV;
    private RaycastHit hit;

    private Vector3 trajectory; // Direction of the trajectory

    void Awake ()
    {
        Initialize();
    }
	
	void Update ()
    {
        if (((int)gunner + 3) == (int)WakakaKocmoca.instance.role)
        {
            if (Input.GetMouseButton(0))
            {
                if (Time.time > timeNextRound)
                {
                    timeNextRound = Time.time + delayRound;
                    Shoot();
                }
            }

            if ((int)gunner % 2 == 0)
            {
                wpnH += Input.GetAxis("Mouse X");
                wpnV -= Input.GetAxis("Mouse Y");
                wpnH = Mathf.Clamp(wpnH, -45, 45);
                wpnV = Mathf.Clamp(wpnV, -30, 45);
                myTransform.localEulerAngles = new Vector3(0, wpnH, wpnV);
            }
            else
            {
                wpnH += Input.GetAxis("Mouse X");
                wpnV -= Input.GetAxis("Mouse Y");
                wpnH = Mathf.Clamp(wpnH, 135, 225);
                wpnV = Mathf.Clamp(wpnV, -30, 45);
                myTransform.localEulerAngles = new Vector3(0, wpnH, wpnV);
            }

            if (WakakaKocmoca.instance.gameState == WakakaKocmoca.GameState.Play)
            {
                string msg = "Gunner/" +
                    (int)gunner + "/"+
                    "Aim/" +
                    wpnH + "," +
                    wpnV + ",";
                WakakaKocmoca.instance.client.SocketSend(msg);
            }
        }       
    }

    public void SynchronizeWPN(float h, float v)
    {
        myTransform.localEulerAngles = new Vector3(0, h, v);
    }

    private void Shoot()
    {
        trajectory = muzzle[0].forward;
        fire.SetActive(false);
        ammoPool.bullet.Reuse(muzzle[0].position, trajectory, WakakaPlanet.instance.helicopter.velocity);
        if (WakakaKocmoca.instance.gameState == WakakaKocmoca.GameState.Play)
        {
            string msg = "Gunner/" +
                (int)gunner + "/" +
                "Shoot/" +
                muzzle[0].position.x + "," +
                muzzle[0].position.y + "," +
                muzzle[0].position.z + "," +
                trajectory.x + "," +
                trajectory.y + "," +
                trajectory.z + "," +
                WakakaPlanet.instance.helicopter.velocity.x + "," +
                WakakaPlanet.instance.helicopter.velocity.y + "," +
                WakakaPlanet.instance.helicopter.velocity.z + ",";
            WakakaKocmoca.instance.client.SocketSend(msg);
        }
        fire.SetActive(true);
    }
}
