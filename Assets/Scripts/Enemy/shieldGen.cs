using UnityEngine;

public class ShieldGen : MonoBehaviour
{
    public float detectionRadius = 35f;

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Enemy"))
        {
            Transform shield = other.transform.Find("Shield");

            if (shield != null)
            {
                shield.gameObject.SetActive(true);
            }
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Enemy"))
        {
            Transform shield = other.transform.Find("Shield");

            if (shield != null)
            {
                shield.gameObject.SetActive(false);
            }
        }
    }

    private void Update()
    {
        GameObject[] enemies = GameObject.FindGameObjectsWithTag("Enemy");

        foreach (GameObject enemy in enemies)
        {
            Transform shield = enemy.transform.Find("Shield");
            if (shield != null && !shield.gameObject.activeSelf)
            {
                shield.gameObject.SetActive(true);
            }
        }
    }
}
