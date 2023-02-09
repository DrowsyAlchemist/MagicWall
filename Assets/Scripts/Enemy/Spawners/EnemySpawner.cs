using System;
using System.Collections;
using UnityEngine;

public class EnemySpawner : MonoBehaviour
{
    [SerializeField] private Transform[] _spawnPoints;

    public bool IsSpawningAllowed { get; private set; }

    public event Action<Enemy> EnemySpawned;

    public void AllowSpawning()
    {
        IsSpawningAllowed = true;
    }

    public void StopSpawning()
    {
        IsSpawningAllowed = false;
    }

    public void Spawn(Enemy template)
    {
        if (template == null)
            throw new ArgumentNullException();

        if (IsSpawningAllowed)
            StartCoroutine(SpawnEnemy(template));
    }

    private IEnumerator SpawnEnemy(Enemy template)
    {
        yield return new WaitForEndOfFrame();
        int spawnPointIndex = UnityEngine.Random.Range(0, _spawnPoints.Length);
        Transform spawnPoint = _spawnPoints[spawnPointIndex];
        Enemy enemy = Instantiate(template, spawnPoint.position, Quaternion.identity, null);
        EnemySpawned?.Invoke(enemy);
    }
}