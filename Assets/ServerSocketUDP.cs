using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;

using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;

public class ServerSocketUDP : MonoBehaviour
{
    UdpClient server;
    private static string localIP;

    private string msgReceived;

    void GetLocalIP()
    {
        IPHostEntry IpEntry = Dns.GetHostEntry(Dns.GetHostName());
        foreach (IPAddress ipa in IpEntry.AddressList)
        {
            if (ipa.AddressFamily == AddressFamily.InterNetwork)
            {
                localIP = ipa.ToString();
                msgReceived = "取得本地端IP: " + localIP + "\n";
                Debug.Log(msgReceived);
            }
        }
    }

    public void Server()
    {
        server = new UdpClient(new IPEndPoint(IPAddress.Parse(localIP), 7000));
        new Thread(ReceiveMessage) { IsBackground = true }.Start();
    }

    private void ReceiveMessage()
    {
        while (true)
        {
            IPEndPoint remoteEndPoint = new IPEndPoint(IPAddress.Any, 0);
            byte[] data = server.Receive(ref remoteEndPoint);
            string message = Encoding.UTF8.GetString(data);
            Debug.Log(message);
        }
    }

    // Use this for initialization
    void Start ()
    {
        GetLocalIP();
    }
	
	// Update is called once per frame
	void Update () {
	
	}
}
