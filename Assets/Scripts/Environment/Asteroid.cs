using UnityEngine;

public class Asteroid : MonoBehaviour
{
    private void OnBecameInvisible()
    {
        AsteroidPool.Instance.ReturnAsteroid(gameObject);
    }
}
