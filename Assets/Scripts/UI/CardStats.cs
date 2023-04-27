using UnityEngine;
using DG.Tweening;
using TMPro;

public class CardStats : MonoBehaviour
{
    [SerializeField] TMP_Text cardNameText;
    [SerializeField] TMP_Text attackPointText;
    [SerializeField] TMP_Text healthPointText;
    [SerializeField] TMP_Text cardAbilityText;

    private void Start()
    {
        ScaleZero();
    }

    public void Show(Item item)
    {
        Debug.Log("tlqkffusdk");
        cardNameText.text = item.cardName;
        attackPointText.text = item.attackPoint.ToString();
        healthPointText.text = item.healthPoint.ToString();
        cardAbilityText.text = item.abilityExplantion;
        ScaleOne();
    }

    public void Close()
    {
        Debug.Log("qudtlstoRldi");
        ScaleZero();
    }

    [ContextMenu("ScaleOne")] void ScaleOne() => transform.localScale = Vector3.one;
    [ContextMenu("ScaleZero")] public void ScaleZero() => transform.localScale = Vector3.zero;
}
