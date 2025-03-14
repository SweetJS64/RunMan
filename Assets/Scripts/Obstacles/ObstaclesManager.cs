using System.Collections;
using UnityEngine;

public class ObstaclesManager : MonoBehaviour
{
    [SerializeField] private float MinGlobalSpawnInterval = 2f;
    [SerializeField] private float MaxGlobalSpawnInterval = 4f;
    [SerializeField] private int MinRotateSpawnCount = 3;
    [SerializeField] private int MinLaserSpawnCount = 2;
    
    private ObstaclesSpawner _spawner;
    private AbstractRareObstacle _rotateObstacle;
    private AbstractRareObstacle _laserObstacle;
    
    private GameObject[] _regularObstacles;
    private int _idRegularObstacle;
    
    private bool _stopSpawn;

    public float SpeedBoost { get; private set; } = 1f;
    
    private void Awake()
    {
        Init();
    }
    
    private void OnEnable()
    {
        ObstacleTrigger.OnPlayerHit += StopSpawn;
        DistanceTracker.GameSpeedUp += SpeedUp;
        LaserGeneratorMover.DisableLaserObstacle += EnableNextObstacle;
        
        var waitToSpawn = Random.Range(MinGlobalSpawnInterval, MaxGlobalSpawnInterval);
        StartCoroutine(WaitNextObstacle(waitToSpawn));
    }

    private void OnDisable()
    {
        ObstacleTrigger.OnPlayerHit -= StopSpawn;
        DistanceTracker.GameSpeedUp -= SpeedUp;
        LaserGeneratorMover.DisableLaserObstacle -= EnableNextObstacle;
    }
    
    private void Init()
    {
        _spawner = GetComponent<ObstaclesSpawner>();
        _regularObstacles = _spawner.GetRegularObstacles();
        _rotateObstacle = _spawner.GetRotateObstacle().GetComponent<AbstractRareObstacle>();
        _rotateObstacle.SetMinSpawnCount(MinRotateSpawnCount);
        _laserObstacle = _spawner.GetLaserObstacle().GetComponent<AbstractRareObstacle>();
        _laserObstacle.SetMinSpawnCount(MinLaserSpawnCount);
    }
    
    private void EnableNextObstacle()
    {
        if (_stopSpawn) return;

        var waitTime = Random.Range(MinGlobalSpawnInterval, MaxGlobalSpawnInterval);
        
        if (_rotateObstacle.ShouldSpawn())
        {
            EnableAbstractObstacle(_rotateObstacle, waitTime);
            return;
        }
        
        if (_laserObstacle.ShouldSpawn())
        {
            EnableAbstractObstacle(_laserObstacle, waitTime);
            StopAllCoroutines();
            return;
        }
        
        EnableRegularObstacle(waitTime);
    }

    private void EnableRegularObstacle(float waitTime)
    {
        
        _regularObstacles[_idRegularObstacle].SetActive(true);
        
        var arrayPassed = _idRegularObstacle == _regularObstacles.Length - 1;
        _idRegularObstacle = arrayPassed ? 0 : ++_idRegularObstacle;
        StartCoroutine(WaitNextObstacle(waitTime));
    }
    
    private void EnableAbstractObstacle (AbstractRareObstacle obstacle, float waitTime)
    {
        obstacle.gameObject.SetActive(true);
        StartCoroutine(WaitNextObstacle(waitTime));
    }
    
    private void SpeedUp(float boost)
    {
        if(_stopSpawn) return;
        SpeedBoost += boost;
    }

    private void StopSpawn()
    {
        _stopSpawn = true;
        StopAllCoroutines();
    }
    
    private IEnumerator WaitNextObstacle(float waitTime)
    {
        yield return new WaitForSeconds(waitTime);
        EnableNextObstacle();
    }
}
