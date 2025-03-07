using System.Collections;
using System.Collections.Generic;
using RBPT.Inputs;
using UnityEngine;

namespace RBPT.Gameplay
{
    public class Picker : MonoBehaviour
    {
        [Header("Injection Fields")]
        [SerializeField] private InputReader m_Input;
        [SerializeField] private Camera m_MainCamera;

        [Header("Selected Token")]
        [SerializeField] private GameObject m_pickedupToken;
        
        [Header("Serialize Private Fields")]
        [SerializeField] private float m_hoverHeight;
        [SerializeField] private Vector3 m_mousePosition;
        [SerializeField] private bool m_isPicking;
        [SerializeField] private Vector3 m_initialTokenPosition;
        [SerializeField] private bool m_snapToGid;

        private void Start()
        {
            m_MainCamera = Camera.main;

            m_isPicking = false;
            m_snapToGid = false;

            // Performed Events Subscriptions
            m_Input.MousePositionEvent += HandleMousePosition;
            m_Input.Mouse1Performed += HandlePickingUp;
            m_Input.EscapePerformed += CancelTokenPickup;
            m_Input.ControlPerformed += ToggleSnapToGrid;

            // Canceled Events Subscriptions
            m_Input.Mouse1Canceled += HandleDropping;
            m_Input.ControlCanceled += ToggleSnapToGrid;
        }
        private void OnDisable()
        {
            // Performed Events Unsubscriptions
            m_Input.MousePositionEvent -= HandleMousePosition;
            m_Input.Mouse1Performed -= HandlePickingUp;
            m_Input.EscapePerformed -= CancelTokenPickup;
            m_Input.ControlPerformed -= ToggleSnapToGrid;

            // Canceled Events Unsubscriptions
            m_Input.Mouse1Canceled -= HandleDropping;
            m_Input.ControlCanceled -= ToggleSnapToGrid;
        }
        private void Update()
        {
            if(m_isPicking)
            {
                PickUpToken();
                DragToken();
            }
            if(!m_isPicking && m_pickedupToken != null)
            {
                DropToken();
            }
        }
        ////////////////////////////////////////////////////////////////////////////////////////////////////////////
        // =========================================================================================================
        // Input Events Methods
        // =========================================================================================================
        private void HandlePickingUp()
        {
            m_isPicking = true;
        }
        private void HandleDropping()
        {
            m_isPicking = false;
        }
        private void HandleMousePosition(Vector2 pos)
        {
            m_mousePosition = pos;
        }
        private void CancelTokenPickup()
        {
            m_pickedupToken.transform.position = m_initialTokenPosition;
            m_isPicking = false;
            m_pickedupToken = null;
            m_Input.SetCursorVisibility(true);
        }
        private void ToggleSnapToGrid()
        {
            m_snapToGid = !m_snapToGid;
        }
        ////////////////////////////////////////////////////////////////////////////////////////////////////////////
        // =========================================================================================================
        // PickUp Drag Drop Update Methods
        // =========================================================================================================
        private void PickUpToken()
        {
            if(m_pickedupToken != null)
            {
                return;
            }

            RaycastHit hit = CastRay();

            if(hit.collider != null)
            {
                if(!hit.collider.CompareTag("Token"))
                {
                    return;
                }

                m_initialTokenPosition = hit.collider.transform.position;
                m_pickedupToken = hit.collider.gameObject;
                m_Input.SetCursorVisibility(false);
            }
        }
        private void DragToken()
        {
            if(m_pickedupToken == null)
            {
                return;
            }

            UpdateTokenPosition(m_hoverHeight);
        }
        private void DropToken()
        {
            UpdateTokenPosition(0.25f); // TODO : Make this 0.25f value universal "ground level" for every token. A static value to fetch from anywhere and insert in such methods
            m_pickedupToken = null;
            m_Input.SetCursorVisibility(true);
        }
        ////////////////////////////////////////////////////////////////////////////////////////////////////////////
        // =========================================================================================================
        // Internal Logic Methods
        // =========================================================================================================
        private void UpdateTokenPosition(float yPos)
        {
            Vector3 position = new Vector3(m_mousePosition.x, m_mousePosition.y, m_MainCamera.WorldToScreenPoint(m_pickedupToken.transform.position).z);
            Vector3 worldPosition = m_MainCamera.ScreenToWorldPoint(position);

            if(m_snapToGid)
            {
                m_pickedupToken.transform.position = new Vector3(Mathf.Round(worldPosition.x * 2) / 2, yPos, Mathf.Round(worldPosition.z * 2) / 2);
            }
            else
            {
                m_pickedupToken.transform.position = new Vector3(worldPosition.x, yPos, worldPosition.z);
            }
        }
        private RaycastHit CastRay()
        {
            Vector3 screenMousePosFar = new Vector3(m_mousePosition.x, m_mousePosition.y, m_MainCamera.farClipPlane);
            Vector3 screenMousePosNear = new Vector3(m_mousePosition.x, m_mousePosition.y, m_MainCamera.nearClipPlane);

            Vector3 worldMousePosFar = m_MainCamera.ScreenToWorldPoint(screenMousePosFar);
            Vector3 worldMousePosNear = m_MainCamera.ScreenToWorldPoint(screenMousePosNear);
            
            RaycastHit hit;

            Physics.Raycast(worldMousePosNear, worldMousePosFar - worldMousePosNear, out hit);
            
            return hit;
        }
    }
}
