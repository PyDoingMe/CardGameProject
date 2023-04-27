using UnityEngine;
using System;

[Serializable]
public class Item
{
    public string cardType;
    public string cardName;
    public string cardGrade;
    public int attackPoint;
    public int healthPoint;
    public int cost;
    public int ability;
    public string abilityExplantion;
    public int sprite;

    public Item(int sprite=0,string CardType="기사카드", string CardName="투지의 사념", string CardGrade="자유기사", int AttackPoint=0,
        int HealthPoint=1, int Cost=0, int Ability=0, string AbilityExplantion="능력이 없습니다.")
    {
        cardGrade = CardGrade;
        cardName = CardName;
        cardType = CardType;
        attackPoint = AttackPoint;
        ability = Ability;
        healthPoint = HealthPoint;
        cost = Cost;
        abilityExplantion = AbilityExplantion;
        this.sprite = sprite;
    }
}

[CreateAssetMenu(fileName = "ItemSO", menuName = "SO/ItemSO")]
public class ItemSO : ScriptableObject
{
    public Item[] items;
}