using UnityEngine;
using UnityEngine.UI;

public class CprFeedbackUI : MonoBehaviour
{
    public CprHandDetector detector;
    public Slider depthBar;
    public Text rateLabel;
    public Image rateIndicator; // เขียวถ้า 100-120/min

    void Update()
    {
        depthBar.value = detector.compressionDepth01;

        float rate = detector.compressionRate;
        rateLabel.text = rate > 0 ? $"{rate:F0} /min" : "—";

        // AHA guideline: 100-120 compressions/min
        bool good = rate >= 100f && rate <= 120f;
        rateIndicator.color = good ? Color.green : Color.red;
    }
}