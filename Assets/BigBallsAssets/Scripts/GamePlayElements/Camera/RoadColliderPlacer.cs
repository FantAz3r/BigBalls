using UnityEngine;

namespace BigBalls.GameplayObjects
{
    public class RoadColliderPlacer : MonoBehaviour
    {
        private readonly Vector3 _topScreenPoint = new Vector3(Screen.width / 2f, Screen.height, 0f);
        private readonly Vector3 _bottomScreenPoint = new Vector3(Screen.width / 2f, 0f, 0f);

        [Header("Настройки")]
        [SerializeField] private Camera _camera;
        [SerializeField] private FrontWall _frontWallPrefab;
        [SerializeField] private BackWall _backWallPrefab;
        [SerializeField] private LayerMask _roadLayerMask;
        [SerializeField] private float _raycastDistance = 50f;

        private FrontWall _topWallInstance;
        private BackWall _bottomWallInstance;

        void Start()
        {
            PlaceFrontWall();
            PlaceBackWall();
        }

        private void PlaceFrontWall()
        {
            _topWallInstance = PlaceWall(_topScreenPoint, _frontWallPrefab, _topWallInstance);
        }

        private void PlaceBackWall()
        {
            _bottomWallInstance = PlaceWall(_bottomScreenPoint, _backWallPrefab, _bottomWallInstance);
        }

        private T PlaceWall<T>(Vector3 screenPoint, T prefab, T instance) where T : MonoBehaviour
        {
            Ray ray = _camera.ScreenPointToRay(screenPoint);

            if (Physics.Raycast(ray, out RaycastHit hit, _raycastDistance, _roadLayerMask))
            {
                if (instance == null)
                    instance = Instantiate(prefab, _camera.transform);
                else
                    instance.gameObject.SetActive(true);

                instance.transform.position = hit.point;
                instance.transform.rotation = Quaternion.identity;
            }
            else
            {
                if (instance != null)
                    instance.gameObject.SetActive(false);
            }

            return instance;
        }
    }
}