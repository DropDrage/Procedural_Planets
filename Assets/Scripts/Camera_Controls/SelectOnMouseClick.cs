using UnityEngine;

namespace Camera_Controls
{
    [RequireComponent(typeof(InputHandler), typeof(Camera), typeof(OrbitCamera))]
    public class SelectOnMouseClick : MonoBehaviour
    {
        private InputHandler _inputHandler;

        private Camera _camera;
        private OrbitCamera _orbitCamera;

        private Ray _leftClickRay;


        private void Start()
        {
            _camera = GetComponent<Camera>();
            _orbitCamera = GetComponent<OrbitCamera>();

            _inputHandler = GetComponent<InputHandler>();
            _inputHandler.LeftClick += OnLeftClick;
        }

        private void OnDestroy()
        {
            _inputHandler.LeftClick -= OnLeftClick;
        }


        private void OnLeftClick(Vector2 mousePosition)
        {
            var ray = _camera.ScreenPointToRay(mousePosition);
            _leftClickRay = ray;

            if (Physics.Raycast(ray, out var hit))
            {
                print($"target set {hit.collider.gameObject.name}");

                _orbitCamera.Target = hit.collider.gameObject;
            }
        }


        private void OnDrawGizmos()
        {
            Gizmos.color = Color.cyan;
            Gizmos.DrawRay(_leftClickRay.origin, _leftClickRay.origin + _leftClickRay.direction * 10000);
            // Gizmos.DrawSphere(_leftClickWorldPosition, 5f);
        }
    }
}
