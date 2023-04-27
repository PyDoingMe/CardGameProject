using UnityEngine;
using System.IO;
using System.Collections.Generic;

/// <summary>
/// µ¦À» ºôµùÇØÁÖ´Â ¿ªÇÒ
/// </summary>
public class DeckManager : MonoBehaviour
{
    PlayerData playerData = new PlayerData();

    public void AddCard(int i)
    {
        playerData.list.Add(i);
    }

    public void SaveDeck()
    {
        File.WriteAllText(Path.Combine(Application.dataPath, "Deck.json"), JsonUtility.ToJson(playerData, true));
    }

    public void LoadDeck()
    {
        playerData = JsonUtility.FromJson<PlayerData>(File.ReadAllText(Path.Combine(Application.dataPath, "Deck.json")));
    }
}

[System.Serializable]
public class PlayerData
{
    public List<int> list = new List<int>();
}

