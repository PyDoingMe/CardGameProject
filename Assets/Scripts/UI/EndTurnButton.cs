using UnityEngine;
using UnityEngine.UI;

public class EndTurnButton : MonoBehaviour
{
    void Start()
    {
        TurnManager.OnMyTurn += Setup;
    }

    private void OnDestroy()
    {
        TurnManager.OnMyTurn -= Setup;
    }

    public void Setup()
    {
        GetComponent<Button>().interactable = true;
    }

    public void EndTurn()
    {
        GetComponent<Button>().interactable = false;
        TurnManager.EndTurn();
    }
}
