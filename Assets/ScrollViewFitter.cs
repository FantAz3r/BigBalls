using UnityEngine;
using UnityEngine.UI;
using VContainer;

public class ScrollViewFitter : MonoBehaviour
{
    [SerializeField] private BallTreeNodeUI _nodeUI;
    [SerializeField] private ScrollRect _scrollRect;

    private float _minX = float.NegativeInfinity;
    private float _maxX =  float.PositiveInfinity;
    private float _minY = float.NegativeInfinity;
    private float _maxY =  float.PositiveInfinity;
    private RectTransform _rectTransform;
    private BallRepository _ballRepository;


    [Inject]
    public void Construct (BallRepository ballRepository)
    {
        _ballRepository = ballRepository;
    }

    private void Awake ()
    {
        _rectTransform = gameObject.GetComponent<RectTransform>();
    }

    private void Start ()
    {
        _rectTransform.sizeDelta = new Vector2(4000, 4000);
        ClampScroll();
    }

    private void OnEnable ()
    {
        ClampScroll();
    }

    private void Update ()
    {
        Vector3 pos = _rectTransform.localPosition;

        float clampedX = Mathf.Clamp(pos.x, _maxX, _minX);
        float clampedY = Mathf.Clamp(pos.y, _maxY, _minY);

        _rectTransform.localPosition = new Vector3(clampedX, clampedY, pos.z);

        Vector2 velocity = _scrollRect.velocity;

        if (!Mathf.Approximately(clampedX, pos.x))
            velocity.x = 0f;

        if (!Mathf.Approximately(clampedY, pos.y))
            velocity.y = 0f;

        _scrollRect.velocity = velocity;
    }

    private void ClampScroll ()
    {
        foreach (var ball in _ballRepository.AllModels.Values)
        {
            Vector2 pos = ball.BallConfig.Position;

            if (pos.x < -_minX)
                _minX = -pos.x;

            if (pos.x > -_maxX)
                _maxX = -pos.x;

            if (pos.y < -_minY)
                _minY = -pos.y;

            if (pos.y > -_maxY)
                _maxY = -pos.y;
        }
    }
}
