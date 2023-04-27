using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class StartButton : MonoBehaviour
{
    [SerializeField] NetworkManager_Server server;
    [SerializeField] NetworkManager_Client client;
    [SerializeField] TMP_Text txt;
    [SerializeField] DDO ddo;
    [SerializeField] GameObject child;
    [SerializeField] GameObject clock;
    [SerializeField] Image image;
    [SerializeField] Button button;
    [SerializeField] Button connectButton;
    [SerializeField] Button backButton;
    [SerializeField] GameObject cancelButton;
    bool serverConnect;
    public bool ServerConnect
    {
        set { serverConnect = value; }
    }

    bool clientConnect;
    public bool ClientConnect
    {
        set { clientConnect = value; }
    }

    private void Update()
    {
        if (client.isGameStart)//Ŭ���̾�Ʈ ���ӽ���
        {
            ddo.NextScene("MainGame");
            client.isGameStart = false;
        }
        if (client.clientReady && !clientConnect && client.isTry2Connect)//Ŭ���̾�Ʈ ���� ����
        {
            client.isTry2Connect = false;
            txt.text = "���ῡ �����߽��ϴ�.";
            cancelButton.SetActive(true);
            connectButton.interactable = true;
            connectButton.gameObject.SetActive(false);
            backButton.interactable = true;
            backButton.gameObject.SetActive(false);
            clientConnect = true;
            clock.SetActive(false);
        }
        if (clientConnect && !client.clientReady)
        {
            txt.text = "������ ���������ϴ�.";
            clientConnect = false;
        }
        if (!client.clientReady && client.isTry2Connect)
        {
            client.isTry2Connect = false;
            txt.text = "���ῡ �����߽��ϴ�.";
            cancelButton.SetActive(true);
            connectButton.interactable = true;
            connectButton.gameObject.SetActive(false);
            backButton.interactable = true;
            backButton.gameObject.SetActive(false);
            clock.SetActive(false);
        }
        if (server.ConnectedClients.Count > 0 && !serverConnect)//���� ���� ����
        {
            serverConnect = true;
            child.SetActive(true);
            image.enabled = true;
            button.enabled = true;
            clock.SetActive(false);
        }
        if(server.ConnectedClients.Count == 0 && serverConnect)//���� ���� ���
        {
            serverConnect = false;
            child.SetActive(false);
            image.enabled = false;
            button.enabled = false;
            clock.SetActive(true);
        }
    }
}
