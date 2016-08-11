using UnityEngine;
using System.Collections;

public class ServerCamera : MonoBehaviour
{
	void Update ()
    {
        if (WakakaKocmoca.instance.client.udp == SocketUDP.UDP.Server)
        {
            float h = Input.GetAxis("Horizontal");
            transform.Rotate(new Vector3(0,h,0));
            //float v = Input.GetAxis("Vertical");
            //transform.Rotate(new Vector3(v,0 , 0));
            //if (transform.localEulerAngles.x > 37 && transform.localEulerAngles.x < 180)
            //    transform.localEulerAngles = new Vector3(37, transform.localEulerAngles.y, 0);
            //else if (transform.localEulerAngles.x > 180 && transform.localEulerAngles.x < 300)
            //    transform.localEulerAngles = new Vector3(300, transform.localEulerAngles.y, 0);

        }
    }
}
