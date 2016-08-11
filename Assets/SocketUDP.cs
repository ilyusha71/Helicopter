using UnityEngine;
using UnityEngine.UI;
using System;
using System.Collections;
using System.Collections.Generic;

using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;

public class SocketUDP : MonoBehaviour
{
    public enum UDP
    {
        None,
        Server,
        Client,
        Error,
    }
    public UDP udp;
    public bool localTest; // 若為客戶端，判定是否為本機測試
    public Text msg;
    public InputField textIP; // 若為客戶端，需設定伺服端IP

    UdpClient socketUDP;
    Thread connectThread; // 連接執行緒

    private IPEndPoint serverEndPoint; // 若為客戶端，需記錄伺服端IP與端口（固定7000）
    private IPEndPoint localEndPoint; // 若為客戶端，記錄本地IP與端口
    private List<int> clientPorts = new List<int>(); // 測試用，若為伺服端記錄客戶端IP與端口
    private static string localIP;
    private static int localPort;
    private string msgReceived;

    private MessageProcessor msgProcessor;
    private List<String> msgPool = new List<String>();

    void Awake()
    {
        msgProcessor = FindObjectOfType<MessageProcessor>();
    }
    void Start()
    {
        GetLocalIP();
    }

    void GetLocalIP()
    {
        IPHostEntry IpEntry = Dns.GetHostEntry(Dns.GetHostName());
        foreach (IPAddress ipa in IpEntry.AddressList)
        {
            if (ipa.AddressFamily == AddressFamily.InterNetwork)
            {
                localIP = ipa.ToString();
                msgReceived = "本地端 IP " + localIP + "\n";
                Debug.Log(msgReceived);
            }
        }
    }
    public void AsServer()
    {
        try
        {
            serverEndPoint = new IPEndPoint(IPAddress.Parse(localIP), 7000);
            socketUDP = new UdpClient(serverEndPoint);
            udp = UDP.Server;
            socketUDP.EnableBroadcast = true;
            WakakaKocmoca.instance.textLocalIP.text = localIP + " : 7000";
            msgReceived = "已設定本機為伺服端";
            Debug.Log(msgReceived);
            WakakaKocmoca.instance.clientCamera.SetActive(false);
            WakakaKocmoca.instance.serverCamera.SetActive(true);
            WakakaKocmoca.instance.gameState = WakakaKocmoca.GameState.Play;
            connectThread = new Thread(ReceiveMessage);
            connectThread.Start();
            InvokeRepeating("Broadcasting", 1,1);
        }
        catch (System.Exception e)
        {
            msgReceived = "伺服端設定錯誤：\n" + e.Message + "\n請重設";
            udp = UDP.Error;
        }
    }
    void Broadcasting()
    {
        Broadcast("Broadcasting");
    }
    public void IsLocal(bool local)
    {
        localTest = local;
    }
    public void AsClient()
    {
        if (!localTest)
        {
            // 區域網路，端口統一為7000
            try
            {
                localPort = 7000;
                socketUDP = new UdpClient(new IPEndPoint(IPAddress.Any, localPort));
                udp = UDP.Client;
            }
            catch (System.Exception e)
            {
                Debug.LogWarning(e.Message);
                msgReceived = "客戶端設定錯誤：\n" + e.Message + "\n尚未與伺服端連線，請重新設定";
                udp = UDP.Error;
            }
        }
        else
        {
            // 本機測試，必須事先指定端口，否則只能發送無法接收
            bool portCheck = false;
            while (!portCheck)
            {
                try
                {
                    localPort = UnityEngine.Random.Range(7001, 7700);
                    socketUDP = new UdpClient(new IPEndPoint(IPAddress.Any, localPort));
                    portCheck = true;
                    udp = UDP.Client;
                }
                catch (System.Exception e)
                {
                    Debug.LogWarning(e.Message);
                    msgReceived = "測試端設定錯誤：\n" + e.Message + "\n尚未與伺服端連線，請重新設定";
                    udp = UDP.Error;
                }
            }
        }
        if (udp == UDP.Client)
        {
            localEndPoint = new IPEndPoint(IPAddress.Parse(localIP), localPort);
            WakakaKocmoca.instance.textLocalIP.text = localIP + " : " + localPort;
            msgReceived = "Socket UDP: 已設定本機為客戶端";
            Debug.Log(msgReceived);
            WakakaKocmoca.instance.gameState = WakakaKocmoca.GameState.Play;
            if(connectThread != null)
                Debug.Log("XX: " + connectThread.IsAlive);
            connectThread = new Thread(ReceiveMessage);
            connectThread.Start();
            SetServerIP();
            SocketSend("Hello"); // 告知伺服器記錄客戶端IP與端口，測試用
        }
    }

