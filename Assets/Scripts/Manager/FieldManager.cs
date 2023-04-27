using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public class FieldManager : MonoBehaviour
{
    [Tooltip("0번 아군 보스/13번 적군 보스/1~12번 아군(좌->우)/14~ 적군(우->좌)")]
    [SerializeField] List<GameObject> fields;
    [SerializeField] CardStats cardStats;
    [SerializeField] GameObject targetPicker;
    [SerializeField] GameObject attackEffect;
    [SerializeField] Camera fieldCamera;

    bool CanAttack => TurnManager.myTurn && !TurnManager.isLoading;
    bool ExistTargetPickEntity => targetField != null;
    private Field selectField;
    EnemyField targetField;
    [SerializeField] GameDirector gameDirector;
    [SerializeField] ItemSO itemSO;

    private void Update()
    {
        ShowTargetPicker(ExistTargetPickEntity);
    }

    void ShowTargetPicker(bool isShow)
    {
        targetPicker.SetActive(isShow);
        if (ExistTargetPickEntity)
        {
            targetPicker.transform.position = targetField.transform.position + Vector3.up*0.1f;
        }
    }

    bool Attack(Field attacker, EnemyField defender)
    {
        if (defender.myEntity == null || !defender.IsKnightZone) return false;
        attacker.attackable = false;

        var atkEntityTransform = attacker.myEntity.transform;
        attackEffect.transform.position = Vector3.Lerp(attacker.transform.position, defender.transform.position, 0.8f);
        attackEffect.transform.LookAt(defender.transform);
        attackEffect.SetActive(true);
        Sequence sequence = DOTween.Sequence().Append(atkEntityTransform.DOMoveY(5, 0.2f).SetEase(Ease.OutQuad))
            .Append(atkEntityTransform.DOMove(defender.myEntity.transform.position, 0.4f)).SetEase(Ease.InSine)
            .AppendCallback(() => {
                attacker.Damaged(defender.atk);
                defender.Damaged(attacker.atk);
            })
            .Append(atkEntityTransform.DOMove(attacker.myEntity.Prs.pos, 0.4f)).SetEase(Ease.OutSine)
            .OnComplete(() => AttackCallback(attacker, defender));
        StartCoroutine(CheckingGameOver());
        return true;
    }

    public void EnemyAttack(ushort atker, ushort dfder)
    {
        EnemyField attacker = fields[atker + 13].GetComponent<EnemyField>();
        Field defender = fields[dfder - 13].GetComponent<Field>();
        Sequence sequence = DOTween.Sequence()
            .Append(attacker.myEntity.transform.DOMoveY(5, 0.2f).SetEase(Ease.OutQuad))
            .Append(attacker.myEntity.transform.DOMove(defender.transform.position, 0.4f)).SetEase(Ease.InSine)
            .AppendCallback(() => {
                attacker.Damaged(defender.atk);
                defender.Damaged(attacker.atk);
            })
            .Append(attacker.myEntity.transform.DOMove(attacker.myEntity.Prs.pos, 0.4f)).SetEase(Ease.OutSine)
            .OnComplete(() => AttackCallback(defender, attacker));
        StartCoroutine(CheckingGameOver());
    }

    IEnumerator CheckingGameOver()
    {
        yield return new WaitForSeconds(2.6f);
        attackEffect.SetActive(false);
        if (fields[0].GetComponent<Field>().isDie)
        {
            TurnManager.EndGame("패배");
        }
        if (fields[13].GetComponent<EnemyField>().isDie)
        {
            TurnManager.EndGame("승리");
        }
            
    }

    void AttackCallback(Field attacker, EnemyField defender)
    {
        if(attacker.isDie)
        {
            Sequence sequence1 = DOTween.Sequence()
                .Append(attacker.myEntity.transform.DOShakePosition(1.3f))
                .Append(attacker.myEntity.transform.DOScale(Vector3.zero, 0.3f)).SetEase(Ease.OutCirc)
                .OnComplete(() =>
                {
                    Destroy(attacker.myEntity.gameObject);
                });
        }
        if (defender.isDie)
        {
            Sequence sequence2 = DOTween.Sequence()
                .Append(defender.myEntity.transform.DOShakePosition(1.3f))
                .Append(defender.myEntity.transform.DOScale(Vector3.zero, 0.3f)).SetEase(Ease.OutCirc)
                .OnComplete(() =>
                {
                    Destroy(defender.myEntity.gameObject);
                });
        }
    }

    public bool Participate(Card card)
    {
        if (card == null)
            return false;

        RaycastHit[] hits = Physics.RaycastAll(fieldCamera.ScreenPointToRay(Input.mousePosition));

        foreach (var i in hits)
        {
            if (i.collider.CompareTag("Field"))
            {
                var j = i.collider.gameObject;
                if (j.GetComponent<Field>().Participate(card.item))
                {
                    gameDirector.SendTwoIntMsg(10, fields.IndexOf(j), card.item.sprite);
                    return true;
                }
            }
        }
        //for (int i= 0; i < fields.Count; i++)
        //{
        //    if (card == null)
        //        continue;
        //    if (Vector3.Distance(card.transform.position, fields[i].transform.position) < 100f)
        //    {
        //        if (fields[i].GetComponent<Field>().Participate(card.item))
        //        {
        //            gameDirector.SendTwoIntMsg(10,i, card.item.sprite);
        //            return true;
        //        }
        //    }
        //}
        return false;
    }

    public void EnemyParticipate(int index, int card)
    {
        fields[index + 13].GetComponent<EnemyField>().Participate(itemSO.items[card]);
    }

    public void EntityMouseDown(Field field)
    {
        if (!CanAttack) return;
        selectField = field;
    }

    public void EntitiyShow(Item item)
    {
        cardStats.Show(item);
    }

    public void EntityClose()
    {
        cardStats.Close();
    }
    
    public void EntityMouseUp()
    {
        if (!CanAttack) return;
        if (selectField && targetField && selectField.attackable)
        {
            if(Attack(selectField, targetField))
                gameDirector.SendTwoIntMsg(11, fields.IndexOf(selectField.gameObject), fields.IndexOf(targetField.gameObject));
        }
        selectField = null;
        targetField = null;

    }

    public void EntityMouseDrag()
    {
        if (!CanAttack || selectField == null) return;
        bool target = false;
        foreach (var hit in Physics.RaycastAll(fieldCamera.ScreenPointToRay(Input.mousePosition)))
        {
            EnemyField field = hit.collider?.GetComponent<EnemyField>();
            if (field != null && selectField.attackable)
            {
                if (field.isDie)
                {
                    field = null;
                    continue;
                }
                targetField = field;
                target = true;
            }
        }
        if (!target) targetField = null;
    }

}