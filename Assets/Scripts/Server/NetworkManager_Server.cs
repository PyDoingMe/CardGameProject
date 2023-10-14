using System.Collections.Generic;
using UnityEngine;
using System;
using System.Net;
using System.Net.Sockets;
using System.Threading;
using UnityEngine.UI;
using System.Runtime.InteropServices;//�������� ���� �����
using Utility;
using static Utility.Constants;

public class NetworkManager_Server : MonoBehaviour
{
    //������
    private Thread tcpListenerThread;
    //������ ����
    private TcpListener tcpListener;
    //Ŭ���̾�Ʈ
    //private ServerClient client;

    // Ŭ���̾�Ʈ ���
    public List<ServerClient> ConnectedClients; // ����� Ŭ���̾�Ʈ ���
    private List<ServerClient> disconnectedClients;  // ���� ������ Ŭ���̾�Ʈ ���

    //������, ��Ʈ
    private int port = 9001;

    //���� ����
    public bool serverReady;
    //��� �޽��� �а� ���� ����
    //private NetworkStream stream;

    //�α�
    public Text ServerLog;//ui
    private List<string> logList;//data

    //���� �޽���
    public InputField Text_Input;


    

    //���� ������ ó�� ����
    private Queue<stTwoIntMsg> receive_changeInfo_MSG;
    public Queue<stTwoIntMsg> receiveCommandQueue;
    private bool isTurnStart = false;
    public bool IsTurnStart
    {
        get { return isTurnStart; }
        set { isTurnStart = value; }
    }

    //������ �޽��� �������
    byte[] sendMessage;

    

    // Start is called before the first frame update
    void Start()
    {
        //�α� �ʱ�ȭ
        logList = new List<string>();


        //Ŭ���̾�Ʈ ��� �ʱ�ȭ
        ConnectedClients = new List<ServerClient>();
        disconnectedClients = new List<ServerClient>();



        receive_changeInfo_MSG = new Queue<stTwoIntMsg>();
        receiveCommandQueue = new Queue<stTwoIntMsg>();
        //������ �޽��� �ʱ�ȭ
        sendMessage = new byte[1024];
    }

    // Update is called once per frame
    void Update()
    {
        //���� �����Ͱ� �ִ°��(�� ���� Ȯ��)
        if (receive_changeInfo_MSG.Count > 0)
        {
            //���ʴ�� �̾Ƴ���.
            stTwoIntMsg CreateObjMsg = receive_changeInfo_MSG.Dequeue();


        }

        //�α׸���Ʈ�� �׿��ٸ�
        if (logList.Count > 0)
        {
            //�α� ���
            WriteLog(logList[0]);
            logList.RemoveAt(0);
        }

    }

    /// <summary>
    /// ���� ���� ��ư
    /// </summary>
    public void ServerCreate()
    {
        //ip, port ����
        //port = int.Parse(GameObject.Find("Text_Port").GetComponent<InputField>().text);

        // TCP���� ��� ������ ����
        tcpListenerThread = new Thread(new ThreadStart(ListenForIncommingRequeset));
        tcpListenerThread.IsBackground = true;
        tcpListenerThread.Start();
    }

