using UnityEngine;
using UnityEngine.UI;

public class HUDStatView : MonoBehaviour
{
    [Header("Components"), Space] 
    [SerializeField] private RectTransform barRectTransform;
    [SerializeField] private RectTransform bgRectTransform;
    [SerializeField] private Image barImage;

    [Header("Setting"), Space] 
    [SerializeField] private Color barColor;
    [SerializeField] private float convertRatio = 2f;

    public void Init(float maxValue)
    {
        barImage.color = barColor;
        var barWidth = maxValue * convertRatio;
        barRectTransform.sizeDelta = new Vector2(barWidth,barRectTransform.sizeDelta.y);
        bgRectTransform.sizeDelta = new Vector2(barWidth,bgRectTransform.sizeDelta.y);
        UpdateBarFill(1);
    }

    public void UpdateBarFill(float fillRatio)
    {
        barImage.fillAmount = fillRatio;
    }
}
