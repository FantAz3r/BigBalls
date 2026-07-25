using System.Collections;
using UnityEngine;

namespace BigBalls.GameplayObjects
{
    public class RoadColliderPlacer : MonoBehaviour
    {
        [Header("Настройки")]
        [SerializeField] private Vector3 _topOffset;
        [SerializeField] private Vector3 _bottomOffset;
        [SerializeField] private Camera _camera;
        [SerializeField] private FrontWall _frontWallPrefab;
        [SerializeField] private BackWall _backWallPrefab;
        [SerializeField] private LayerMask _roadLayerMask;
        [SerializeField] private float _raycastDistance = 50f;
        [SerializeField] private float _debugRayDuration = 2f;

        private FrontWall _topWallInstance;
        private BackWall _bottomWallInstance;

        void Start ()
        {
            StartCoroutine(WaitRoutine());
        }

        private IEnumerator WaitRoutine()
        {
            yield return new WaitForSeconds(0.5f);

            PlaceFrontWall();
            PlaceBackWall();
        }

        private void PlaceFrontWall ()
        {
            var topScreenPoint = new Vector3(Screen.width / 2f, Screen.height, 0f);
            _topWallInstance = PlaceWall(topScreenPoint, _frontWallPrefab, _topWallInstance, _topOffset);
        }

        private void PlaceBackWall ()
        {
            var bottomScreenPoint = new Vector3(Screen.width / 2f, 0f, 0f);
            _bottomWallInstance = PlaceWall(bottomScreenPoint, _backWallPrefab, _bottomWallInstance, _bottomOffset);
        }

        private T PlaceWall<T> (Vector3 screenPoint, T prefab, T instance, Vector3 offset) where T : MonoBehaviour
        {
            if (_camera == null || prefab == null)
                return instance;

            Ray ray = _camera.ScreenPointToRay(screenPoint);

            if (Physics.Raycast(ray, out RaycastHit hit, _raycastDistance, _roadLayerMask))
            {
                if (instance == null)
                    instance = Instantiate(prefab, _camera.transform);
                else
                    instance.gameObject.SetActive(true);

                instance.transform.position = hit.point + offset;
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