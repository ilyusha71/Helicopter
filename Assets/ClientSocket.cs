using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using System.Net;
using System.Net.Sockets;
using System.Threading;

public class ClientSocket : MonoBehaviour
{
    private IPEndPoint ipEndPoint;
    private Socket clientSocket;
    private Thread connectThread;
    private Thread receiveThread;
    private MessageProcessor msgProcessor;
    private string msgReceived;
    private string msgReceivedLast;
    private byte[] recieveBuffer = new byte[2048];
    private byte[] result = new byte[1024];
    private bool isConnected = false;
    private bool isReceived = false;
    private string ip;
    private int port;
    public string localIp;

    private List<String> msgPool = new List<String>();

    public int imsg;
    public int pmsg;

    public bool disconnected;

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
        //        localIp = ipa.ToString();
        //        msgReceived = "【本地端訊息】本地端IP: " + localIp + "\n";
        //        Debug.Log(msgReceived);
        //    }
        //}
    }
    void Update()
    {
        while (msgPool.Count > 0)
        {
            pmsg++;
            msgProcessor.GamingMsgProcessor(msgPool[0]);
            msgPool.RemoveAt(0);
        }
        //if (imsg != pmsg)
        //{
        //    Debug.LogError("STOP: imsg:  " + imsg + " pmsg: " + pmsg);
        //}
        //if (msgReceived != msgReceivedLast)
        //{
        //    msgProcessor.GamingMsgProcessor(msgReceived);
        //    msgReceivedLast = msgReceived;
        //}
    }
    /* Connet Server */
    #region Connet Server
    public void TCPClient(string ip, int port)
    {
        this.ip = ip;
        this.port = port;
        // Initialize IP endpoint
        this.ipEndPoint = new IPEndPoint(IPAddress.Parse(this.ip), this.port);
        // Initialize client socket
        clientSocket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Udp);
    }
    public void Connect()
    {
        // Server connect thread
        connectThread = new Thread(this.ConnectToServer);
        connectThread.IsBackground = true;
        connectThread.Start();
    }
    private void ConnectToServer()
    {
        while (!isConnected)
        {
            try
            {
                clientSocket.Connect(this.ipEndPoint);
                this.isConnected = true;
            }
            catch (Exception e)
            {
                msgReceived = string.Format("【本地端訊息】因為一個錯誤的發生，暫時無法連接到伺服器，錯誤信息為:{0}", e.Message);
                Debug.Log(msgReceived);
                this.isConnected = false;
            }
            if (!isConnected)
            {
                Thread.Sleep(3000);
                msgReceived = "【本地端訊息】正在嘗試重新連接...";
                Debug.Log(msgReceived);
            }
        }

        msgReceived = "【本地端訊息】連接伺服器成功，現在可以和伺服器進行會話了";
        //Debug.Log(msgReceived);
        WakakaKocmoca.instance.gameState = WakakaKocmoca.GameState.Play;

        // Receive message thread
        receiveThread = new Thread(this.ReceiveMessage);
        receiveThread.IsBackground = true;

        receiveThread.Start();
    }
    #endregion

    private void ReceiveMessage()
    {
        isReceived = true;
        while (isReceived)
        {
            try
            {
                //获取数据长度
                int receiveLength = this.clientSocket.Receive(result);
                //获取服务器消息
                string serverMessage = Encoding.UTF8.GetString(result, 0, receiveLength);
                //输出服务器消息
                // Server@....
                // Client@.../...
                string[] s = serverMessage.Split('@');
                //Debug.Log("REV: " + serverMessage);

                //if (s.Length > 3)
                //    Debug.LogWarning("Multi REV: " + serverMessage);

                for (int i = 1; i < s.Length; i+=2)
                {
                    if (s[i-1] == "Server")
                        msgReceived = "【伺服端訊息】" + s[1];
                    else if (s[i-1] == "Client")
                    {
                        msgReceived = s[i];
                        string[] sss = s[i].Split('/');
                        //if (sss[0] == "Launcher")
                        //    Debug.LogWarning("LA: " + s[i]);

                        //if (s.Length > 3)
                        //    Debug.LogWarning("Part REV: " + s[i]);
                    }
                    imsg++;
                    msgPool.Add(msgReceived);
                }
            }
            catch (Exception e)
            {
                //停止消息接收
                isReceived = false;
                //断开服务器
                this.clientSocket.Shutdown(SocketShutdown.Both);
                //关闭套接字
                this.clientSocket.Close();

                //重新尝试连接服务器
                this.isConnected = false;
                Debug.LogWarning("DISCONNECTED");
                disconnected = true;
                OnApplicationQuit();
            }
        }
    }

    //public void Send()
    //{
    //    if (msgProcessor.msgInput.text == string.Empty || this.clientSocket == null)
    //        return;
    //    clientSocket.Send(Encoding.UTF8.GetBytes(msgProcessor.msgInput.text));
    //}
    //public void Send(string gamingMsg)
    //{
    //    byte[] byteArray = System.Text.Encoding.UTF8.GetBytes(gamingMsg);
    //    SendData(byteArray);
    //    //clientSocket.Send(Encoding.UTF8.GetBytes(gamingMsg));
    //}
    //private void SendData(byte[] data)
    //{
    //    SocketAsyncEventArgs socketAsyncData = new SocketAsyncEventArgs();
    //    socketAsyncData.SetBuffer(data, 0, data.Length);
    //    clientSocket.SendAsync(socketAsyncData);
    //}

    void OnApplicationQuit()
    {
        //connectThread.Abort();
        //if (isReceived)
        //    receiveThread.Abort();
        //if (clientSocket.Connected)
        //{
        //    clientSocket.Shutdown(SocketShutdown.Both);
        //    clientSocket.Close();
        //}
    }




    /// <summary>
    /// 發送到 Server & Receive
    /// </summary>
    public void SendServer(string gamingMsg)
    {
        try
        {
            byte[] byteArray = System.Text.Encoding.UTF8.GetBytes(gamingMsg);
            SendData(byteArray);
        }
        catch (SocketException ex)
        {
            Debug.LogWarning(ex.Message);
        }
        //clientSocket.BeginReceive(recieveBuffer, 0, recieveBuffer.Length, SocketFlags.None, new AsyncCallback(ReceiveCallback), null);
    }
    /// <summary>
    /// 發送 Data Server 
    /// </summary>
    private void SendData(byte[] data)
    {
        SocketAsyncEventArgs socketAsyncData = new SocketAsyncEventArgs();
        socketAsyncData.SetBuffer(data, 0, data.Length);
        clientSocket.SendAsync(socketAsyncData);
    }
    /// <summary>
    /// Receives the callback.
    /// </summary>
    private void ReceiveCallback(IAsyncResult AR)
    {
        int recieved = clientSocket.EndReceive(AR);

        Debug.Log("ReceiveCallback - recieved: " + recieved + " bytes");

        if (recieved <= 0)
            return;

        byte[] recData = new byte[recieved];
        Buffer.BlockCopy(recieveBuffer, 0, recData, 0, recieved);

        string recvStr = Encoding.UTF8.GetString(recData, 0, recieved);
        Debug.Log("recvStr: " + recvStr);

        clientSocket.BeginReceive(recieveBuffer, 0, recieveBuffer.Length, SocketFlags.None, new AsyncCallback(ReceiveCallback), null);
    }
}
