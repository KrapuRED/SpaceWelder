using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AsteroidManager : MonoBehaviour
{
    public static AsteroidManager Instance;

    [Header("Asteroid Spawn Manager Config")]
    [SerializeField] private GameObject prefabAsteroid;
    [SerializeField] private Transform _asteroidContiner;
    [SerializeField] private int minAsteroid;
    [SerializeField] private int maxAsteroid;
    [SerializeField] private float spawnRate;
    [SerializeField] private int _maxActiveAsteroids;

    [Header("Asteroid Waypoint Manager Config")]
    [SerializeField] private Transform _spawPointContainer;
    [SerializeField] private Transform _endPointContainer;
    private List<Transform> _spawnPoints = new List<Transform>();
    private List<Transform> _endPoints = new List<Transform>();

    private Coroutine _startSpawnAsteroid;
    private int _activeAsteroidCount;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        else
            Destroy(gameObject);
    }

    private void Start()
    {
        foreach (Transform point in _spawPointContainer)
            _spawnPoints.Add(point.GetComponentInChildren<Transform>());

        foreach (Transform point in _endPointContainer)
            _endPoints.Add(point.GetComponentInChildren<Transform>());

        _startSpawnAsteroid = StartCoroutine(DelaySpawnAsteroid());
    }

    private Vector3 GetSpawnPosition()
    {
        int index = Random.Range(0, _spawnPoints.Count);

        return _spawnPoints[index].position;
    }

    private Vector3 GetEndPosition()
    {
        int index = Random.Range(0, _endPoints.Count);
        return _endPoints[index].position;
    }

    private void OnSpawnAsteroid()
    {
        Vector3 spawnPos = GetSpawnPosition();

        var newAsteroidGO = Instantiate(prefabAsteroid, spawnPos, Quaternion.identity, _asteroidContiner);

        if (newAsteroidGO == null)
        {
            Debug.LogWarning("[AsteroidManager - OnSpawnAsteroid] New Asteroid is Failed to Instantiate");
            return;
        }

        Asteroid newAsteroid = newAsteroidGO.GetComponent<Asteroid>();
        if (newAsteroid == null)
        {
            Debug.LogWarning("[AsteroidManager - OnSpawnAsteroid] The newAsteroidGO not been assign with Asteroid script!");
            return;
        }
        newAsteroid.InitializedAsteroid(GetEndPosition());
        _activeAsteroidCount++;
    }

    public void OnAsteroidDestroyed()
    {
        _activeAsteroidCount = Mathf.Max(0, _activeAsteroidCount - 1);
    }

    IEnumerator DelaySpawnAsteroid()
    {
        while (true)
        {
            yield return new WaitForSeconds(spawnRate);

            int needSpawn = Random.Range(minAsteroid, maxAsteroid + 1);

            for (int i = 0; i < needSpawn; i++)
            {
                // Don't exceed hard cap
                if (_activeAsteroidCount >= _maxActiveAsteroids) break;

                OnSpawnAsteroid();
            }
        }
    }

}
