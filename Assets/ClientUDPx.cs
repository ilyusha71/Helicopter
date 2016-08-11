using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;

public class ClientUDPx : MonoBehaviour
{

    private MessageProcessor msgProcessor;
    private List<String> msgPool = new List<String>();
    private string localIP;
    private string msgReceived;
    public int imsg;
    public int pmsg;

    string editString = "hello wolrd";

    Socket socket; // 目標socket
    EndPoint serverEnd; // 伺服端
    IPEndPoint ipEnd; // 伺服端端口
    string recvStr; // 接收的字串
    string sendStr; // 發送的字串
    byte[] recvData = new byte[1024]; // 接收的數據，必須為位元組
    byte[] sendData = new byte[1024]; // 發送的數據，必須為位元組
    int recvLen; // 接收的數據長度
    Thread connectThread; // 連接執行緒

    //初始化
    void InitSocket()
    {
        // 定義連接的伺服器ip和端口，可以是本機ip，區域網路，網際網路
        ipEnd = new IPEndPoint(IPAddress.Parse(localIP), 7000);
        // 定義socket类型，在主執行緒中定義
        socket = new Socket(AddressFamily.InterNetwork, SocketType.Dgram, ProtocolType.Udp);
        // 客戶端需要綁定端口
        IPEndPoint lpEnd = new IPEndPoint(IPAddress.Any, 6999);
        socket.Bind(lpEnd);
        // 定義伺服端
        IPEndPoint sender = new IPEndPoint(IPAddress.Any, 7000);
        serverEnd = (EndPoint)sender;
        print("waiting for sending UDP dgram");

        socket.EnableBroadcast = true;
        socket.SetSocketOption(SocketOptionLevel.Socket, SocketOptionName.Broadcast, true);

        // 建立初始連接，这句非常重要，第一次連接初始化了serverEnd后面才能收到消息
        SocketSend("Server, I'm coming");

        // 開啟一個執行緒連接，必須的，否則主執行緒卡死
        connectThread = new Thread(new ThreadStart(SocketReceive));
        connectThread.Start();
    }

    public void SocketSend(string sendStr)
    {
        // 清空發送快取
        sendData = new byte[1024];
        // 數據類型轉換
        sendData = Encoding.ASCII.GetBytes(sendStr);
        // 發送给指定伺服端
        socket.SendTo(sendData, sendData.Length, SocketFlags.None, ipEnd);
    }

    // 客戶端接收
    void SocketReceive()
    {
        // 進入接收迴圈
        while (true)
        {
            // 對data清零
            recvData = new byte[1024];
            // 獲取伺服端，獲取伺服端數據，用引用給伺服端賦值，實際上伺服端已經定義好並不需要賦值
            recvLen = socket.ReceiveFrom(recvData, ref serverEnd);
            //print("message from: " + serverEnd.ToString()); // 打印伺服端信息
            // 輸出接收到的數據
            recvStr = Encoding.ASCII.GetString(recvData, 0, recvLen);
            //print(recvStr);

            // 預備要處理的訊息
            msgPool.Add(recvStr);
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
        if (socket != null)
            socket.Close();
    }

    void Awake()
    {
        msgProcessor = FindObjectOfType<MessageProcessor>();
    }

    void Start()
    {
        //// 獲取本地端IP
        //IPHostEntry IpEntry = Dns.GetHostEntry(Dns.GetHostName());
        //foreach (IPAddress ipa in IpEntry.AddressList)
        //{
        //    if (ipa.AddressFamily == AddressFamily.InterNetwork)
        //    {
        //        localIP = ipa.ToString();
        //        msgReceived = "客戶端IP: " + localIP + "\n";
        //        Debug.Log(msgReceived);
        //    }
        //}
        //InitSocket(); //在這裡初始化
    }

    //void OnGUI()
    //{
    //    editString = GUI.TextField(new Rect(10, 10, 100, 20), editString);
    //    if (GUI.Button(new Rect(10, 30, 60, 20), "send"))
    //        SocketSend(editString);
    //}

    void Update()
    {
        while (msgPool.Count > 0)
        {
            pmsg++;
            msgProcessor.GamingMsgProcessor(msgPool[0]);
            msgPool.RemoveAt(0);
        }
    }

    void OnApplicationQuit()
    {
        SocketQuit();
    }
}
