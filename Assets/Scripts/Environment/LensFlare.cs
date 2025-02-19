using UnityEngine;
using UnityEngine.UI;

public class LensFlare : MonoBehaviour
{
    public Camera mainCamera;              
    public Light lightSource;              
    public RawImage flareImage;            
    public float angleThreshold = 40f;     

    private CanvasGroup canvasGroup;        

    void Start()
    {
        if (mainCamera == null) mainCamera = Camera.main; 
        if (flareImage != null)
        {
            flareImage.gameObject.SetActive(false); // Start with the flare hidden
            canvasGroup = flareImage.GetComponent<CanvasGroup>();
            if (canvasGroup == null)
            {
                canvasGroup = flareImage.gameObject.AddComponent<CanvasGroup>();
            }
        }
    }

    void Update()
    {
        if (lightSource != null && mainCamera != null && flareImage != null)
        {
            Vector3 lightDirection = lightSource.transform.position - mainCamera.transform.position;
            float angle = Vector3.Angle(mainCamera.transform.forward, lightDirection);

            if (angle < angleThreshold)
            {
                if (!flareImage.gameObject.activeSelf)
                {
                    flareImage.gameObject.SetActive(true);
                }

                float fadeAmount = Mathf.InverseLerp(0f, angleThreshold, angle);
                canvasGroup.alpha = Mathf.Lerp(canvasGroup.alpha, 1f - fadeAmount, Time.deltaTime * 5f);
            }
            else
            {
                if (flareImage.gameObject.activeSelf)
                {
                    flareImage.gameObject.SetActive(false);
                }
            }
        }
    }
}


