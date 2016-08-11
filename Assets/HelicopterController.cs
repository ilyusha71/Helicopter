using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using DG.Tweening;

public class HelicopterController : MonoBehaviour
{
    public Transform mainRotor;
    public Transform tailRotor;
    public float spdForward;
    public float spdRotate;
    public float spdElevate;
    public Text hint;

    private Transform myTransform;
    private Rigidbody myRigidbody;
    private AudioSource myAudio;
    private float pitch;
    private float roll;
    private float throttle = 0f;
    private float lift;
    private float _vPitch;
    private float _vRoll;
    private float _vElevate;

    public Vector3 last;
    public Vector3 velocity;

    public int hull = 1000;

    public float delayShake;
    private float timeNextShake;
    private float shake = 5;
    private bool shakeSwitch = false;
    Vector3 shakeOffset;
    public float shakePower;

    public static Vector3 synchronizedPos;
    public static Vector3 synchronizedRot;

    void Awake()
    {
        myTransform = transform;
        myRigidbody = GetComponent<Rigidbody>();
        myAudio = GetComponent<AudioSource>();
    }
	void Update ()
    {
        //Debug.Log(hull);
        mainRotor.Rotate(Vector3.up, 360 * 18000 * Time.deltaTime);
        tailRotor.Rotate(Vector3.up, 360 * 54000 * Time.deltaTime);
    }
    void FixedUpdate()
    {
        if (WakakaKocmoca.instance.role == WakakaKocmoca.Role.Controller)
        {
            velocity = (myTransform.position - last) / Time.deltaTime;
            last = myTransform.position;
            /* Non Physics Control */
            _vPitch = Input.GetAxis("Vertical");
            if (_vPitch == 0)
                pitch = Mathf.LerpAngle(pitch, 0, 0.37f * Time.deltaTime);
            else
                pitch = Mathf.LerpAngle(pitch, pitch += _vPitch * 0.137f, 100 * Time.deltaTime);
            pitch = Mathf.Clamp(pitch, -23, 23);

            _vRoll = Input.GetAxis("Horizontal");
            if (_vRoll == 0)
                roll = Mathf.LerpAngle(roll, 0, 0.37f * Time.deltaTime);
            else
                roll = Mathf.LerpAngle(roll, roll -= _vRoll * 0.237f, 100 * Time.deltaTime);
            roll = Mathf.Clamp(roll, -23, 23);

            if (Input.GetKey(KeyCode.Space))
                _vElevate = 0.1f;
            else if (Input.GetKey(KeyCode.LeftShift))
                _vElevate = -0.1f;

            if (Input.GetKey(KeyCode.KeypadPlus))
                throttle = Mathf.LerpAngle(throttle, throttle += 0.01f, 50 * Time.deltaTime);
            else if (Input.GetKey(KeyCode.KeypadMinus))
                throttle = Mathf.LerpAngle(throttle, throttle -= 0.01f, 50 * Time.deltaTime);
            throttle = Mathf.Clamp(throttle, 0, 1);

            /* Processing */
            lift = Mathf.LerpAngle(lift, throttle * (1 + pitch * 0.03f), 0.73f * Time.deltaTime);
            //throttle = Mathf.Clamp(throttle, -0.005f, 0.3f);
            //Debug.Log(pitch + " / " + throttle + " / " + lift + " / " + lift * spdForward);
            //myTransform.Translate(Vector3.forward * _vForward * spdForward); // 非物理
            myTransform.Translate(Vector3.forward * lift * spdForward); // 類物理
            myTransform.RotateAround(myTransform.position, Vector3.up, -roll * spdRotate * 0.055f);
            myTransform.Translate(Vector3.up * _vElevate * spdElevate);

            if (shakeSwitch == true)
            {
                Vector3 rand2 = new Vector3(
                    Random.Range(0, shake * 2 * shakePower) - shake * shakePower + shakeOffset.x,
                    Random.Range(0, shake * 2 * shakePower) - shake * shakePower + shakeOffset.y,
                    Random.Range(0, shake * 2 * shakePower) - shake * shakePower + shakeOffset.z) * 0.037f;
                myTransform.position += rand2;
                shake = shake / 1.05f;

                if (shake < 0.05)
                {
                    shake = 5;
                    shakeSwitch = false;
                }
            }

            _vElevate = 0.0f;

            /* Physics Control */
            //myRigidbody.AddForceAtPosition(myTransform.up * throttle, myTransform.position, ForceMode.Acceleration);
            //myRigidbody.AddForce(myRigidbody.velocity.magnitude * -myRigidbody.velocity, ForceMode.Acceleration);
            myTransform.localEulerAngles = new Vector3(pitch, myTransform.localEulerAngles.y, roll);

            if (WakakaKocmoca.instance.gameState == WakakaKocmoca.GameState.Play)
            {
                string msg = "Controller/" + 
                    myTransform.position.x + "," + 
                    myTransform.position.y + "," + 
                    myTransform.position.z + "," +
                    myTransform.localEulerAngles.x + "," + 
                    myTransform.localEulerAngles.y + "," + 
                    myTransform.localEulerAngles.z + ",";
                WakakaKocmoca.instance.client.SocketSend(msg);
            }
        }
        else
        {
            if (WakakaKocmoca.instance.gameState == WakakaKocmoca.GameState.Play)
            {
                myTransform.position = synchronizedPos;
                myTransform.localEulerAngles = synchronizedRot;
            }
        }
    }
    void LastUpdate()
    {
        
    }
    void OnTriggerEnter(Collider hit)
    {
        if (WakakaKocmoca.instance.role == WakakaKocmoca.Role.Controller)
        {
            if (hit.name == "Buffering")
                hint.text = "正偏離航線！！！請盡速返回";
        }
        if (hit.name == "Waypoint")
            WakakaPlanet.instance.nowStage.FlyByWaypoint(hit.transform);
    }
    void OnTriggerExit(Collider hit)
    {
        if (WakakaKocmoca.instance.role == WakakaKocmoca.Role.Controller)
        {
            if (hit.name == "Buffering")
                hint.text = "";
        }
        if (hit.name == "Waypoint")
            WakakaPlanet.instance.nowStage.ExitWaypoint(hit.transform);
    }
    void OnCollisionEnter(Collision hit)
    {
        if (hit.transform.name == "Boundary")
        {
            if (WakakaKocmoca.instance.role == WakakaKocmoca.Role.Controller)
                hint.text = "已駛離航道，將切換自動駕駛";
            myRigidbody.Sleep();
        }
        else
        {
            //Debug.LogError("Crash");
        }
    }
    void OnCollisionExit(Collision hit)
    {
        if (hit.transform.name == "Boundary")
        {
            if (WakakaKocmoca.instance.role == WakakaKocmoca.Role.Controller)
                hint.text = "自動駕駛解除中";
            myRigidbody.Sleep();
        }
    }

    public void Shake(Vector3 offset, float power)
    {
        if (WakakaKocmoca.instance.role == WakakaKocmoca.Role.Controller)
        {
            if (Time.time > timeNextShake)
            {
                shakePower = power;
                timeNextShake = Time.time + delayShake + Random.Range(-1.5f, 1.5f);
                //shakeSwitch = true;
                //shakeOffset = offset;
                myAudio.pitch = Random.Range(1, 7) * 0.5f;
                myAudio.Play();
            }
            hull--;
        }
    }

    void Reborn()
    {

    }
}