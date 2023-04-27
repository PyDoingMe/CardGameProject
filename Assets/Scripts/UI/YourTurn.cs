using TMPro;
using DG.Tweening;
using UnityEngine;

public class YourTurn : MonoBehaviour
{
    [SerializeField] TMP_Text txt;

    public void Show(string m)
    {
        txt.text = m;
        Sequence sequence = DOTween.Sequence().Append(transform.DOScale(Vector3.one, 0.9f).SetEase(Ease.InOutCirc))
            .AppendInterval(0.7f).Append(transform.DOScale(Vector3.zero, 0.3f).SetEase(Ease.InOutQuad));
    }

    private void Start()
    {
        ScaleZero();
    }

    [ContextMenu("ScaleOne")] void ScaleOne() => transform.localScale = Vector3.one;
    [ContextMenu("ScaleZero")] public void ScaleZero() => transform.localScale = Vector3.zero;
}
