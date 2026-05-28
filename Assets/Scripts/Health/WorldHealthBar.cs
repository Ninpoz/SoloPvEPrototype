using UnityEngine;
using UnityEngine.UI;

public class WorldHealthBar : MonoBehaviour
{
    [SerializeField] private Image fillImage;
    [SerializeField] private Camera targetCamera;

    public void SetHealth(int currentHealth, int maxHealth)
    {
        if (fillImage == null)
        {
            return;
        }

        float healthPercent = (float)currentHealth / maxHealth;
        fillImage.fillAmount = Mathf.Clamp01(healthPercent);
    }

    private void LateUpdate()
    {
        if (targetCamera == null)
        {
            targetCamera = Camera.main;
        }

        if (targetCamera == null)
        {
            return;
        }

        transform.LookAt(transform.position + targetCamera.transform.forward);
    }
}