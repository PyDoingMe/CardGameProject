using System.Collections;
using UnityEngine;
using Utility;

public class TurnManager : Singleton<TurnManager>
{
    public static System.Action OnAddCard;
    public static System.Action OnMyTurn;
    public static bool isLoading;
    public static bool myTurn = false;
    public static ushort turnCount = 0;

    private static YourTurn yourTurn;
    private static GameDirector director;
    private static FadeOut fadeOut;

    public static void Init()
    {
        yourTurn = GameObject.Find("YTImage").GetComponent<YourTurn>();
        director = GameObject.Find("Field Canvas").GetComponent<GameDirector>();
        fadeOut = GameObject.Find("FadeOut").GetComponent<FadeOut>();
    }

    public static IEnumerator StartGameCo()
    {
        isLoading = true;
        for (int _ = 0; _ < Constants.MULLIGAN_COUNT; _++)
        {
            yield return new WaitForSeconds(0.2f);
            OnAddCard?.Invoke();
        }
        isLoading = false;
    }

    public static IEnumerator StartTurnCo()
    {
        isLoading = true;
        turnCount++;
        yourTurn.Show(".당신의 차례입니다.");
        yield return new WaitForSeconds(0.4f);
        OnAddCard?.Invoke();
        yield return new WaitForSeconds(0.4f);
        isLoading = false;
        myTurn = true;
        OnMyTurn?.Invoke();
    }

    public static void EndTurn()
    {
        if (isLoading)
            return;
        director.TurnEnd();
        myTurn = false;
    }

    public static void EndGame(string msg)
    {
        myTurn = false;
        isLoading = true;
        yourTurn.Show(msg);
        fadeOut.Show();
    }
}
