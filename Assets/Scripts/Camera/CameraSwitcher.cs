using UnityEngine;

public class CameraSwitcher : MonoBehaviour
{
    public Camera normalCamera;
    public Camera lookBehindCamera;

    void Start()
    {
        normalCamera.enabled = true;
        lookBehindCamera.enabled = false;
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.B))
        {
            normalCamera.enabled = false;
            lookBehindCamera.enabled = true;
        }
        else if (Input.GetKeyUp(KeyCode.B))
        {
            normalCamera.enabled = true;
            lookBehindCamera.enabled = false;
        }
    }
}
