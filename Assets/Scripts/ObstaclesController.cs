using UnityEngine;
using Random = UnityEngine.Random;

public class ObstaclesController : MonoBehaviour
{
    [SerializeField] private float MaxSpawnInterval = 4f;
    [SerializeField] private float MinSpawnInterval = 2f;
    
    [SerializeField] private GameObject[] ObstaclePrefabs;

    
    private float _spawnInterval;
    private float _spawnTimer;
    
    private int _nextObstacle;
    private int _lastObstacle;
    private int _penultimateObstacle;
    
    private bool _stopSpawn;
    
    private Transform[] _obstaclesObjects;

    public float SpeedBoost { get; private set; } = 1f;
    //private float _lerpSpeedBoost;
    //private float _lerpStepBoost;

    private void Awake()
    {
        InstantiatePrefabs();
    }
    private void Start()
    {
        Init();
    }

    private void Update()
    {
        if (_stopSpawn) return;
        Timer();
        Spawn();
        //LerpSpeedUp();
    }

    /*private void LerpSpeedUp()
    //ты все сломал, лох.
    {
        while (SpeedBoost != _lerpSpeedBoost)
        {
            SpeedBoost = Mathf.Lerp(SpeedBoost, _lerpSpeedBoost, _lerpStepBoost/10);
        }
    }*/

    private void InstantiatePrefabs()
    {
        _obstaclesObjects = new Transform[ObstaclePrefabs.Length];
        for (int i = 0; i < ObstaclePrefabs.Length; i++)
        {
            _obstaclesObjects[i] = Instantiate(
                    ObstaclePrefabs[i], 
                    Vector3.zero, 
                    Quaternion.identity, 
                    transform).GetComponent<Transform>();
            _obstaclesObjects[i].gameObject.SetActive(false);
        }
    }

    private void Init()
    {
        _spawnInterval = MinSpawnInterval;
        _nextObstacle = Random.Range(0, _obstaclesObjects.Length - 1);
        _lastObstacle = _penultimateObstacle = _nextObstacle;
    }
    
    private void Timer()
    {
        _spawnTimer += Time.deltaTime;
    }

    private void Spawn()
    {
        if (_spawnTimer < _spawnInterval) return;
        if (_obstaclesObjects[_nextObstacle].gameObject.activeInHierarchy)
        {
            NextObstacle();
            return;
        }
        _obstaclesObjects[_nextObstacle].gameObject.SetActive(true);
        
        _spawnTimer = 0;
        _spawnInterval = Random.Range(MinSpawnInterval, MaxSpawnInterval);
        
        _penultimateObstacle = _lastObstacle;
        _lastObstacle = _nextObstacle;
        
        NextObstacle();
    }

    private void NextObstacle()
    {
        _nextObstacle = Random.Range(0, _obstaclesObjects.Length - 1);
        if (_obstaclesObjects.Length <= 3) return;
        if (_nextObstacle == _lastObstacle || _nextObstacle == _penultimateObstacle) NextObstacle();
    }
    
    private void OnEnable()
    {
        ObstacleTrigger.OnPlayerHit += StopSpawn;
        DistanceTracker.GameSpeedUp += SpeedUp;
    }

    private void OnDisable()
    {
        ObstacleTrigger.OnPlayerHit -= StopSpawn;
        DistanceTracker.GameSpeedUp -= SpeedUp;
    }

    private void StopSpawn()
    {
        _stopSpawn = true;
    }
    
    private void SpeedUp(float boost)
    {
        SpeedBoost += boost;
    }
}
