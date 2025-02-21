using UnityEngine;

public class ObstacleMover : MonoBehaviour, IScrollingObject
{
    [SerializeField] private float SpeedMove = 3.6f;
    
    private ObstaclesController _obstaclesController;
    private BordersData _bordersData;
    
    private bool _needStopping;
    private float _hightSprite;
    private static float _speedBoost = 1f;
    
    private void Awake()
    {
        Init();
    }
    private void Update()
    {
        UpdateScrolling();
        if (_needStopping) StopScrolling();
    }
    
    private void OnEnable()
    {
        ObstacleTrigger.OnPlayerHit += StopScrolling;
    }

    private void OnDisable()
    {
        ObstacleTrigger.OnPlayerHit -= StopScrolling;
    }

    private void Init()
    {
        _bordersData = GetComponentInParent<BordersData>();
        _obstaclesController = GetComponentInParent<ObstaclesController>();
        _hightSprite = GetComponent<SpriteRenderer>().bounds.size.y;
    }

    public void UpdateScrolling()
    {
        var minX = _bordersData.MinX - _hightSprite;
        if (transform.position.x < minX)
        {
            gameObject.SetActive(false);
            return;
        }

        var offset = new Vector3(SpeedMove * _obstaclesController.SpeedBoost * Time.deltaTime, 0, 0);
        transform.position -= offset;
    }
    
    public void StopScrolling()
    {
        _needStopping = true;
        _speedBoost = Mathf.Lerp(_speedBoost, 0, 0.1f);
    }
}
