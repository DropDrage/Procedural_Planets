using System;
using UnityEngine;
using UnityEngine.InputSystem;

namespace Camera_Controls
{
    public class InputHandler : MonoBehaviour
    {
        [NonSerialized] public bool isRightClicked;

        public static Vector2 MousePosition => Mouse.current.position.ReadValue(); //ToDo cache?

        public event Action<Vector2> MouseMove;
        public event Action<Vector2> LeftClick;
        public event Action<float> VerticalScroll;

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
            inputActionsUi.Click.performed += OnLeftClick;
            inputActionsUi.RightClick.started += OnRightClick;
            inputActionsUi.RightClick.performed += OnRightClick;
            inputActionsUi.RightClick.canceled += OnRightClick;
            inputActionsUi.MouseMove.performed += OnMouseMove;
            inputActionsUi.ScrollWheel.performed += OnScrollMouse;
        }

        private void OnMouseMove(InputAction.CallbackContext context)
        {
            var mouseDelta = context.action.ReadValue<Vector2>();
            MouseMove?.Invoke(mouseDelta);
        }

        private void OnLeftClick(InputAction.CallbackContext context)
        {
            print(MousePosition);
            LeftClick?.Invoke(MousePosition);
        }

        private void OnRightClick(InputAction.CallbackContext context)
        {
            isRightClicked = context.phase != InputActionPhase.Canceled;
        }

        private void OnScrollMouse(InputAction.CallbackContext context)
        {
            VerticalScroll?.Invoke(context.action.ReadValue<Vector2>().y);
        }


        private void OnDestroy()
        {
            var inputActionsUi = _inputActions.UI;
            inputActionsUi.Click.performed -= OnLeftClick;
            inputActionsUi.RightClick.started -= OnRightClick;
            inputActionsUi.RightClick.performed -= OnRightClick;
            inputActionsUi.RightClick.canceled -= OnRightClick;
            inputActionsUi.MouseMove.performed -= OnMouseMove;
            inputActionsUi.ScrollWheel.performed -= OnScrollMouse;
        }
    }
}
