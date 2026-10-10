using UnityEngine;

[RequireComponent(typeof(LineRenderer))]
public class RopeViewer : MonoBehaviour
{
    [Header("Links")]
    [SerializeField] private Transform _startPoint;
    [SerializeField] private Transform _endPoint;

    [Header("Settings")]
    [SerializeField] private bool _useSmoothing = false;
    [SerializeField] private int _subdivisions = 10;

    private LineRenderer _lineRenderer;

    private void Awake ()
    {
        _lineRenderer = GetComponent<LineRenderer>();

        if (_lineRenderer.positionCount < 2)
        {
            _lineRenderer.positionCount = 2;
        }
    }

    private void Update ()
    {
        if (_startPoint == null || _endPoint == null)
            return;

        UpdateRope();
    }

    private void UpdateRope ()
    {
        if (!_useSmoothing)
        {
            _lineRenderer.positionCount = 2;
            _lineRenderer.SetPosition(0, _startPoint.position);
            _lineRenderer.SetPosition(1, _endPoint.position);
        }
        else
        {
            DrawCurvedLine();
        }
    }

    private void DrawCurvedLine ()
    {
        _lineRenderer.positionCount = _subdivisions;

        Vector3 start = _startPoint.position;
        Vector3 end = _endPoint.position;

        Vector3 midPoint = (start + end) * 0.5f;
        midPoint += Vector3.up * 0.5f;

        for (int i = 0; i < _subdivisions; i++)
        {
            float t = i / (float) (_subdivisions - 1);

            Vector3 pos = Mathf.Pow(1 - t, 2) * start +
                          2 * (1 - t) * t * midPoint +
                          Mathf.Pow(t, 2) * end;

            _lineRenderer.SetPosition(i, pos);
        }
    }
}