    /// <summary>
    /// ���� ������ ����
    /// </summary>
    private void ListenForIncommingRequeset()
    {
        try
        {
            // ���� ����
            tcpListener = new TcpListener(IPAddress.Any/*������ ���� ������ IP*/, port);
            tcpListener.Start();

            // ���� ���� ON
            serverReady = true;

            // �α� ���
            logList.Add("[�ý���] ���� ����(port:" + port + ")");

            // ������ ���ú� �׽� ���(Update)
            while (true)
            {
                // ������ ������ ���ٸ�
                if(!serverReady)
                    break;

                //���� �õ����� Ŭ���̾�Ʈ Ȯ��
                if(tcpListener != null && tcpListener.Pending())
                {

                    ConnectedClients.Add( new ServerClient(tcpListener.AcceptTcpClient()));

                    BroadCast(" ����!");


                    stHeader stHeaderTmp = new stHeader();

                    stHeaderTmp.MsgID = 3;
                    stHeaderTmp.PacketSize = (ushort)Marshal.SizeOf(stHeaderTmp);//�޽��� ũ��

                    byte[] SendData = GetHeaderToByte(stHeaderTmp);

                    ConnectedClients[0].stream.Write(SendData, 0, SendData.Length);
                    ConnectedClients[0].stream.Flush();

                }

                //���ӵ� Ŭ���̾�Ʈ ����� ��ȣ�ۿ� ó��
                foreach(var Client in ConnectedClients)
                {
                    ServerClient client = Client;

                    if (client != null)
                    {
                        //Ŭ���̾�Ʈ ���� �����
                        if (!IsConnected(client.clientSocket))
                        {
                            
                            // �̰����� �ٷ� Ŭ���̾�Ʈ�� �����ϸ� �����尣�� ������ ���̷� ������ �߻������� ���������� Ŭ���̾�Ʈ ������� ����
                            //logList.Add("[�ý���] Ŭ���̾�Ʈ ���� ����");
                            
                            // ���������� Ŭ���̾�Ʈ ��Ͽ� �߰�
                            disconnectedClients.Add(client);

                            continue;
                        }
                        //Ŭ���̾�Ʈ �޽��� ó��
                        else
                        {
                            //�޽����� ���Դٸ�
                            if (client.stream.DataAvailable)
                            {
                                //�޽��� ���� ���� �ʱ�ȭ
                                Array.Clear(client.buffer, 0, client.buffer.Length);

                                //�޽����� �д´�.
                                int messageLength = client.stream.Read(client.buffer, 0, client.buffer.Length);

                                //���� ó���ϴ� ����
                                byte[] pocessBuffer = new byte[messageLength + client.nTempByteSize];//���� �о�� �޽����� ���� �޽����� ����� ���ؼ� ó���� ���� ����
                                                                                              //���Ҵ� �޽����� �ִٸ�
                                if (client.isTempByte)
                                {
                                    //�� �κп� ���Ҵ� �޽��� ����
                                    Array.Copy(client.tempBuffer, 0, pocessBuffer, 0, client.nTempByteSize);
                                    //���� ���� �޽��� ����
                                    Array.Copy(client.buffer, 0, pocessBuffer, client.nTempByteSize, messageLength);
                                }
                                else
                                {
                                    //���Ҵ� �޽����� ������ ���� �о�� �޽����� ����
                                    Array.Copy(client.buffer, 0, pocessBuffer, 0, messageLength);
                                }

                                //ó���ؾ� �ϴ� �޽����� ���̰� 0�� �ƴ϶��
                                if (client.nTempByteSize + messageLength > 0)
                                {
                                    //���� �޽��� ó��
                                    OnIncomingData(client, pocessBuffer);
                                }
                            }
                            else if (client.nTempByteSize > 0)
                            {
                                byte[] pocessBuffer = new byte[client.nTempByteSize];
                                Array.Copy(client.tempBuffer, 0, pocessBuffer, 0, client.nTempByteSize);
                                OnIncomingData(client, pocessBuffer);
                            }
                        }
                    }
                }
                
                //���� ������ Ŭ���̾�Ʈ ��� ó��
                for(int i = disconnectedClients.Count-1; i >= 0; i--)
                {
                    //�αױ��
                    logList.Add("[�ý���]Ŭ���̾�Ʈ ���� ����");
                    //���ӵ� Ŭ���̾�Ʈ ��Ͽ��� ����
                    ConnectedClients.Remove(disconnectedClients[i]);
                    // ó���� ���������� Ŭ���̾�Ʈ ��Ͽ��� ����
                    disconnectedClients.Remove(disconnectedClients[i]);
                }
                
                //����� Ŭ���̾�Ʈ ���(connectedClients)�� �߰��� �Ǿ� foreach���� Ÿ�� ������ ������ �ȵ��� client�� null�� �Ǵ� ������ �߻��Ͽ� �����̸� �ش�
                Thread.Sleep(10);
            }
        }
        catch (SocketException socketException)
        {
            Debug.Log("SocketException " + socketException.ToString());
        }
    }

    /// <summary>
    /// Ŭ���̾�Ʈ ���� Ȯ��
    /// </summary>
    /// <param name="client"></param>
    /// <returns></returns>
    private bool IsConnected(TcpClient client)
    {
        try
        {
            if(client != null && client.Client != null && client.Client.Connected)
            {
                if(client.Client.Poll(0, SelectMode.SelectRead))
                {
                    return !(client.Client.Receive(new byte[1], SocketFlags.Peek) == 0);
                }
                else
                {
                    return true;
                }
            }
            else
            {
                return false;
            }
        }
        catch
        {
            return false;
        }
    }

