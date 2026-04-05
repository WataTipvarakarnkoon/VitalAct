using UnityEngine;

public class UITouchTrigger : MonoBehaviour
{
    public GameObject uiPanel; // ลาก HeadUI หรือ ChestUI มาใส่

    void Start()
    {
        // ซ่อนไว้ก่อน
        if (uiPanel != null)
            uiPanel.SetActive(false);
    }

    void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Hand"))
        {
            if (uiPanel != null)
                uiPanel.SetActive(true);
        }
    }
}