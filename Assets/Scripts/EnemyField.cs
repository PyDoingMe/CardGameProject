using UnityEngine;
using Utility;

public class EnemyField : MonoBehaviour
{
    [SerializeField] private GameObject entity;
    [SerializeField] private FieldManager fieldManager;

    public int atk;
    [SerializeField] private int hp;
    [SerializeField] private bool isKnightZone;
    public bool IsKnightZone { get { return isKnightZone; } }
    public bool isDie = true;
    public Entity myEntity;

    private Item item;

    public bool Damaged(int damage)
    {
        hp -= damage;
        myEntity.healthPointText.text = hp.ToString();

        if (hp <= 0)
        {
            item = null;
            myEntity = null;
            isDie = true;
            return true;
        }
        return false;
    }

    public void Participate(Item item)
    {
        this.item = item;
        isDie = false;
        var instance = Instantiate(entity, transform);
        instance.transform.SetParent(transform);
        myEntity = instance.GetComponent<Entity>();
        myEntity.Init(item);
        myEntity.Prs = new PosRotSca(transform.position, Quaternion.identity, Vector3.one);
        atk = item.attackPoint;
        hp = item.healthPoint;
    }
    private void OnMouseDown()
    {
        if (!isDie)
        {
            fieldManager.EntitiyShow(item);
        }
    }

    private void OnMouseExit()
    {
        if (!isDie)
            fieldManager.EntityClose();
    }
}
