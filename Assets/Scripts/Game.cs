using UnityEngine;

public class Game : MonoBehaviour
{
    [SerializeField] private Player _player;
    [SerializeField] private Level _level;
    [SerializeField] private StoreMenu _store;
    [SerializeField] private Mana _mana;
    [SerializeField] private Wall _wall;
    [SerializeField] private EnemySpawner _enemySpawner;

    [SerializeField] private LevelMessage _levelMessage;

    private static Game _instance;
    private AliveEnemiesHolder _aliveEnemiesHolder;

    public static Wall Wall => _instance._wall;

    private void Awake()
    {
        if (_instance != null)
            Destroy(gameObject);
        else
            _instance = this;
    }

    private void Start()
    {
        PlayerPrefs.DeleteAll();
        _store.Fill();
        _mana.Init(_enemySpawner);
        _player.Play();
        _wall.Destroyed += OnWallDestroyed;
        _aliveEnemiesHolder = new AliveEnemiesHolder(_enemySpawner);
        _enemySpawner.AllowSpawning();
        _level.LevelFinished += OnLevelFinished;
        _level.StartNextLevel();
        _levelMessage.Show("Level " + (_level.CurrentLevel + 1) + " is started");
        enabled = false;
    }

    private void Update()
    {
        if (_aliveEnemiesHolder.Count == 0)
        {
            _wall.Health.IncreaseMaxHealth(10);
            _wall.Health.RestoreHealth(_wall.Health.MaxHealth / 2);
            _level.StartNextLevel();
            enabled = false;
            _levelMessage.Show("Level " + (_level.CurrentLevel + 1) + " is started");
        }
    }

    private void OnLevelFinished()
    {
        enabled = true;
    }

    private void OnWallDestroyed()
    {
        _wall.Destroyed -= OnWallDestroyed;
        _levelMessage.Show("GameOver");
        _player.StopPlaying();
        _enemySpawner.StopSpawning();
        _aliveEnemiesHolder.StopAllEnemies();
    }
}