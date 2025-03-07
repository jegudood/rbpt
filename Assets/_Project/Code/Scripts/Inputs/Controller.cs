using System.Collections;
using Cinemachine;
using UnityEngine;

namespace RBPT.Inputs
{
    public class Controller : MonoBehaviour
    {
        [Header("Injection Fields")]
        [SerializeField] private InputReader m_Input;
        [SerializeField] private Camera m_MainCamera;
        [SerializeField] private CinemachineVirtualCamera m_CinemachineVirtualCamera;

        [Header("Serialized Private Fields")]
        [SerializeField] private float m_movementSpeed;
        [SerializeField] private float m_boostedSpeed;
        [SerializeField] private float m_rotationSpeed;
        [SerializeField] private float m_zoomSpeed;
        [SerializeField] private float m_zoomMagnitude;     //TODO: This will be the one influenced in settings, not zoomSpeed
        [SerializeField] private float m_followOffsetMin;
        [SerializeField] private float m_followOffsetMax;
        [SerializeField] private Vector3 m_followOffset;
        [SerializeField] private float m_orthographicSize;
        [SerializeField] private float m_orthographicSizeMin;
        [SerializeField] private float m_orthographicSizeMax;
        [SerializeField] private float m_verticalRotation;
        [SerializeField] private float m_previousPerspectiveAngle;
        [SerializeField] private const float MaxVerticalAngle = 83.0f;
        [SerializeField] private const float MinVerticalAngle = 1.0f;
        [SerializeField] private const float OrthographicVerticalAngle = 90.00f;

        private Vector2 m_moveDirection;
        private Vector2 m_rotationDirection;
        private Vector2 m_scrollDirection;
        private bool m_enableCameraRotation;
        private bool m_enableCameraSpeedBoost;
        private bool m_orthographicMode;
        private float m_boostSpeedValue;
        private bool m_cameraIsBusy;

