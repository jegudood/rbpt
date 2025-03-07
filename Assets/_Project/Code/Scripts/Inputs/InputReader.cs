using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

namespace RBPT.Inputs
{
    [CreateAssetMenu(fileName = "InputReader", menuName = "Project/Input Reader", order = 1)]
    public class InputReader : ScriptableObject, PlayerInputs.IMainInputsActions
    {
        private PlayerInputs m_PlayerInputs;
        ////////////////////////////////////////////////////////////////////////////////////////////////////////////
        // =========================================================================================================
        // Events for Movement Actions | Vector2
        // =========================================================================================================
        public event Action<Vector2> MovementEvent;
        public void OnMovement(InputAction.CallbackContext context)
        {
            MovementEvent?.Invoke(context.ReadValue<Vector2>());
        }
        ////////////////////////////////////////////////////////////////////////////////////////////////////////////
        // =========================================================================================================
        // Events for MouseMovement Actions | Vector2
        // =========================================================================================================
        public event Action<Vector2> MouseMovementPerformed;
        public event Action<Vector2> MouseMovementCanceled;
        public void OnLookAround(InputAction.CallbackContext context)
        {
            if(context.phase == InputActionPhase.Performed)
            {
                MouseMovementPerformed?.Invoke(context.ReadValue<Vector2>());
            }
            if(context.phase == InputActionPhase.Canceled)
            {
                MouseMovementCanceled?.Invoke(context.ReadValue<Vector2>());
            }
        }
        ////////////////////////////////////////////////////////////////////////////////////////////////////////////
        // =========================================================================================================
        // Events for MousePosition Actions | Vector2
        // =========================================================================================================
        public event Action<Vector2> MousePositionEvent;
        public void OnMousePosition(InputAction.CallbackContext context)
        {
            MousePositionEvent?.Invoke(context.ReadValue<Vector2>());
        }
        ////////////////////////////////////////////////////////////////////////////////////////////////////////////
        // =========================================================================================================
        // Events for Mouse1 Actions | Int
        // =========================================================================================================
        public event Action Mouse1Performed;
        public event Action Mouse1Canceled;
        public void OnMouse1(InputAction.CallbackContext context)
        {
            if(context.phase == InputActionPhase.Performed)
            {
                Mouse1Performed?.Invoke();
            }
            if(context.phase == InputActionPhase.Canceled)
            {
                Mouse1Canceled?.Invoke();
            }
        }
        ////////////////////////////////////////////////////////////////////////////////////////////////////////////
        // =========================================================================================================
        // Events for Mouse2 Actions | Int
        // =========================================================================================================
        public event Action Mouse2Performed;
        public event Action Mouse2Canceled;
        public void OnMouse2(InputAction.CallbackContext context)
        {
            if(context.phase == InputActionPhase.Performed)
            {
                Mouse2Performed?.Invoke();
            }
            if(context.phase == InputActionPhase.Canceled)
            {
                Mouse2Canceled?.Invoke();
            }
        }
        ////////////////////////////////////////////////////////////////////////////////////////////////////////////
        // =========================================================================================================
        // Events for MiddleMouse Actions | Float
        // =========================================================================================================
        public event Action MiddleMousePerformed;
        public event Action MiddleMouseCanceled;
        public void OnMiddleMouse(InputAction.CallbackContext context)
        {
            if(context.phase == InputActionPhase.Performed)
            {
                MiddleMousePerformed?.Invoke();
            }
            if(context.phase == InputActionPhase.Canceled)
            {
                MiddleMouseCanceled?.Invoke();
            }
        }
        ////////////////////////////////////////////////////////////////////////////////////////////////////////////
        // =========================================================================================================
        // Events for MouseScroll Actions | Vector2.y
        // =========================================================================================================
        public event Action<Vector2> MouseScrollEvent;
        public void OnMouseScroll(InputAction.CallbackContext context)
        {
            MouseScrollEvent?.Invoke(context.ReadValue<Vector2>());
        }
        ////////////////////////////////////////////////////////////////////////////////////////////////////////////
        // =========================================================================================================
        // Events for Shift Actions | Int
        // =========================================================================================================
        public event Action ShiftPerformed;
        public event Action ShiftCanceled;
        public void OnShift(InputAction.CallbackContext context)
        {
            if(context.phase == InputActionPhase.Performed)
            {
                ShiftPerformed?.Invoke();
            }
            if(context.phase == InputActionPhase.Canceled)
            {
                ShiftCanceled?.Invoke();
            }
        }
        ////////////////////////////////////////////////////////////////////////////////////////////////////////////
        // =========================================================================================================
        // Events for Tab Actions | Int
        // =========================================================================================================
        public event Action TabPerformed;
        public event Action TabCanceled;
        public void OnTab(InputAction.CallbackContext context)
        {
            if(context.phase == InputActionPhase.Performed)
            {
                TabPerformed?.Invoke();
            }
            if(context.phase == InputActionPhase.Canceled)
            {
                TabCanceled?.Invoke();
            }
        }
        ////////////////////////////////////////////////////////////////////////////////////////////////////////////
        // =========================================================================================================
        // Events for Escape Actions | Int
        // =========================================================================================================
        public event Action EscapePerformed;
        public event Action EscapeCanceled;
        public void OnEscape(InputAction.CallbackContext context)
        {
            if(context.phase == InputActionPhase.Performed)
            {
                EscapePerformed?.Invoke();
            }
            if(context.phase == InputActionPhase.Canceled)
            {
                EscapeCanceled?.Invoke();
            }
        }
        ////////////////////////////////////////////////////////////////////////////////////////////////////////////
        // =========================================================================================================
        // Events for Control Actions | Int
        // =========================================================================================================
        public event Action ControlPerformed;
        public event Action ControlCanceled;
        public void OnControl(InputAction.CallbackContext context)
        {
            if(context.phase == InputActionPhase.Performed)
            {
                ControlPerformed?.Invoke();
            }
            if(context.phase == InputActionPhase.Canceled)
            {
                ControlCanceled?.Invoke();
            }
        }
        ////////////////////////////////////////////////////////////////////////////////////////////////////////////
        // =========================================================================================================
        // Private Methods
        // =========================================================================================================
        private void OnEnable()
        {
            if(m_PlayerInputs == null)
            {
                m_PlayerInputs = new PlayerInputs();
                m_PlayerInputs.MainInputs.SetCallbacks(this);
            }
        }
        private void OnDisable()
        {
        }
        private void ResetCursor()
        {
            Cursor.visible = true;
            Cursor.lockState = CursorLockMode.None;
        }
        ////////////////////////////////////////////////////////////////////////////////////////////////////////////
        // =========================================================================================================
        // Public Methods
        // =========================================================================================================
        public void SetCursorVisibility(bool visible)
        {
            Cursor.visible = visible;
        }
        public void SetCursorVisibilityAndLockMode(bool visible, CursorLockMode mode)
        {
            Cursor.visible = visible;
            Cursor.lockState = mode;
        }
        public void EnableInputs()
        {
            m_PlayerInputs.MainInputs.Enable();
        }
        public void DisableInputs()
        {
            m_PlayerInputs.MainInputs.Disable();
        }
    }
}
