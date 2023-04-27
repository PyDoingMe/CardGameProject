using System.Collections;
using UnityEngine;

public class GameDirector : MonoBehaviour
{
    [SerializeField] FieldManager fieldManager;

    NetworkManager_Server server;
    NetworkManager_Client client;
    bool isServ;

    private IEnumerator Start()
    {
        var i = GameObject.Find("DDO");
        server = i.GetComponentInChildren<NetworkManager_Server>();
        client = i.GetComponentInChildren<NetworkManager_Client>();
        isServ = server.serverReady;
        yield return StartCoroutine(TurnManager.StartGameCo());
        if (isServ)
        {
            if(Random.value > 0.5f)
            {
                TurnEnd();
            }
            else
            {
                StartCoroutine(TurnManager.StartTurnCo());
            }
        }
    }

    private void Update()
    {
        if (isServ)
        {
            if (server.receiveCommandQueue.Count > 0)
            {
                var i = server.receiveCommandQueue.Dequeue();
                switch (i.MsgID)
                {
                    case 10:
                        fieldManager.EnemyParticipate(i.int1, i.int2);
                        break;
                    case 11:
                        fieldManager.EnemyAttack(i.int1, i.int2);
                        break;
                    default:
                        break;
                }
            }
            if (server.IsTurnStart)
            {
                StartCoroutine(TurnManager.StartTurnCo());
                server.IsTurnStart = false;
            }
        }
        else
        {
            if(client.receiveCommandQueue.Count > 0)
            {
                var i = client.receiveCommandQueue.Dequeue();
                switch (i.MsgID)
                {
                    case 10:
                        fieldManager.EnemyParticipate(i.int1, i.int2);
                        break;
                    case 11:
                        fieldManager.EnemyAttack(i.int1, i.int2);
                        break;
                    default:
                        break;
                }
            }
            if (client.IsTurnStart)
            {
                StartCoroutine(TurnManager.StartTurnCo());
                client.IsTurnStart = false;
            }
        }
    }

    /// <summary>
    /// msgID) 10:출전, 11:공격
    /// </summary>
    /// <param name="msgID"></param>
    /// <param name="index"></param>
    /// <param name="card"></param>
    public void SendTwoIntMsg(ushort msgID , int index, int card)
    {
        if (isServ)
        {
            server.SendTwoIntMsg(msgID, index, card);
        }
        else
        {
            client.SendTwoIntMsg(msgID, index, card);
        }
    }

    public void TurnEnd()
    {
        if (isServ)
        {
            server.SendHeaderMsg();
        }
        else
        {
            client.SendHeaderMsg();
        }
    }
}