    /// <summary>
    /// ���� �޽��� ó��
    /// </summary>
    /// <param name="client"></param>
    /// <param name="data"></param>
    private void OnIncomingData(ServerClient client, byte[] data)
    {

        // �������� ũ�Ⱑ ����� ũ�⺸�ٵ� ������
        if (data.Length < Constants.HEADER_SIZE)
        {
            Array.Copy(data, 0, client.tempBuffer, client.nTempByteSize, data.Length);     // ���� ���� ���ۿ� ���� �޽��� ����
            client.isTempByte = true;
            client.nTempByteSize += data.Length;
            return;
        }

        //����κ� �߶󳻱�(�����ϱ�)
        byte[] headerDataByte = new byte[Constants.HEADER_SIZE];
        Array.Copy(data, 0, headerDataByte, 0, headerDataByte.Length); //��� ������ ��ŭ ������ ����
        //��� ������ ����üȭ(������)
        stHeader headerData = HeaderfromByte(headerDataByte);

        // ����� ������� ���� �޽����� ����� ������
        if (headerData.PacketSize > data.Length)
        {
            Array.Copy(data, 0, client.tempBuffer, client.nTempByteSize, data.Length);     // ���� ���� ���ۿ� ���� �޽��� ����
            client.isTempByte = true;
            client.nTempByteSize += data.Length;
            return;
        }

        //����� �޽���ũ�⸸ŭ�� �޽��� �����ϱ�
        byte[] msgData = new byte[headerData.PacketSize]; //��Ŷ �и��� ���� ���� ���� ����� ��Ŷ �����ŭ ���� ����
        Array.Copy(data, 0, msgData, 0, headerData.PacketSize); //������ ���ۿ� ��Ŷ ���� ����

        //����� �޽�����
        if (headerData.MsgID == 0)//�� ���� Ȯ��
        {
        }
        else if(headerData.MsgID == 1)//�� ���� ����
        {
            stTwoIntMsg stChangeInfoMsg1 = TwoIntfromByte(msgData);
            receive_changeInfo_MSG.Enqueue(stChangeInfoMsg1);

        }
        else if (headerData.MsgID == 2)//�޽���
        {
            stSendMsg SendMsgInfo = SendMsgfromByte(msgData);
            logList.Add(SendMsgInfo.strSendMsg);
            BroadCastByte(msgData);
        }
        else if(headerData.MsgID == 10)
        {
            stTwoIntMsg twoIntMsg = TwoIntfromByte(msgData);
            receiveCommandQueue.Enqueue(twoIntMsg);
        }
        else if (headerData.MsgID == 11)//����
        {
            stTwoIntMsg twoIntMsg = TwoIntfromByte(msgData);
            receiveCommandQueue.Enqueue(twoIntMsg);
        }
        else if (headerData.MsgID == 12)//������
        {
            isTurnStart = true;
        }
        else//�ĺ����� ���� ID
        {

        }

        // ��� �޽����� ó���Ǽ� ���� �޽����� ���� ��� 
        if (data.Length == msgData.Length)
        {
            client.isTempByte = false;
            client.nTempByteSize = 0;
        }
        // �޽��� ó�� �� �޽����� �����ִ� ���
        else
        {
            //�ӽ� ���� û��
            Array.Clear(client.tempBuffer, 0, client.tempBuffer.Length);

            //������ ���ۿ� ��Ŷ ���� ����
            Array.Copy(data, msgData.Length, client.tempBuffer, 0, data.Length - (msgData.Length));// �ӽ� ���� ���ۿ� ���� �޽��� ����
            client.isTempByte = true;
            client.nTempByteSize += data.Length - (msgData.Length);
        }
    }

    public void BroadCastByte(byte[] data)
    {
        foreach(var client in ConnectedClients)
        {
            client.stream.Write(data,0, data.Length);
            client.stream.Flush();
        }
    }


    /// <summary>
    /// �α� ����
    /// </summary>
    /// <param name="message"></param>
    public void WriteLog(/*Time*/string message)
    {
        ServerLog.GetComponent<Text>().text += message + "\n";
    }

    public void SendTwoIntMsg(ushort msgID, int index, int card)
    {
        stTwoIntMsg twoIntMsg = new stTwoIntMsg();
        twoIntMsg.MsgID = msgID;
        twoIntMsg.PacketSize = (ushort)Marshal.SizeOf(twoIntMsg);
        twoIntMsg.int1 = (ushort)index;
        twoIntMsg.int2 = (ushort)card;

        byte[] data = GetTwoIntMsgToByte(twoIntMsg);
        BroadCastByte(data);
    }

    public void SendHeaderMsg()
    {
        stHeader header = new stHeader();
        header.MsgID = 12;
        header.PacketSize = (ushort)Marshal.SizeOf(header);

        BroadCastByte(GetHeaderToByte(header));
    }

    public void SendMsg()
    {
        if (!serverReady)
            return;

        // ���� ���� ����ü �ʱ�ȭ
        stSendMsg stSendMsgInfo = new stSendMsg();

        string strSendMsg = Text_Input.text;

        //�޽��� �ۼ�
        stSendMsgInfo.MsgID = 2;//�޽��� ID
        stSendMsgInfo.PacketSize = (ushort)Marshal.SizeOf(stSendMsgInfo);//�޽��� ũ��
        stSendMsgInfo.strSendMsg = strSendMsg;

        //����ü ����Ʈȭ �� ����
        byte[] SendData = GetSendMsgToByte(stSendMsgInfo);

        bool bCheckSend = false;
        foreach (var client in ConnectedClients)
        {
            client.stream.Write(SendData, 0, SendData.Length);
            client.stream.Flush();
            bCheckSend = true;
        }
        //�α� ���
        if(bCheckSend)
            logList.Add("���� : " + strSendMsg);
    }


