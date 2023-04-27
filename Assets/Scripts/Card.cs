using Utility;
using UnityEngine;
using TMPro;
using UnityEngine.UI;
using DG.Tweening;

public class Card : MonoBehaviour
{
    [SerializeField] Image cardFrame;
    [SerializeField] Image cardImage;
    [SerializeField] TMP_Text costText;
    [SerializeField] TMP_Text cardNameText;
    [SerializeField] TMP_Text attackPointText;
    [SerializeField] TMP_Text healthPointText;
    [SerializeField] TMP_Text cardAbilityText;
    [SerializeField] TMP_Text cardExplantionText;
    public bool isDrop = false;
    Hand hand;

    PosRotSca prs;
    public PosRotSca Prs
    {
        get { return prs; }
        set { prs = value; }
    }
    public Item item;

    public void init(Item i)
    {
        item = i;
        cardImage.sprite = SpriteManager.Get(item.sprite);
        costText.text = item.cost.ToString();
        cardNameText.text = item.cardName;
        attackPointText.text = item.attackPoint.ToString();
        healthPointText.text = item.healthPoint.ToString();
        cardAbilityText.text = item.abilityExplantion;
        cardExplantionText.text = item.cardType + " - " + item.cardGrade;
        hand = GameObject.Find("Hand").GetComponent<Hand>();
    }

    public void Movement3D(PosRotSca prs, bool isUsingDG = false, float dGTime = 0)
    {
        if (isUsingDG)
        {
            transform.DOMove(prs.pos, dGTime);
            transform.DORotateQuaternion(prs.rot, dGTime);
            transform.DOScale(prs.sca, dGTime);
        }
        else
        {
            transform.position = prs.pos;
            transform.rotation = prs.rot;
            transform.localScale = prs.sca;
        }
    }


    private void OnMouseOver()
    {
        hand.CardMouseOver(this);
    }

    private void OnMouseExit()
    {
        hand.CardMouseExit(this);
    }

    private void OnMouseDown()
    {
        hand.CardMouseDown();
    }

    private void OnMouseUp()
    {
        hand.CardMouseUp();
    }
}
