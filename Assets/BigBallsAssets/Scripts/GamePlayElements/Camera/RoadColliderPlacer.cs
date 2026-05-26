using UnityEngine;

public class RoadColliderPlacer : MonoBehaviour
{
    private readonly Vector3 _topScreenPoint = new Vector3(Screen.width / 2f, Screen.height, 0f);
    private readonly Vector3 _bottomScreenPoint = new Vector3(Screen.width / 2f, 0f, 0f);

    [Header("Íàñòðîéêè")]
    [SerializeField] private Camera _ñamera;
    [SerializeField] private BoxCollider _colliderPrefab;
    [SerializeField] private LayerMask _roadLayerMask;
    [SerializeField] private float _raycastDistance = 50f;

    private BoxCollider _topColliderInstance;
    private BoxCollider _bottomColliderInstance;

    void Start()
    {
        PlaceCollidersAtRoadEdges();
    }

    public void PlaceCollidersAtRoadEdges()
    {
        Ray topRay = _ñamera.ScreenPointToRay(_topScreenPoint);
        Ray bottomRay = _ñamera.ScreenPointToRay(_bottomScreenPoint);

        _topColliderInstance = CheckAndPlaceCollider(topRay, _topColliderInstance);
        _bottomColliderInstance = CheckAndPlaceCollider(bottomRay, _bottomColliderInstance);
    }

    private BoxCollider CheckAndPlaceCollider(Ray ray, BoxCollider colliderInstance)
    {
        if (Physics.Raycast(ray, out RaycastHit hit, _raycastDistance, _roadLayerMask))
        {
            if (colliderInstance == null)
                colliderInstance = Instantiate(_colliderPrefab, _ñamera.transform);
            else
                colliderInstance.gameObject.SetActive(true);

            colliderInstance.transform.position = hit.point;
            colliderInstance.transform.rotation = Quaternion.identity;

            return colliderInstance;
        }
        else
        {
            if (colliderInstance != null)
            {
                colliderInstance.gameObject.SetActive(false);
            }

            return null;
        }
    }
}

