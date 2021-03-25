using System;
using UnityEngine;
using UnityEngine.InputSystem;

public class Camera : MonoBehaviour
{
    private InputActions _inputActions;

    private Vector2 _mousePosition;
    private Vector2 _mouseDelta;

    private float _distanceToTarget;
    private float _minScrollDistance;

    // private Ve
    private GameObject _target;
    private bool _rightClicked;


    private Action emptyTarget = () => { };
    private Action _followTarget;

    private Action _followTargetUpdate;


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
        _followTargetUpdate = emptyTarget;


        _inputActions = new InputActions();

        _inputActions.UI.Point.started += OnMouseMove;
        _inputActions.UI.Click.performed += OnLeftClick;
        _inputActions.UI.RightClick.started += OnRightClick;
        _inputActions.UI.RightClick.performed += OnRightClick;
        _inputActions.UI.RightClick.canceled += OnRightClick;
        _inputActions.UI.MouseMove.performed += OnRightMouseMove;
        _inputActions.UI.ScrollWheel.performed += OnScrollMouse;
    }

    private void Update()
    {
        _followTargetUpdate();
    }

    private void Move()
    {
        var myTransform = transform;
        var endPosition = _target.transform.position - myTransform.forward * _distanceToTarget;
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
        var ray = UnityEngine.Camera.main!.ScreenPointToRay(_mousePosition);

        if (Physics.Raycast(ray, out var hit))
        {
            print($"target set {hit.collider.gameObject.name}");

            _target = hit.collider.gameObject;
            _minScrollDistance = _target.GetComponent<Planet>().shapeSettings.planetRadius * 2f;
            if (_target.GetComponent<Sun>() != null)
            {
                _minScrollDistance *= 1.5f;
            }

            _distanceToTarget =
                ValidateDistance((_target.transform.position - transform.position).magnitude + _minScrollDistance);

            _followTargetUpdate = _followTarget;
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
}