    public void SetServerIP()
    {
        if (textIP.text == string.Empty)
            textIP.text = localIP;
        serverEndPoint = new IPEndPoint(IPAddress.Parse(textIP.text), 7000);
    }
    public void SocketSend(string msg)
    {
        byte[] data = Encoding.UTF8.GetBytes(msg);
        if (serverEndPoint != null)
            socketUDP.Send(data, data.Length, serverEndPoint);
    }
    public void Broadcast(string msg)
    {
        byte[] data = Encoding.UTF8.GetBytes(msg);

        // 區域網路
        socketUDP.Send(data, data.Length, new IPEndPoint(IPAddress.Broadcast, 7000));

        // 本機測試備用
        foreach (int s in clientPorts)
        {
            socketUDP.Send(data, data.Length, new IPEndPoint(IPAddress.Broadcast, s));
        }
    }
    private void ReceiveMessage()
    {
        while (true)
        {
            IPEndPoint remoteEndPoint = new IPEndPoint(IPAddress.Any, 0);
            byte[] data = socketUDP.Receive(ref remoteEndPoint);
            string message = Encoding.UTF8.GetString(data);
            //msgReceived = "Socket UDP: " + message + " / from: " + remoteEndPoint.Address + " : " + remoteEndPoint.Port;
            msgReceived = message;
            //Debug.Log(msgReceived);
            if (udp == UDP.Server)
            {
                // 不處理廣播資訊
                if (remoteEndPoint.ToString() != serverEndPoint.ToString())
                {
                    msgPool.Add(message);
                    // 本機測試用
                    if (!clientPorts.Contains(remoteEndPoint.Port))
                        clientPorts.Add(remoteEndPoint.Port);
                    Broadcast(message);
                }
            }
            else
            {
                serverEndPoint = remoteEndPoint;
                msgPool.Add(message);
            }
        }
    }

    void Update ()
    {
        // 更新伺服器IP與端口
        if (serverEndPoint != null)
        {
            WakakaKocmoca.instance.textServerIP.text = serverEndPoint.Address + " : " + serverEndPoint.Port;
            textIP.text = "" + serverEndPoint.Address;
        }

        if (udp == UDP.Client)
        {
            if (localEndPoint.ToString() == serverEndPoint.ToString())
            {
                Debug.Log(localEndPoint.ToString() + " /// " + serverEndPoint.ToString());
                msgReceived = "本地端設定與伺服端位置衝突\n請重設";
                udp = UDP.Error;
                WakakaKocmoca.instance.gameState = WakakaKocmoca.GameState.Initial;
                serverEndPoint = null;
                SocketQuit();
            }
        }

        // 訊息處理
        msg.text = msgReceived;
        while (msgPool.Count > 0)
        {
            msgProcessor.GamingMsgProcessor(msgPool[0]);
            msgPool.RemoveAt(0);
        }

        // UDP Socket 設定錯誤重設機制
        if (udp == UDP.Error)
        {
            WakakaKocmoca.instance.setting.SetActive(true);
            udp = UDP.None;
        }
    }

    // 連接關閉
    void SocketQuit()
    {
        // 關閉執行緒
        if (connectThread != null)
        {
            connectThread.Interrupt();
            connectThread.Abort();
        }
        // 最後關閉socket
        if (socketUDP != null)
            socketUDP.Close();
    }

    void OnApplicationQuit()
    {
        SocketQuit();
    }
}
