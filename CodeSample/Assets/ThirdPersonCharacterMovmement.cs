using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

namespace PlayerControls
{
    [RequireComponent(typeof(PlayerInput))]

    public class ThirdPersonCharacterMovmement : MonoBehaviour
    {
        //ThirdPersonCharacterMovmement Variables
        public float moveSpeed = 5f; // Adjust the speed as needed

        private Rigidbody rb;

        private PlayerInput _playerInput;
        private PlayerAssetsInput _input;

        private GameObject _mainCamera;

        private const float _threshold = 0.01f;

        [Header("Cinemachine")]
        [Tooltip("The follow target set in the Cinemachine Virtual Camera that the camera will follow")]
        public GameObject CinemachineCameraTarget;

        [Tooltip("How far in degrees can you move the camera up")]
        public float TopClamp = 70.0f;

        [Tooltip("How far in degrees can you move the camera down")]
        public float BottomClamp = -30.0f;

        [Tooltip("Additional degress to override the camera. Useful for fine tuning camera position when locked")]
        public float CameraAngleOverride = 0.0f;

        [Tooltip("For locking the camera position on all axis")]
        public bool LockCameraPosition = false;

        // cinemachine
        private float _cinemachineTargetYaw;
        private float _cinemachineTargetPitch;

        private float _targetRotation = 0.0f;
        private float _rotationVelocity;
        private float _verticalVelocity;

        [Tooltip("How fast the character turns to face movement direction")]
        [Range(0.0f, 0.3f)]
        public float RotationSmoothTime = 0.12f;

        public PlatformLogic platformLogic;

        public float radius = 0.5f;
        public float yOffset = 0.0f; // Offset distance from the center of the object in the y-axis
        public LayerMask groundLayer;
        public bool isGrounded = false;

        private bool IsCurrentDeviceMouse
        {
            get
            {
                return _playerInput.currentControlScheme == "KeyboardMouse";
            }
        }

        private void Awake()
        {
            // get a reference to our main camera
            if (_mainCamera == null)
            {
                _mainCamera = GameObject.FindGameObjectWithTag("MainCamera");
            }
        }

        void Start()
        {
            rb = GetComponent<Rigidbody>();
            _cinemachineTargetYaw = CinemachineCameraTarget.transform.rotation.eulerAngles.y;
            
            // Make sure the Rigidbody has constraints on rotation to avoid unexpected behavior
            // rb.freezeRotation = true;

            _input = GetComponent<PlayerAssetsInput>();
            _playerInput = GetComponent<PlayerInput>();

        }

        void Update()
        {
            // Call the function to handle player input
            HandleInput();
        }

        private void LateUpdate()
        {
            CameraRotation();
        }

        void HandleInput()
        {
            // Move the character using forces
            GroundCheck();
            MoveCharacter();
            LiftCharacter();
        }

        private void CameraRotation()
        {
            // if there is an input and camera position is not fixed
            if (_input.look.sqrMagnitude >= _threshold && !LockCameraPosition)
            {
                //Don't multiply mouse input by Time.deltaTime;
                float deltaTimeMultiplier = IsCurrentDeviceMouse ? 1.0f : Time.deltaTime;

                _cinemachineTargetYaw += _input.look.x * deltaTimeMultiplier;
                _cinemachineTargetPitch += _input.look.y * deltaTimeMultiplier;
            }

            // clamp our rotations so our values are limited 360 degrees
            _cinemachineTargetYaw = ClampAngle(_cinemachineTargetYaw, float.MinValue, float.MaxValue);
            _cinemachineTargetPitch = ClampAngle(_cinemachineTargetPitch, BottomClamp, TopClamp);

            // Cinemachine will follow this target
            CinemachineCameraTarget.transform.rotation = Quaternion.Euler(_cinemachineTargetPitch + CameraAngleOverride,
                _cinemachineTargetYaw, 0.0f);
        }

        void MoveCharacter()
        {
            Vector3 inputDirection = new Vector3(_input.move.x, 0.0f, _input.move.y).normalized;

            // note: Vector2's != operator uses approximation so is not floating point error prone, and is cheaper than magnitude
            // if there is a move input rotate player when the player is moving
            if (_input.move != Vector2.zero)
            {
                _targetRotation = Mathf.Atan2(inputDirection.x, inputDirection.z) * Mathf.Rad2Deg +
                                  _mainCamera.transform.eulerAngles.y;
                float rotation = Mathf.SmoothDampAngle(transform.eulerAngles.y, _targetRotation, ref _rotationVelocity,
                    RotationSmoothTime);

                // rotate to face input direction relative to camera position
                //transform.rotation = Quaternion.Euler(0.0f, rotation, 0.0f);
            

                Vector3 targetDirection = Quaternion.Euler(0.0f, _targetRotation, 0.0f) * Vector3.forward;
                
                // Calculate the desired movement vector based on input and speed
                Vector3 movement = targetDirection * moveSpeed * Time.deltaTime;

                // Apply the force to the Rigidbody
                rb.AddForce(movement);
            }
        }
        void LiftCharacter()
        {
            if(_input.lift)
            {
                platformLogic.Lift();
            }
            else
            {
                platformLogic.Reset();
            }
        }

        void GroundCheck()
        {
            // Perform a sphere cast downwards to check for ground
            RaycastHit hit;
            if (Physics.SphereCast(transform.position + Vector3.up * yOffset, radius, Vector3.down, out hit, Mathf.Infinity, groundLayer))
            {
                isGrounded = false;
                Debug.Log("On Ground!");
                // You can add further actions here if the object is on the ground
            }
            else
            {
                isGrounded = true;
                Debug.Log("Not On Ground!");
                // You can add further actions here if the object is not on the ground
            }
        }

        private static float ClampAngle(float lfAngle, float lfMin, float lfMax)
        {
            if (lfAngle < -360f) lfAngle += 360f;
            if (lfAngle > 360f) lfAngle -= 360f;
            return Mathf.Clamp(lfAngle, lfMin, lfMax);
        }

        void OnDrawGizmosSelected()
        {
            // Draw a wire sphere representing the sphere cast for debugging
            Gizmos.color = Color.yellow;
            Gizmos.DrawWireSphere(transform.position + Vector3.up * yOffset, radius);
        }
    }
}