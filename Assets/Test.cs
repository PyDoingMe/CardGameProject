using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Test : MonoBehaviour
{
    public DeckManager deckManager;
    // Start is called before the first frame update
    void Start()
    {
        Debug.Log(transform.rotation);
        for (int i = 0; i < 30; i++)
        {
            deckManager.AddCard(i);
        }
        deckManager.SaveDeck();
        TurnManager.Init();
        //StartCoroutine(TurnManager.StartGameCo());
    }

    // Update is called once per frame
    void Update()
    {

        if (Input.GetKeyDown(KeyCode.Space))
        {
            StartCoroutine(TurnManager.StartTurnCo());
        }
    }
}