        private void Start()
        {
            m_MainCamera = Camera.main;

            //TODO: Enable Inputs from a Manager or something
            m_Input.EnableInputs();

            m_enableCameraRotation = false;
            m_enableCameraSpeedBoost = false;
            m_orthographicMode = false;
            m_boostSpeedValue = 1.0f;
            m_followOffset = m_CinemachineVirtualCamera.GetCinemachineComponent<CinemachineTransposer>().m_FollowOffset;

            // Performed Events
            m_Input.MovementEvent += HandleCameraMovement;
            m_Input.MouseMovementPerformed += HandleCameraRotation;
            m_Input.Mouse2Performed += EnableCameraRotation;
            m_Input.MouseScrollEvent += HandleCameraZoom;
            m_Input.ShiftPerformed += EnableCameraSpeedBoost;
            m_Input.TabPerformed += ToggleCameraMode;

            // Canceled Events
            m_Input.MouseMovementCanceled += HandleCameraRotation;
            m_Input.Mouse2Canceled += EnableCameraRotation;
            m_Input.ShiftCanceled += EnableCameraSpeedBoost;

            
        }
        private void OnDisable()
        {
            // Performed Events
            m_Input.MovementEvent -= HandleCameraMovement;
            m_Input.MouseMovementPerformed -= HandleCameraRotation;
            m_Input.Mouse2Performed -= EnableCameraRotation;
            m_Input.MouseScrollEvent -= HandleCameraZoom;
            m_Input.ShiftPerformed -= EnableCameraSpeedBoost;
            m_Input.TabPerformed -= ToggleCameraMode;

            // Canceled Events
            m_Input.MouseMovementCanceled -= HandleCameraRotation;
            m_Input.Mouse2Canceled -= EnableCameraRotation;
            m_Input.ShiftCanceled -= EnableCameraSpeedBoost;

        }
        private void LateUpdate()
        {
            if(!m_cameraIsBusy)
            {
                MoveCamera();
                RotateCamera();
                ZoomCamera();
            }
        }
        ////////////////////////////////////////////////////////////////////////////////////////////////////////////
        // =========================================================================================================
        // Camera Inputs Methods
        // =========================================================================================================
        private void HandleCameraMovement(Vector2 dir)
        {
            m_moveDirection = dir;
        }
        private void HandleCameraRotation(Vector2 dir)
        {
            m_rotationDirection = dir;
        }
        private void ToggleCameraMode()
        {
            if(!m_cameraIsBusy)
            {
                m_orthographicMode = !m_orthographicMode;
            }
            CameraModeTransition();
        }
        ////////////////////////////////////////////////////////////////////////////////////////////////////////////
        // =========================================================================================================
        // Mouse Methods
        // =========================================================================================================
        private void EnableCameraRotation()
        {
            m_enableCameraRotation = !m_enableCameraRotation;
        }
        private void HandleCameraZoom(Vector2 dir)
        {
            m_scrollDirection = dir.normalized * m_zoomMagnitude;
        }
        ////////////////////////////////////////////////////////////////////////////////////////////////////////////
        // =========================================================================================================
        // Keyboard Methods
        // =========================================================================================================
        private void EnableCameraSpeedBoost()
        {
            m_enableCameraSpeedBoost = !m_enableCameraSpeedBoost;
            m_boostSpeedValue = m_enableCameraSpeedBoost? m_boostedSpeed : 1.0f;
        }
        private void CameraModeTransition()
        {
            if(!m_orthographicMode)
            {
                StartCoroutine(LerpCameraToTargetAngle(m_previousPerspectiveAngle, false));
            }
            if(m_orthographicMode)
            {
                StartCoroutine(LerpCameraToTargetAngle(OrthographicVerticalAngle, true));
            }
        }
        ////////////////////////////////////////////////////////////////////////////////////////////////////////////
        // =========================================================================================================
        // Camera Movement Methods
        // =========================================================================================================
        private void MoveCamera()
        {
            if(m_moveDirection == Vector2.zero)
            {
                return;
            }

            Vector3 inputDir = new Vector3(m_moveDirection.x, 0.0f, m_moveDirection.y);

            Quaternion cameraRotation = Quaternion.Euler(0.0f, m_MainCamera.transform.eulerAngles.y, 0.0f);

            Vector3 moveDir = cameraRotation * inputDir;

            transform.position += new Vector3(moveDir.x, 0.0f, moveDir.z) * (m_movementSpeed * m_boostSpeedValue * Time.deltaTime);
        }
        private void RotateCamera()
        {
            if(!m_enableCameraRotation)
            {
                return;
            }

            float horizontalRotation = m_rotationDirection.x * m_rotationSpeed * Time.deltaTime;

            transform.Rotate(0.0f, horizontalRotation, 0.0f, Space.World);

            if(!m_orthographicMode)
            {
                m_verticalRotation -= m_rotationDirection.y * m_rotationSpeed * Time.deltaTime;
                m_verticalRotation = Mathf.Clamp(m_verticalRotation, MinVerticalAngle, MaxVerticalAngle);
                m_previousPerspectiveAngle = m_verticalRotation;
            }

            transform.eulerAngles = new Vector3(m_verticalRotation, transform.eulerAngles.y, 0.0f);
        }
        private void ZoomCamera()
        {
            if(m_orthographicMode)
            {
                ZoomCameraOrthoMode();
            }

            Vector3 zoomDir = new Vector3(0.0f, 0.0f, m_scrollDirection.y);

            if(m_followOffset.magnitude < m_followOffsetMin)
            {
               m_followOffset.z = -m_followOffsetMin;
            }
            if(m_followOffset.magnitude > m_followOffsetMax)
            {
                m_followOffset.z = -m_followOffsetMax;
            }

            m_followOffset.z += zoomDir.z;
            m_orthographicSize = -m_followOffset.z / 2;
            m_CinemachineVirtualCamera.GetCinemachineComponent<CinemachineTransposer>().m_FollowOffset = Vector3.Lerp(m_CinemachineVirtualCamera.GetCinemachineComponent<CinemachineTransposer>().m_FollowOffset, m_followOffset, m_zoomSpeed * Time.deltaTime);
        }
        private void ZoomCameraOrthoMode()
        {
            m_orthographicSize -= m_scrollDirection.y;
            m_followOffset.z = -m_orthographicSize * 2;
            m_orthographicSize = Mathf.Clamp(m_orthographicSize, m_orthographicSizeMin, m_orthographicSizeMax);

            m_CinemachineVirtualCamera.m_Lens.OrthographicSize = Mathf.Lerp(m_CinemachineVirtualCamera.m_Lens.OrthographicSize, m_orthographicSize, m_zoomSpeed * Time.deltaTime);
        }
        private void ToggleTopDownView()
        {
            m_CinemachineVirtualCamera.m_Lens.Orthographic = m_orthographicMode;
        }
        private IEnumerator LerpCameraToTargetAngle(float targetAngle, bool endModeIsOrtho)
        {
            if(!endModeIsOrtho)
            {
                ToggleTopDownView();
            }

            m_cameraIsBusy = true;
            float snapThreshold = 1.0f;

            while(true)
            {
                m_verticalRotation = transform.eulerAngles.x;
                float angleDifference = Mathf.DeltaAngle(m_verticalRotation, targetAngle);

                if(Mathf.Abs(angleDifference) < snapThreshold)
                {
                    m_verticalRotation = targetAngle;
                    transform.eulerAngles = new Vector3(m_verticalRotation, transform.eulerAngles.y, 0.0f);
                    break;
                }

                float eulerX = Mathf.LerpAngle(m_verticalRotation, targetAngle, 10.0f * Time.deltaTime);
                transform.eulerAngles = new Vector3(eulerX, transform.eulerAngles.y, transform.eulerAngles.z);

                if(!endModeIsOrtho)
                {
                    m_CinemachineVirtualCamera.GetCinemachineComponent<CinemachineTransposer>().m_FollowOffset = Vector3.Lerp(m_CinemachineVirtualCamera.GetCinemachineComponent<CinemachineTransposer>().m_FollowOffset, m_followOffset, m_zoomSpeed * Time.deltaTime);
                }

                yield return new WaitForEndOfFrame();
            }

            if(endModeIsOrtho)
            {
                m_CinemachineVirtualCamera.m_Lens.OrthographicSize = m_orthographicSize;
                ToggleTopDownView();
            }
            m_cameraIsBusy = false;
        }
    }
}
