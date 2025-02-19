using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class ShipCoordinates : MonoBehaviour
{
    [Header("Boundary Sphere")]
    public GameObject BoundarySphere;

    [Header("UI Elements")]
    public TextMeshProUGUI coordinatesText;

    private float boundaryRadius;
    private Vector3 boundaryCenter;
    public float distanceThreshhold = 50f;

    void Start()
    {
        boundaryCenter = BoundarySphere.transform.position;
        boundaryRadius = BoundarySphere.transform.localScale.x * 0.5f;
    }

    void Update()
    {
        // the relative position from the center
        Vector3 relativePosition = transform.position - boundaryCenter;

        // the distance from the edge of the boundary
        float distanceFromEdge = boundaryRadius - relativePosition.magnitude;

        distanceFromEdge = Mathf.Max(0f, distanceFromEdge);
        string distanceString = string.Format("Distance from Edge: {0:F2}", distanceFromEdge);

        coordinatesText.text = distanceString;
    }
}
