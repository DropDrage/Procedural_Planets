using System;
using JetBrains.Annotations;
using UnityEngine;

[RequireComponent(typeof(Camera), typeof(InputHandler))]
public class OrbitCamera : MonoBehaviour
{
    private const float NewTargetDistanceModifier = 3f;

    public GameObject Target
    {
        get => _target;
        set
        {
            _target = value;
            _minScrollDistance = Target.GetComponent<PlanetGenerator>().shapeSettings.planetRadius * 2f;
            if (Target.GetComponent<SunGenerator>() != null)
            {
                _minScrollDistance *= 1.5f;
            }

            _distanceToTarget = _minScrollDistance * NewTargetDistanceModifier;
            _followTargetUpdate = FollowTarget;
        }
    }

    public bool HasTarget => _target != null;

    [SerializeField] private float editAngle;

    private float _distanceToTarget;
    private float _minScrollDistance;

    private InputHandler _inputHandler;

    [CanBeNull] private GameObject _target;


    private readonly Action _emptyTarget = () => { };
    private Action _followTargetUpdate;

    private Camera _camera;

    private Ray _leftClickRay, _editedClickRay;
    private Vector3 _leftClickWorldPosition;


    private void Awake()
    {
        _followTargetUpdate = _emptyTarget;

        _camera = GetComponent<Camera>();

        _inputHandler = GetComponent<InputHandler>();
        _inputHandler.OnLeftClick += OnLeftClick;
        _inputHandler.OnVerticalScroll += OnVerticalScroll;
        _inputHandler.OnMouseMove += OnMouseMove;
    }

    private void LateUpdate()
    {
        _followTargetUpdate();
    }

    private void FollowTarget()
    {
        var myTransform = transform;
        var startPosition = myTransform.position;
        var endPosition = Target.transform.position - myTransform.forward * _distanceToTarget;

        myTransform.position = Vector3.Lerp(startPosition, endPosition, 0.15f);
    }

    private void Rotate(Vector2 delta)
    {
        var myTransform = transform;
        var tempMouseDelta = delta * (5 * Time.fixedDeltaTime);

        var startEulerAngles = myTransform.eulerAngles;
        var startRotation = Quaternion.Euler(startEulerAngles.x, startEulerAngles.y, 0);

        myTransform.Rotate(-tempMouseDelta.y, tempMouseDelta.x, 0, Space.Self);
        var endEulerAngles = myTransform.eulerAngles;
        var endRotation = Quaternion.Euler(endEulerAngles.x, endEulerAngles.y, 0);

        myTransform.rotation = Quaternion.Slerp(startRotation, endRotation, 0.5f);
    }

    private void OnMouseMove(Vector2 mouseDelta)
    {
        if (_inputHandler.isRightClicked)
        {
            Rotate(mouseDelta);
        }
    }

    private void OnLeftClick(Vector2 mousePosition)
    {
        print(mousePosition);
        var ray = _camera.ScreenPointToRay(mousePosition);
        _leftClickRay = ray;
        _editedClickRay =
            new Ray(ray.origin,
                Matrix4x4.Rotate(Quaternion.Euler(transform.right * editAngle)).MultiplyVector(ray.direction));
        _leftClickWorldPosition = _camera.ScreenToWorldPoint(mousePosition);

        if (Physics.Raycast(_editedClickRay, out var hit, Camera.main!.farClipPlane))
        {
            print($"target set {hit.collider.gameObject.name}");

            Target = hit.collider.gameObject;
        }

        if (Physics.Raycast(ray, out var hitEdit))
        {
            print($"Just target set {hitEdit.collider.gameObject.name}");
        }
    }

    private void OnVerticalScroll(float scroll)
    {
        var tempScroll = _distanceToTarget - scroll * Time.deltaTime;
        _distanceToTarget = ValidateDistance(tempScroll);
    }

    private float ValidateDistance(float distance) => distance > _minScrollDistance ? distance : _minScrollDistance;

    private void OnDestroy()
    {
        _inputHandler.OnLeftClick -= OnLeftClick;
        _inputHandler.OnVerticalScroll -= OnVerticalScroll;
        _inputHandler.OnMouseMove -= OnMouseMove;
    }


    private void OnDrawGizmos()
    {
        Gizmos.color = Color.cyan;
        Gizmos.DrawRay(_leftClickRay.origin, _leftClickRay.origin + _leftClickRay.direction * 10000);
        Gizmos.DrawSphere(_leftClickWorldPosition, 5f);
        Gizmos.color = Color.magenta;
        Gizmos.DrawRay(_editedClickRay.origin, _editedClickRay.origin + _editedClickRay.direction * 10000);
    }
}