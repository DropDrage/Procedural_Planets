using System;
using JetBrains.Annotations;
using UnityEngine;
using UnityEngine.InputSystem;

public class OrbitCamera : MonoBehaviour
{
    private const float NewTargetDistanceModifier = 3f;


    private InputActions _inputActions;

    private Vector2 _mousePosition;
    private Vector2 _mouseDelta;

    private float _distanceToTarget;
    private float _minScrollDistance;

    // private Ve
    public GameObject Target
    {
        get => _target;
        set
        {
            _target = value;
            _minScrollDistance = Target.GetComponent<Planet>().shapeSettings.planetRadius * 2f;
            if (Target.GetComponent<Sun>() != null)
            {
                _minScrollDistance *= 1.5f;
            }

            _distanceToTarget = _minScrollDistance * NewTargetDistanceModifier;
            _followTargetUpdate = _followTarget;
        }
    }

    public bool HasTarget => _target != null;
    [CanBeNull] private GameObject _target;

    private bool _rightClicked;


    private readonly Action _emptyTarget = () => { };
    private Action _followTarget;

    private Action _followTargetUpdate;


    private Vector3 leftClickPosition;
    private Vector3 leftClickDirection;


    private void OnEnable()
    {
        _inputActions.Enable();
    }

    private void OnDisable()
    {
        _inputActions.Disable();
    }

    private void Awake()
    {
        _followTarget = () =>
        {
            if (_rightClicked)
            {
                Rotate();
            }

            Move();
        };
        _followTargetUpdate = _emptyTarget;


        _inputActions = new InputActions();

        _inputActions.UI.Point.started += OnMouseMove;
        _inputActions.UI.Click.performed += OnLeftClick;
        _inputActions.UI.RightClick.started += OnRightClick;
        _inputActions.UI.RightClick.performed += OnRightClick;
        _inputActions.UI.RightClick.canceled += OnRightClick;
        _inputActions.UI.MouseMove.performed += OnRightMouseMove;
        _inputActions.UI.ScrollWheel.performed += OnScrollMouse;
    }

    private void LateUpdate()
    {
        _followTargetUpdate();
    }

    private void Move()
    {
        var myTransform = transform;
        var endPosition = Target.transform.position - myTransform.forward * _distanceToTarget;
        var startPosition = myTransform.position;

        myTransform.position = Vector3.Lerp(startPosition, endPosition, 0.15f);
    }

    private void Rotate()
    {
        var myTransform = transform;
        var tempMouseDelta = _mouseDelta * (5 * Time.fixedDeltaTime);

        var startEulerAngles = myTransform.eulerAngles;
        var startRotation = Quaternion.Euler(startEulerAngles.x, startEulerAngles.y, 0);

        myTransform.Rotate(-tempMouseDelta.y, tempMouseDelta.x, 0, Space.Self);
        var endEulerAngles = myTransform.eulerAngles;
        var endRotation = Quaternion.Euler(endEulerAngles.x, endEulerAngles.y, 0);

        myTransform.rotation = Quaternion.Slerp(startRotation, endRotation, 0.5f);
    }

    private void OnRightMouseMove(InputAction.CallbackContext context)
    {
        _mouseDelta = context.action.ReadValue<Vector2>();
    }

    private void OnMouseMove(InputAction.CallbackContext context)
    {
        _mousePosition = context.action.ReadValue<Vector2>();
    }

    private void OnLeftClick(InputAction.CallbackContext context)
    {
        var ray = Camera.main!.ScreenPointToRay(_mousePosition);

        leftClickPosition = transform.position;
        leftClickDirection = ray.direction;

        if (Physics.Raycast(ray, out var hit))
        {
            print($"target set {hit.collider.gameObject.name}");

            Target = hit.collider.gameObject;
        }
    }

    private void OnRightClick(InputAction.CallbackContext context)
    {
        _rightClicked = context.phase != InputActionPhase.Canceled;
    }

    private void OnScrollMouse(InputAction.CallbackContext context)
    {
        var tempScroll = _distanceToTarget - context.action.ReadValue<Vector2>().y * Time.fixedDeltaTime;
        _distanceToTarget = ValidateDistance(tempScroll);
    }

    private float ValidateDistance(float distance) => distance > _minScrollDistance ? distance : _minScrollDistance;


    private void OnDrawGizmos()
    {
        Gizmos.color = Color.cyan;
        Gizmos.DrawRay(leftClickPosition, leftClickPosition + leftClickDirection * 10000);
    }
}