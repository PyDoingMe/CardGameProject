using UnityEngine.UI;
using UnityEngine;
using System.Collections;
using DG.Tweening;
using UnityEngine.SceneManagement;

public class FadeOut : MonoBehaviour
{
    [SerializeField] GameObject button;
    Image image;
    Color color;

    private void Start()
    {
        image = GetComponent<Image>();
        color = image.color;
    }

    public void NextScene()
    {
        SceneManager.LoadScene("EndGame");
    }

    public void Show()
    {
        transform.localScale = Vector3.one;
        StartCoroutine(Showing());
    }
    private IEnumerator Showing()
    {
        color.a += 0.05f;
        image.color = color;
        if (color.a < 0.6f)
        {
            yield return new WaitForSeconds(0.1f);
            StartCoroutine(Showing());
        }
        else
        {
            button.SetActive(true);
            button.transform.DOScale(Vector3.one, 0.3f);
        }
    }
}
