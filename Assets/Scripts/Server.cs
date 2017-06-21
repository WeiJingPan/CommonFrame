using System;
using UnityEngine;
using System.Collections;
using System.Net;
using System.Net.Sockets;
using System.Threading;
using System.Collections.Generic;
using System.Text;

public class SocketState
{
    public Socket socket;
    public byte[] buffer;

    public SocketState(Socket tmpSocket)
    {
        buffer=new byte[1024];
        socket = tmpSocket;
    }
    #region Receive

    public void BeginRecv()
    {
        socket.BeginReceive(buffer, 0, 1024, SocketFlags.None, RecvCallBack, this);

    }

    private void RecvCallBack(IAsyncResult ar)
    {
        int length = socket.EndReceive(ar);
        string tmpStr = Encoding.Default.GetString(buffer, 0, length);
        Debug.Log("server recv ==" + tmpStr);
        BeginSend(tmpStr);
    } 
    #endregion
    #region Send
    
    #endregion
    public void BeginSend(string tmpStr)
    {
        byte[] data = Encoding.Default.GetBytes(tmpStr);
        socket.BeginSend(data, 0, data.Length, SocketFlags.None, SendCallBack, this);
    }

    private void SendCallBack(IAsyncResult ar)
    {
        int byteSend = socket.EndSend(ar);
        Debug.Log("byte send count =="+byteSend);
    }
}

public class Server : MonoBehaviour
{
    private Socket serverSocket;

    #region AcceptClient

    private bool isRunning = true;
    
    #endregion
    void Start()
    {
        InitailSocket();
    }
    private List<SocketState> socketArr;
    public void InitailSocket()
    {
        IPEndPoint endPoint=new IPEndPoint(IPAddress.Any,18010);
        serverSocket=new Socket(AddressFamily.InterNetwork,SocketType.Stream,ProtocolType.Tcp);
        serverSocket.Bind(endPoint);
        serverSocket.Listen(100);

        socketArr=new List<SocketState>();
        Thread tmpThread=new Thread(ListenRecv);
        tmpThread.Start();
    }
    private void ListenRecv()
    {
        while (isRunning)
        {
            try
            {
                serverSocket.BeginAccept(new AsyncCallback(AsyncAcceptCallBack), serverSocket);
            }
            catch (Exception e)
            {
                
            }
            Thread.Sleep(1000);
        }
    }
    private void AsyncAcceptCallBack(IAsyncResult ar)
    {
        Socket listener = (Socket)ar.AsyncState;
        Socket clientSocket = listener.EndAccept(ar);
        SocketState socketState = new SocketState(clientSocket);
        socketArr.Add(socketState);
    }
    void OnApplicationQuit()
    {
        serverSocket.Shutdown(SocketShutdown.Both);
        serverSocket.Close();
    }

    void Update()
    {
        if (socketArr.Count>0)
        {
            for (int i = 0; i < socketArr.Count; i++)
            {
                socketArr[i].BeginRecv();
            }
        }
    }
}
