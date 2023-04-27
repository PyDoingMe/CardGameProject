using Utility;
using UnityEngine;
using TMPro;
using UnityEngine.UI;
using DG.Tweening;

public class Entity : MonoBehaviour
{
    [SerializeField] TMP_Text cardNameText;
    [SerializeField] TMP_Text attackPointText;
    [SerializeField] public TMP_Text healthPointText;
    [SerializeField] TMP_Text cardExplantionText;

    PosRotSca prs;
    public PosRotSca Prs
    {
        get { return prs; }
        set { prs = value; }
    }
    Item item;

    public void Init(Item i)
    {
        item = i;
        cardNameText.text = item.cardName;
        attackPointText.text = item.attackPoint.ToString();
        healthPointText.text = item.healthPoint.ToString();
        cardExplantionText.text = item.cardType + ":" + item.cardGrade;
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
}
