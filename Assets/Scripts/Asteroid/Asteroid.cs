using UnityEngine;

public class Asteroid : MonoBehaviour
{
    [SerializeField] private float _speedAsteroid;
    [SerializeField] private float _nearDestinationPoint;
    [SerializeField] private float _damage;

    private Vector2 _destinationPoint;

    private void Update()
    {
        MoveAsteroid();

        if (Vector2.Distance(transform.position, _destinationPoint) <= _nearDestinationPoint)
        {
            AsteroidDestroy();
        }
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        IDamageAble damageAble = collision.collider.GetComponent<IDamageAble>();

        if (damageAble == null) return;

        damageAble.OnTakeDamage(_damage);
        AsteroidDestroy();
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

    void AsteroidDestroy()
    {
        AsteroidManager.Instance.OnAsteroidDestroyed();
        Destroy(gameObject);
    }
}
