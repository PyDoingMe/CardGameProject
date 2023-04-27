using RengeGames.HealthBars;
using System;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using Utility;

public class Hand : MonoBehaviour
{
    [SerializeField] Deck deck;
    [SerializeField] Field field;
    [SerializeField] Transform canvas;
    [SerializeField] GameObject card;
    [SerializeField] TMP_Text costUI;
    [SerializeField] CardStats cardStats;
    [SerializeField] FieldManager fieldManager;
    [Space(10f)]
    [SerializeField] Vector3 spawnPoint = new Vector3(1550, -280, -3);
    [SerializeField] float handSize = 400.0f;

    ECardState eCardState;
    List<Card> cards = new List<Card>();
    Card selectCard;
    bool isDrag = false;
    ushort cost;
    private bool onCardArea;

    private void Start()
    {
        TurnManager.OnAddCard += Drow;
        TurnManager.OnMyTurn += GetCost;
    }

    private void OnDestroy()
    {
        TurnManager.OnAddCard -= Drow;
        TurnManager.OnMyTurn -= GetCost;
    }

    private void Update()
    {
        if (isDrag)
        {
            CardDrag();
        }
        DetectCardArea();
        SetECardState();
    }

    public void Drow()
    {
        var instance = Instantiate(card, spawnPoint, Quaternion.identity);
        instance.transform.SetParent(canvas);
        cards.Add(instance.GetComponent<Card>());
        cards[cards.Count-1].init(deck.Drowed());
        AlignCard(true);
    }

    public void GetCost()
    {
        UpdateCost(TurnManager.turnCount);
    }

    private void UpdateCost(ushort us)
    {
        cost += us;
        costUI.text = cost.ToString() + " / " + TurnManager.turnCount.ToString();
        StatusBarsManager.SetPercent("Player", "Primary", (float)cost/8);
    }

    private void SelectTheCard(bool isEnlarge, Card card)
    {
        if (isEnlarge)
        {
            card.Movement3D(new PosRotSca(new Vector3(card.Prs.pos.x, 100f, -10f), Quaternion.identity, Vector3.one * 1.5f), false);
        }
        else
        {
            card.Movement3D(card.Prs, false);
        }
    }

    void AlignCard(bool flag)
    {
        float interval = 1f / (cards.Count + 1);

        for (int i = 0; i < cards.Count; i++)
        {
            cards[i].Prs = new PosRotSca(Vector3.Lerp(Vector3.left,Vector3.right,(1+i)*interval)*handSize+transform.position+(Vector3.up*(-200f*Mathf.Pow(interval*(i+1)-0.5f,2)+50f)), 
                Quaternion.Slerp(new Quaternion(0,0,0.1f,1),new Quaternion(0,0,-0.1f,1),interval*(i+1)), Vector3.one);
            cards[i].Movement3D(cards[i].Prs, true, 0.7f);
        }
    }

    #region MyCard

    public void CardMouseOver(Card card)
    {
        if (eCardState == ECardState.CanNothing) return;
        SelectTheCard(true, card);
        selectCard = card;
        cardStats.Show(card.item);
    }

    public void CardMouseExit(Card card)
    {
        SelectTheCard(false, card);
        cardStats.Close();
    }

    public void CardMouseDown()
    {
        if (eCardState != ECardState.CanMouseDrag) return;
        isDrag = true;
    }

    public void CardMouseUp()
    {
        if (eCardState != ECardState.CanMouseDrag)
        {
            isDrag = false;
            return;
        }
        if (!onCardArea && selectCard.item.cost <= cost)
        {
            if (fieldManager.Participate(selectCard))
            {
                UpdateCost((ushort)(-1*selectCard.item.cost));
                cards.Remove(selectCard);
                Destroy(selectCard.gameObject);
                AlignCard(true);
                cardStats.Close();
            }
        }
        isDrag = false;
    }

    void CardDrag()
    {
        if (!onCardArea)
        {
            selectCard.Movement3D(new PosRotSca(Constants.MousePos, Quaternion.identity, selectCard.Prs.sca), false);
        }
    }

    void DetectCardArea()
    {
        RaycastHit[] hits = Physics.RaycastAll(Camera.main.ScreenPointToRay(Input.mousePosition));
        int layer = LayerMask.NameToLayer("CardArea");
        onCardArea = Array.Exists(hits, x => x.collider.gameObject.layer == layer);
    }

    void SetECardState()
    {
        if (TurnManager.isLoading)
            eCardState = ECardState.CanNothing;
        else if (!TurnManager.myTurn)
        {
            eCardState = ECardState.CanMouseOver;
            if (cost != 0)
                UpdateCost((ushort)(cost * -1));
        }
        else if (TurnManager.myTurn)
            eCardState = ECardState.CanMouseDrag;
    }

    #endregion
}
