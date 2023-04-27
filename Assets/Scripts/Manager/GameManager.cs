using System.Collections;
using UnityEngine;
using UnityEngine.Networking;

public class GameManager : MonoBehaviour
{
    [SerializeField] ItemSO itemSO;
    const string URL = "https://docs.google.com/spreadsheets/d/1Yf_I11Q0_boORH3SYr30cnhM1SdK47oNhZ8FdzpRaz4/export?format=tsv&range=A2:H33";

    private void Start()
    {
        StartCoroutine(DownloadItemSOFromGoogleSheets());
        SpriteManager.Init();
    }

    IEnumerator DownloadItemSOFromGoogleSheets()
    {
        UnityWebRequest www = UnityWebRequest.Get(URL);
        yield return www.SendWebRequest();
        SetItemSO(www.downloadHandler.text);
        yield return new WaitForSeconds(5f);
        //StartCoroutine(DownloadItemSOFromGoogleSheets());
    }

    void SetItemSO(string tsv)
    {
        int i = 0;
        Sprite[] sprites = new Sprite[32];
        foreach (string r in tsv.Split('\n'))
        {
            string[] c = r.Split('\t');
            itemSO.items[i] = new Item(i++, c[0], c[1], c[2], int.Parse(c[3]), int.Parse(c[4]), int.Parse(c[5]), int.Parse(c[6]), c[7]);
        }
    }
}
