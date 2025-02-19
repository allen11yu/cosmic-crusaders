using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Health : MonoBehaviour
{
    public int healthValue = 1;
    private float moveSpeed = 1f; // Speed of movement
    private float height = 2f;     // Height of the up/down movement

    private Vector3 startingPosition;

    void Start()
    {
        startingPosition = transform.position;
    }

    void Update()
    {
        float newY = startingPosition.y + Mathf.Sin(Time.time * moveSpeed) * height;
        transform.position = new Vector3(startingPosition.x, newY, startingPosition.z);
    }
}

