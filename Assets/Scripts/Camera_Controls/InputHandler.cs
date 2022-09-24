using System;
using UnityEngine;
using UnityEngine.InputSystem;

namespace Camera_Controls
{
    public class InputHandler : MonoBehaviour
    {
        [NonSerialized] public bool isRightClicked;

        public Vector2 mousePosition;

        public event Action<Vector2> OnMouseMove;
        public event Action<Vector2> OnLeftClick;
        public event Action<float> OnVerticalScroll;

        private InputActions _inputActions;


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
            _inputActions = new InputActions();

            var inputActionsUi = _inputActions.UI;
            inputActionsUi.Point.started += MousePositionChange;
            inputActionsUi.Click.performed += LeftClick;
            inputActionsUi.RightClick.started += RightClick;
            inputActionsUi.RightClick.performed += RightClick;
            inputActionsUi.RightClick.canceled += RightClick;
            inputActionsUi.MouseMove.performed += MouseMove;
            inputActionsUi.ScrollWheel.performed += OnScrollMouse;
        }

        private void MouseMove(InputAction.CallbackContext context)
        {
            var mouseDelta = context.action.ReadValue<Vector2>();
            OnMouseMove?.Invoke(mouseDelta);
        }

        private void MousePositionChange(InputAction.CallbackContext context)
        {
            mousePosition = context.action.ReadValue<Vector2>();
        }

        private void LeftClick(InputAction.CallbackContext context)
        {
            print(mousePosition);
            OnLeftClick?.Invoke(mousePosition);
        }

        private void RightClick(InputAction.CallbackContext context)
        {
            isRightClicked = context.phase != InputActionPhase.Canceled;
        }

        private void OnScrollMouse(InputAction.CallbackContext context)
        {
            OnVerticalScroll?.Invoke(context.action.ReadValue<Vector2>().y);
        }


        private void OnDestroy()
        {
            var inputActionsUi = _inputActions.UI;
            inputActionsUi.Point.started -= MousePositionChange;
            inputActionsUi.Click.performed -= LeftClick;
            inputActionsUi.RightClick.started -= RightClick;
            inputActionsUi.RightClick.performed -= RightClick;
            inputActionsUi.RightClick.canceled -= RightClick;
            inputActionsUi.MouseMove.performed -= MouseMove;
            inputActionsUi.ScrollWheel.performed -= OnScrollMouse;
        }
    }
}
