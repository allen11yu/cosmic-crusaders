using System.Collections;
using UnityEngine;

public class CameraShake : MonoBehaviour
{
    public Transform cameraTransform;  // Reference to the camera's transform
    public float shakeDuration = 0.5f; // Duration of the shake effect
    public float shakeMagnitude = 0.5f; // Magnitude of the shake
    public float dampingSpeed = 1.0f;  // How fast the shake should diminish

    private Vector3 initialPosition;  // Store the camera's original position
    private bool isShaking = false;  // Whether the camera is currently shaking

    void Start()
    {
        // Save the initial position of the camera
        if (cameraTransform == null)
        {
            cameraTransform = transform;
        }
        initialPosition = cameraTransform.localPosition;
    }

    // Call this method to shake the camera
    public void TriggerShake()
    {
        if (!isShaking)
        {
            StartCoroutine(Shake());
        }
    }

    IEnumerator Shake()
    {
        isShaking = true;
        float elapsed = 0.0f;

        while (elapsed < shakeDuration)
        {
            // Randomly move the camera within the magnitude range
            Vector3 shakeOffset = Random.insideUnitSphere * shakeMagnitude;
            cameraTransform.localPosition = initialPosition + shakeOffset;

            elapsed += Time.deltaTime;
            yield return null;
        }

        // Reset the camera to its original position
        cameraTransform.localPosition = initialPosition;
        isShaking = false;
    }
}