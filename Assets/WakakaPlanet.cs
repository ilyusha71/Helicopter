using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class WakakaPlanet : MonoBehaviour
{
    public static WakakaPlanet instance;
    public Image nextWaypoint;
    public Image pathWaypoint;
    public Text msg;
    public HelicopterController helicopter;

    [System.Serializable]
    public class Stage
    {
        public bool pass;
        public GameObject waypointSet;
        public GameObject boundarySet;
        public int nowOrder;
        public int nextOrder;
        public Transform[] waypoint;
        public Transform[] enermyList;

        internal void Initialize()
        {
            Transform[] wpSet = waypointSet.transform.GetComponentsInChildren<Transform>();
            waypoint = new Transform[wpSet.Length - 1];
            for (int i = 1; i < wpSet.Length; i++)
            {
                waypoint[i - 1] = wpSet[i];
            }
        }
        internal void Navigation()
        {
            if (nextOrder < waypoint.Length)
            {
                if (pass)
                    instance.nextWaypoint.rectTransform.position = Navigator(0);
                else
                {
                    if (nextOrder < waypoint.Length - 1)
                        instance.nextWaypoint.rectTransform.position = Navigator(0);
                    else
                    {
                        instance.nextWaypoint.gameObject.SetActive(false);
                        // Boss
                    }
                }
            }

            if (nextOrder < waypoint.Length - 2)
                instance.pathWaypoint.rectTransform.position = Navigator(1);
            else
                instance.pathWaypoint.gameObject.SetActive(false);

            if (instance.pathWaypoint.rectTransform.position.z < instance.nextWaypoint.rectTransform.position.z)
                instance.pathWaypoint.rectTransform.SetSiblingIndex(999);
            else
                instance.pathWaypoint.rectTransform.SetSiblingIndex(1);
        }
        Vector3 Navigator(int index)
        {
            if(index == 0)
                instance.nextWaypoint.gameObject.SetActive(true);
            else if (index == 1)
                instance.pathWaypoint.gameObject.SetActive(true);

            Vector3 rect = Camera.main.WorldToScreenPoint(waypoint[nextOrder + index].position);
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
        public void FlyByWaypoint(Transform waypoint)
        {
            if (waypoint == this.waypoint[nextOrder])
            {
                if (nextOrder == this.waypoint.Length - 1)
                {
                    if (pass)
                    {
                        instance.msg.text = "已進入下一個戰區";
                        instance.nextWaypoint.gameObject.SetActive(false);
                        nowOrder = nextOrder;
                    }
                }
                else
                {
                    instance.msg.text = "已通過導航點";
                    nowOrder = nextOrder;
                    nextOrder++;
                }
            }
        }
        public void ExitWaypoint(Transform waypoint)
        {
            if (waypoint == this.waypoint[nowOrder])
            {
                instance.msg.text = "";
                this.waypoint[nowOrder].gameObject.SetActive(false);
                if (nowOrder == this.waypoint.Length - 1)
                {
                    if (pass)
                        instance.NextStage();
                }
            }
        }
    }
    public Stage stage1 = new Stage();
    public Stage stage2 = new Stage();
    public Stage stage3 = new Stage();
    public Stage stage4 = new Stage();
    private Stage[] listStage;
    private int indexStage = 0;
    public Stage nowStage;

    public Camera camera1;
    public Camera camera2;
    public Camera camera3;
    public Camera camera4;

    int count;

    void Awake()
    {
        instance = this;
        listStage = new Stage[4];
        (listStage[0] = stage1).Initialize();
        (listStage[1] = stage2).Initialize();
        (listStage[2] = stage3).Initialize();
        (listStage[3] = stage4).Initialize();
    }
    void Start ()
    {
        nowStage = listStage[indexStage];
    }
    void Update ()
    {
        if (WakakaKocmoca.instance.gameState == WakakaKocmoca.GameState.Play)
        {
            if (WakakaKocmoca.instance.client.udp == SocketUDP.UDP.Client)
                nowStage.Navigation();
            else
            {
                nextWaypoint.gameObject.SetActive(false);
                pathWaypoint.gameObject.SetActive(false);
            }

            if (Input.GetKeyDown(KeyCode.F))
                Pass();

            //if (Input.GetKeyUp(KeyCode.V))
            //{
            //    count++;
            //    if (count == 4)
            //        count = 0;

            //    if (count == 0)
            //    {

            //        camera1.enabled = true;
            //        camera2.enabled = false;
            //        camera3.enabled = false;
            //        camera4.enabled = false;

            //    }
            //    if (count == 1)
            //    {

            //        camera1.enabled = false;
            //        camera2.enabled = true;
            //        camera3.enabled = false;
            //        camera4.enabled = false;

            //    }
            //    if (count == 2)
            //    {

            //        camera1.enabled = false;
            //        camera2.enabled = false;
            //        camera3.enabled = true;
            //        camera4.enabled = false;

            //    }
            //    if (count == 3)
            //    {

            //        camera1.enabled = false;
            //        camera2.enabled = false;
            //        camera3.enabled = false;
            //        camera4.enabled = true;

            //    }
            //}
        }
    }
    public void Pass()
    {
        nowStage.pass = true;
        nowStage.Navigation();
        nextWaypoint.gameObject.SetActive(true);
        if (WakakaKocmoca.instance.role == WakakaKocmoca.Role.Controller)
        {
            string msg = "Controller/Pass/";
            WakakaKocmoca.instance.client.SocketSend(msg);
        }
    }
    void NextStage()
    {
        nowStage.boundarySet.SetActive(false);
        indexStage++;
        nowStage = listStage[indexStage];
        nowStage.boundarySet.SetActive(true);
        nextWaypoint.gameObject.SetActive(true);
        pathWaypoint.gameObject.SetActive(true);
    }
}