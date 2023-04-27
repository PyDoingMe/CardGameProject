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
        if (client.isGameStart)//클라이언트 게임시작
        {
            ddo.NextScene("MainGame");
            client.isGameStart = false;
        }
        if (client.clientReady && !clientConnect && client.isTry2Connect)//클라이언트 연결 성공
        {
            client.isTry2Connect = false;
            txt.text = "연결에 성공했습니다.";
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
            txt.text = "연결이 끊어졌습니다.";
            clientConnect = false;
        }
        if (!client.clientReady && client.isTry2Connect)
        {
            client.isTry2Connect = false;
            txt.text = "연결에 실패했습니다.";
            cancelButton.SetActive(true);
            connectButton.interactable = true;
            connectButton.gameObject.SetActive(false);
            backButton.interactable = true;
            backButton.gameObject.SetActive(false);
            clock.SetActive(false);
        }
        if (server.ConnectedClients.Count > 0 && !serverConnect)//서버 연결 성공
        {
            serverConnect = true;
            child.SetActive(true);
            image.enabled = true;
            button.enabled = true;
            clock.SetActive(false);
        }
        if(server.ConnectedClients.Count == 0 && serverConnect)//서버 연결 취소
        {
            serverConnect = false;
            child.SetActive(false);
            image.enabled = false;
            button.enabled = false;
            clock.SetActive(true);
        }
    }
}
