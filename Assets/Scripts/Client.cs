using System;
using UnityEngine;
using System.Collections;
using System.Net;
using System.Net.Sockets;
using System.Text;

public class Client : MonoBehaviour
{
    private Socket client;
    public byte[] byteData=new byte[1024];

    void Start()
    {
        Initail();
    }

    void Initail()
    {
        IPAddress ipAddress=IPAddress.Parse("127.0.0.1");
        IPEndPoint ipEnd=new IPEndPoint(ipAddress,18010);
        client=new Socket(AddressFamily.InterNetwork,SocketType.Stream, ProtocolType.Tcp);
        client.BeginConnect(ipEnd,ConnectCallBack,client);
    }

    public void ConnectCallBack(IAsyncResult ar)
    {
        client.EndReceive(ar);
    }

    void Update()
    {
        if (Input.GetMouseButton(0))
        {
            BeginSend("12345678");
        }
        BeginRecv();
    }

    #region Receive

    public void BeginRecv()
    {
        client.BeginReceive(byteData, 0, 1024, SocketFlags.None, RecvCallBack, this);

    }

    private void RecvCallBack(IAsyncResult ar)
    {
        int length = client.EndReceive(ar);
        string tmpStr = Encoding.Default.GetString(byteData, 0, length);
        Debug.Log("server recv ==" + tmpStr);
        BeginSend(tmpStr);
    }
    #endregion

    #region Send
    public void BeginSend(string tmpStr)
    {
        byte[] data = Encoding.Default.GetBytes(tmpStr);
        client.BeginSend(data, 0, data.Length, SocketFlags.None, SendCallBack, this);
    }

    private void SendCallBack(IAsyncResult ar)
    {
        int byteSend = client.EndSend(ar);
        Debug.Log("byte send count ==" + byteSend);
    }

    #endregion
    
}
