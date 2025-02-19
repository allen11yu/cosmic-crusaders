using UnityEngine;

public class Jet : MonoBehaviour
{
    public Transform parentObject;

    void Update()
    {
        if (parentObject != null)
        {
            // Invert rotation to match the parent
            transform.rotation = Quaternion.Inverse(parentObject.rotation);
        }
    }
}