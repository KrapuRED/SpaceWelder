using UnityEngine;

public class Asteroid : MonoBehaviour
{
    [SerializeField] private float _speedAsteroid;
    [SerializeField] private float _nearDestinationPoint;

    private Vector2 _destinationPoint;

    private void Update()
    {
        MoveAsteroid();

        if (Vector2.Distance(transform.position, _destinationPoint) <= _nearDestinationPoint)
        {
            AsteroidManager.Instance.OnAsteroidDestroyed();
            Destroy(gameObject);
        }
    }

    public void InitializedAsteroid(Vector2 destinationPoint)
    {
        _destinationPoint = destinationPoint;
    }

    private void MoveAsteroid()
    {
        if (_destinationPoint == null) return;

        transform.position = Vector2.MoveTowards(
            transform.position,
            _destinationPoint,
            _speedAsteroid * Time.deltaTime);
    }
}