    /// <summary>
    /// �޽��� ����
    /// </summary>
    public void Send(ServerClient client, string message = "")
    {
        //������ �����°� �ƴ϶��
        if (!serverReady)
            return;

        //������ �ƴѰ�� �Է��� �ؽ�Ʈ ����
        if(message == "")
        {
            // ���� ���� ����ü �ʱ�ȭ
            stSendMsg stSendMsgInfo = new stSendMsg();

            string strSendMsg = Text_Input.text;

            //�޽��� �ۼ�
            stSendMsgInfo.MsgID = 2;//�޽��� ID
            stSendMsgInfo.PacketSize = (ushort)Marshal.SizeOf(stSendMsgInfo);//�޽��� ũ��
            stSendMsgInfo.strSendMsg = strSendMsg;

            //����ü ����Ʈȭ �� ����
            byte[] SendData = GetSendMsgToByte(stSendMsgInfo);

            client.stream.Write(SendData, 0, SendData.Length);
            client.stream.Flush();

            //�α� ���
            logList.Add("���� : " + strSendMsg);

        }
        else
        {
            try
            {

                stSendMsg stSendMsgInfo = new stSendMsg();

                stSendMsgInfo.MsgID = 2;//�޽��� ID
                stSendMsgInfo.PacketSize = (ushort)Marshal.SizeOf(stSendMsgInfo);//�޽��� ũ��
                stSendMsgInfo.strSendMsg = message;

                //����ü ����Ʈȭ �� ����
                byte[] sendMessageByte = GetSendMsgToByte(stSendMsgInfo);

                //����
                client.stream.Write(sendMessageByte, 0, sendMessageByte.Length);
                client.stream.Flush();

                //�α� ���
                logList.Add("���� : " + message);
            }
            catch (Exception e)
            {
                Debug.Log("SendException " + e.ToString());
            }
        }

        
    }   

    public void StartGame()
    {
        stHeader header = new stHeader();
        header.MsgID = 9;
        header.PacketSize = (ushort)Marshal.SizeOf(header);

        BroadCastByte(GetHeaderToByte(header));
    }

    /// <summary>
    /// ���� �ݱ�
    /// </summary>
    public void CloseSocket()
    {
        //������ ������ ���ٸ�
        if (!serverReady)
        {
            return;
        }
        else//�ʱ�ȭ
        {
            //Ŭ���̾�Ʈ���� ���� ���� ����
            BroadCast("���� ����!");

            
            //���� ���� �� �ʱ�ȭ
            if (tcpListener != null) { tcpListener.Stop(); tcpListener = null; }

            //���� �ʱ�ȭ
            serverReady = false;

            //������ �ʱ�ȭ
            tcpListenerThread.Abort();
            tcpListenerThread = null;

            //����� Ŭ���̾�Ʈ �ʱ�ȭ
            foreach (var client in ConnectedClients)
            {
                client.stream = null;
                client.clientSocket.Close();
            }
            ConnectedClients.Clear();
        }
    }

    public void BroadCast(string message)
    {
        foreach (var client in ConnectedClients)
        {
            Send(client, message);
        }
    }

    /// <summary>
    /// ���� �����
    /// </summary>
    private void OnApplicationQuit()
    {
        CloseSocket();
    }
}

/// <summary>
/// Ŭ���̾�Ʈ Ŭ����
/// </summary>
public class ServerClient
{
    public TcpClient clientSocket;//Ŭ���̾�Ʈ ����(��� ����)
    public NetworkStream stream; // Ŭ���̾�Ʈ ��� ����

    //���� ������ �������
    public byte[] buffer;
    //���� �����Ͱ� �߸� ��츦 ����Ͽ� �ӽù��ۿ� �����Ͽ� ����
    public byte[] tempBuffer;//�ӽù���
    public bool isTempByte;//�ӽù��� ����
    public int nTempByteSize;//�ӽù����� ũ��


    public ServerClient(TcpClient clientSocket)
    {
        this.clientSocket = clientSocket;
        this.stream = clientSocket.GetStream();

        //������ ������� �ʱ�ȭ
        this.buffer = new byte[1024];
        //�ӽù��� �ʱ�ȭ
        this.tempBuffer = new byte[1024];
        this.isTempByte = false;
        this.nTempByteSize = 0;
    }
}