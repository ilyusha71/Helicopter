using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;

using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;

public class ClientSocketUDP : MonoBehaviour
{
    public InputField textIP;
    private IPEndPoint serverIP;
    UdpClient client;
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
                msgReceived = "Local IP " + localIP + "\n";
                Debug.Log("Client Socket UDP: " + msgReceived);
            }
        }
    }

    public void SetServerIP()
    {
        if (textIP.text == string.Empty)
            textIP.text = localIP;
        serverIP = new IPEndPoint(IPAddress.Parse(textIP.text), 7000);
    }

    public void Client()
    {
        client = new UdpClient();
        Debug.Log("Client Socket UDP: 已設定本機為客戶端");
    }

    public void Send(string msg)
    {
        byte[] data = Encoding.UTF8.GetBytes(msg);
        client.Send(data,data.Length, serverIP);
    }

    // Use this for initialization
    void Start()
    {
        GetLocalIP();
    }

    // Update is called once per frame
    void Update () {
	
	}
}
