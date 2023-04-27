using UnityEngine;
using Utility;

public class Field : MonoBehaviour
{
    [SerializeField] private bool isKnightZone;
    [SerializeField] private bool isKingZone;
    [SerializeField] private FieldManager fieldManager;
    [SerializeField] private GameObject entity;

    public int atk;
    public int hp;
    public bool isDie = true;
    public Entity myEntity;
    public bool attackable = false;

    public Item item;

    private void Start()
    {
        TurnManager.OnMyTurn += AttackableSetup;
    }

    private void OnDestroy()
    {
        TurnManager.OnMyTurn -= AttackableSetup;
    }
    public bool Damaged(int damage)
    {
        hp -= damage;
        myEntity.healthPointText.text = hp.ToString();

        if (hp <= 0)
        {
            isDie = true;
            attackable = false;
            return true;
        }
        return false;
    }

    public bool Participate(Item item)
    {
        if (!isDie)
            return false;
        if ((isKnightZone && item.cardType == "기사카드") || (!isKnightZone && item.cardType == "무기카드"))
        {
            this.item = item;
            isDie = false;
            var instance = Instantiate(entity, transform);
            //instance.transform.SetParent(transform);
            myEntity = instance.GetComponent<Entity>();
            myEntity.Init(item);
            myEntity.Prs = new PosRotSca(transform.position, Quaternion.identity, Vector3.one);
            atk = item.attackPoint;
            hp = item.healthPoint;
            return true;
        }
        return false;
    }

    public void AttackableSetup()
    {
        if (!isDie&& !isKingZone)
            attackable = true;
    }

    private void OnMouseDown()
    {
        if (!isDie) {
            fieldManager.EntityMouseDown(this);
            fieldManager.EntitiyShow(item);        
        }
    }

    private void OnMouseExit()
    {
        if (!isDie)
            fieldManager.EntityClose();
    }

    private void OnMouseUp()
    {
        if (!isDie)
        {
            fieldManager.EntityMouseUp();
        }
    }
    private void OnMouseDrag()
    {
        fieldManager.EntityMouseDrag();
    }
}
