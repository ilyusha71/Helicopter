using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class WakakaKocmoca : MonoBehaviour
{
    public static WakakaKocmoca instance;
    public enum GameState
    {
        Initial,
        Connect,
        Ready,
        Play,
    }
    public GameState gameState;
    public enum Role
    {
        None,
        Controller,
        Launcher,
        GunnerFrontLeft,
        GunnerFrontRight,
        GunnerLeft,
        GunnerRight,
        GunnerRearLeft,
        GunnerRearRight,
    }
    public Role role;
    public enum Viewpoint
    {
        N,
        NE,
        E,
        SE,
        S,
        SW,
        W,
        NW,
    }
    public Viewpoint viewpoint;
    public Text textFOV;
    public Text textResolution;
    public Text textGameState;
    public Text textRole;
    public Text textViewpoint;
    public Text textServerIP;
    public Text textLocalIP;
    public SocketUDP client;
    public InputField inputIP;
    public GameObject panel;
    public GameObject serverBlock;
    public GameObject localBlock;
    public GameObject disconnected;
    public GameObject serverCamera;
    public GameObject clientCamera;

    public GameObject setting;
    public GameObject roleSetting;

    void Awake()
    {
        instance = this;
        textRole.text = role.ToString();
        textViewpoint.text = viewpoint.ToString();
        Camera.main.aspect = 0.5f;
        if(Screen.fullScreen)
            Screen.SetResolution(1000, 2000, true);
        textResolution.text = Screen.width + " x " + Screen.height;
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.F11))
            InitializeFOV(false);
        else if (Input.GetKeyDown(KeyCode.F12))
            InitializeFOV(true);

        if (Input.GetKeyDown(KeyCode.F9))
            panel.SetActive(!panel.activeSelf);
        if (Input.GetKeyDown(KeyCode.F10))
            localBlock.SetActive(!localBlock.activeSelf);

        if (Input.GetKeyDown(KeyCode.V))
        {
            if (client.udp == SocketUDP.UDP.Client)
            {
                int index = (int)viewpoint;
                index++;
                if (index > 7)
                    index = 0;
                viewpoint = (Viewpoint)index;
                Camera.main.transform.localEulerAngles += new Vector3(0, 45, 0);
            }
        }
        textViewpoint.text = viewpoint.ToString();
        textGameState.text = gameState.ToString();
    }

    public void Connect(int role)
    {
        serverCamera.SetActive(false);
        this.role = (Role)role;
        textRole.text = this.role.ToString();
        client.AsClient();
    }

    void InitializeFOV(bool fullscreen)
    {
        float radHFOV = 45.0f * 1.2f * Mathf.Deg2Rad;
        float radVFOV = 2 * Mathf.Atan(Mathf.Tan(radHFOV / 2) / Camera.main.aspect);
        float vFOV = Mathf.Rad2Deg * radVFOV;
        Camera.main.fieldOfView = vFOV;
        textFOV.text = 45 + " x " + vFOV;

        Screen.SetResolution((int)(Screen.height* Camera.main.aspect), Screen.height, fullscreen);
        textResolution.text = Screen.width + " x " + Screen.height;
    }
}
