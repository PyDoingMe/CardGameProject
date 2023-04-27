using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;

public class Deck : MonoBehaviour
{
    [SerializeField] private ItemSO itemSO;
    private List<Item> cards = new List<Item>();

    private void Start()
    {
        foreach(int i in JsonUtility.FromJson<PlayerData>(File.ReadAllText(Path.Combine(Application.dataPath, "Deck.json"))).list)
        {
            cards.Add(itemSO.items[i]);
        }
        Shuffle();
    }

    public void Shuffle()
    {
        for (int i = 0; i < cards.Count; i++)
        {
            Item temp = cards[i];
            int randomIndex = Random.Range(0, cards.Count);
            cards[i] = cards[randomIndex];
            cards[randomIndex] = temp;
        }
    }

    public Item Drowed()
    {
        if (cards.Count == 0)
        {
            return new Item();
        }
        else
        {
            Item returnItem = cards[0];
            cards.RemoveAt(0);
            return returnItem;
        }
    }
}
