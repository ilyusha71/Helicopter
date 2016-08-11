using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class MessageProcessor : MonoBehaviour
{
    public InputField msgInput;
    public Text msgPrint;
    private HelicopterLauncher launcher;
    private WakakaPlanet planet;
    private AmmoPool pool;
    public HelicopterDoorGunner[] gunner;

    private int synconut;
    void Awake()
    {
        launcher = FindObjectOfType<HelicopterLauncher>();
        planet = FindObjectOfType<WakakaPlanet>();
        pool = FindObjectOfType<AmmoPool>();
    }

    public void GamingMsgProcessor(string msg)
    {
        //Debug.Log("PCE: " + msg);
        msgPrint.text = msg;
        if (msg != null)
        {
            string[] s = msg.Split('/');
            if (s[0] == "Controller")
            {
                if (s[1] == "Pass")
                {
                    if (WakakaKocmoca.instance.role != WakakaKocmoca.Role.Controller)
                        planet.Pass();
                }
                else
                    HelicopterTransform(s[1]);
            }
            else if (s[0] == "Launcher")
            {
                //Debug.LogWarning(i + " : " + msg);
                if (WakakaKocmoca.instance.role != WakakaKocmoca.Role.Launcher)
                {
                    Debug.LogWarning(msg);
                    if (s[1] == "Lock")
                        launcher.SynchonizeLock(int.Parse(s[2]));
                    else if (s[1] == "Shoot")
                    {
                        if (s[3] == "")
                        {
                            //if (synconut != int.Parse(s[2]))
                            //    Debug.LogError("STOP: LC: " + synconut + " LAUN: " + int.Parse(s[2]));
                            launcher.Shoot(int.Parse(s[2]));
                            synconut++;
                            if (synconut > 3)
                                synconut = 0;
                        }
                        else
                        {
                            if (int.Parse(s[2]) != synconut)
                                synconut = int.Parse(s[2]);
                            launcher.Shoot(int.Parse(s[2]));
                            synconut++;
                            if (synconut > 3)
                                synconut = 0;
                        }
                    }
                }
            }
            else if (s[0] == "Gunner")
            {
                int index = int.Parse(s[1]);
                if ((int)WakakaKocmoca.instance.role != index + 3)
                {
                    if (s[2] == "Aim")
                    {
                        string[] data = s[3].Split(',');
                        if (data.Length == 3)
                        {
                            float h = float.Parse(data[0]);
                            float v = float.Parse(data[1]);
                            gunner[index].SynchronizeWPN(h, v);
                        }
                    }
                    else if (s[2] == "Shoot")
                    {
                        BulletTransfer(s[3]);
                    }
                }
            }
        }  
    }

    void HelicopterTransform(string data)
    {
        string[] split = data.Split(',');
        float[] dataTrans = new float[6];
        if (split.Length == 7)
        {
            for (int i = 0; i < 6; i++)
            {
                dataTrans[i] = float.Parse(split[i]);
            }
            HelicopterController.synchronizedPos = new Vector3(dataTrans[0], dataTrans[1], dataTrans[2]);
            HelicopterController.synchronizedRot = new Vector3(dataTrans[3], dataTrans[4], dataTrans[5]);
        }
        else
            Debug.LogWarning("Controller 訊息錯誤：" + data);
    }

    void BulletTransfer(string data)
    {
        string[] split = data.Split(',');
        float[] dataTrans = new float[9];
        if (split.Length == 10)
        {
            for (int i = 0; i < 9; i++)
            {
                dataTrans[i] = float.Parse(split[i]);
            }
            Vector3 muzzle = new Vector3(dataTrans[0], dataTrans[1], dataTrans[2]);
            Vector3 trajectory = new Vector3(dataTrans[3], dataTrans[4], dataTrans[5]);
            Vector3 velocity = new Vector3(dataTrans[6], dataTrans[7], dataTrans[8]);
            pool.bullet.Reuse(muzzle, trajectory, velocity);
        }

    }
}
