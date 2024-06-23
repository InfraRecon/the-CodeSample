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
        public Animator animator;
        private string animMoveSpeed = "MoveSpeed";
        private string animIsFalling = "IsFalling";
        private string animIsJumping = "IsJumping";
        private string animIsAttacking = "IsAttacking";
        private string animAttackType = "AttackType";


        public float originalMoveSpeed = 100f; // Adjust the speed as needed
        public float moveSpeed = 100f; // Adjust the speed as needed
        public float currentSpeed = 0f;
        public float RotationSmoothSpeed = 1f;

        public float jumpCooldown = 1f;  // Adjust this value to control the cooldown duration
        private float jumpCooldownTimer = 0f;
        public float JumpHeight = 1f;

        private Rigidbody rb;

        private PlayerInput _playerInput;
        private PlayerAssetsInput _input;

        private GameObject _mainCamera;

        private const float _threshold = 0.01f;

        public Transform characterDirection;

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

        //Kinematicsrig
        public Transform SpineKinematics;


        private float _targetRotation = 0.0f;
        private float _rotationVelocity;
        private float _verticalVelocity;

        [Tooltip("How fast the character turns to face movement direction")]
        [Range(0.0f, 0.3f)]
        public float RotationSmoothTime = 0.12f;

        public PlatformLogic platformLogic;
        public GameObject LiftObject;

        public float radius = 0.5f;
        public float yOffset = 0.0f; // Offset distance from the center of the object in the y-axis
        public LayerMask groundLayer;
        public bool isGrounded = false;
        public GameObject GroundedCursor;
        private bool landed = true;
        public GameObject LandingParticle;

        public float attackRadius = 0.5f;
        public float attackyOffset = 0.0f; // Offset distance from the center of the object in the y-axis
        public LayerMask attackLayer;

        public float comboResetTime = 1f;  // Time to reset the combo
        private float lastAttackTime = 0f;
        private int comboStep = 0;
        private bool isAttacking = false;


        // The prefab to be instantiated
        public GameObject orbPrefab;

        // The number of orbs to spawn
        public int numberOfOrbs = 5;

        // The range of the explosion force
        public float explosionForce = 0.1f;
        public float explosionRadius = 0.1f;

        
        // The range for randomizing the initial position slightly
        public float positionVariance = 1f;


        public Transform faceTarget;
        public Transform footTarget;

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
            //CameraRotation();
        }

        void HandleInput()
        {
            // Move the character using forces
            GroundCheck();
            AttackCheck();

            SprintCharacter();
            MoveCharacter();
            jumpCooldownTimer -= Time.deltaTime;  // Decrease the cooldown timer
            JumpCharacter();
            AimRotation();
            AttackCharacter();
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

        private void AimRotation()
        {
            Vector2 inputDirection = new Vector2(_input.look.x, _input.look.y);
    
            // Check if there is any input for looking
            if (inputDirection != Vector2.zero)
            {
                // Calculate the target rotation based on input direction and camera orientation
                float targetRotation = Mathf.Atan2(inputDirection.x, inputDirection.y) * Mathf.Rad2Deg + _mainCamera.transform.eulerAngles.y;

                // Smoothly rotate towards the target rotation
                Quaternion targetQuaternion = Quaternion.Euler(0f, targetRotation, 0f);
                SpineKinematics.rotation = Quaternion.Slerp(SpineKinematics.rotation, targetQuaternion, Time.deltaTime * RotationSmoothSpeed);
            }
            else
            {
                // If there's no input direction, smoothly reset the character's transform rotation
                ResetCharacterRotation();
            }
        }

        private void ResetCharacterRotation()
        {
            // Calculate the rotation to reset to (usually identity rotation)
            Quaternion targetQuaternion = characterDirection.rotation;
            // Smoothly rotate towards the target rotation
            SpineKinematics.rotation = Quaternion.Slerp(SpineKinematics.rotation, targetQuaternion, Time.deltaTime * RotationSmoothSpeed);
        }

        void SprintCharacter()
        {
            if (_input.sprint)
            {
                moveSpeed = originalMoveSpeed + originalMoveSpeed/2f;
            }
            else
            {
                moveSpeed = originalMoveSpeed;
            }
        }

        void MoveCharacter()
        {
            Vector3 inputDirection = new Vector3(_input.move.x, 0.0f, _input.move.y).normalized;

            // Calculate the magnitude of the input direction to scale the speed
            float inputMagnitude = _input.move.magnitude;

            // note: Vector2's != operator uses approximation so is not floating point error prone, and is cheaper than magnitude
            // if there is a move input rotate player when the player is moving
            if (_input.move != Vector2.zero)
            {
                _targetRotation = Mathf.Atan2(inputDirection.x, inputDirection.z) * Mathf.Rad2Deg +
                                _mainCamera.transform.eulerAngles.y;
                float rotation = Mathf.SmoothDampAngle(transform.eulerAngles.y, _targetRotation, ref _rotationVelocity,
                    RotationSmoothTime);

                // Rotate to face input direction relative to camera position
                //transform.rotation = Quaternion.Euler(0.0f, rotation, 0.0f);

                Vector3 targetDirection = Quaternion.Euler(0.0f, _targetRotation, 0.0f) * Vector3.forward;

                // Smoothly interpolate the character direction towards the target direction
                Vector3 currentDirection = characterDirection.forward;
                Vector3 newDirection = Vector3.RotateTowards(currentDirection, targetDirection, Time.deltaTime * RotationSmoothSpeed, 0.0f);
                characterDirection.rotation = Quaternion.LookRotation(newDirection);

                Vector3 movement;

                if (isGrounded)
                {
                    // Adjust movement direction to align with ground slope
                    RaycastHit hit;
                    if (Physics.Raycast(transform.position + Vector3.up * yOffset, Vector3.down, out hit, radius, groundLayer))
                    {
                        targetDirection = Vector3.ProjectOnPlane(targetDirection, hit.normal).normalized;
                    }

                    // Calculate the desired movement vector based on input magnitude and speed
                    movement = targetDirection * moveSpeed * inputMagnitude * Time.deltaTime;
                }
                else
                {
                    // Calculate the desired movement vector based on input magnitude and speed
                    movement = targetDirection * moveSpeed * inputMagnitude * Time.deltaTime;
                }

                // Apply the force to the Rigidbody
                rb.velocity = new Vector3(movement.x, rb.velocity.y, movement.z);
            }

            // Get the current velocity of the Rigidbody
            Vector3 velocity = rb.velocity;

            // Project the velocity onto the XZ plane (ignore the Y component)
            Vector3 velocityXZ = new Vector3(velocity.x, 0.0f, velocity.z);

            // Calculate the magnitude (speed) of the projected velocity
            float targetSpeed = velocityXZ.magnitude;

            // Smoothly interpolate the current speed towards the target speed
            currentSpeed = Mathf.Lerp(currentSpeed, targetSpeed, Time.deltaTime * 10f);

            // Round the current speed to the nearest integer, only if it's stable
            if (Mathf.Abs(currentSpeed - Mathf.Round(currentSpeed)) < 0.05f)
            {
                currentSpeed = Mathf.Round(currentSpeed);
            }

            animator.SetFloat(animMoveSpeed, currentSpeed);
        }






        // void MoveCharacter()
        // {
        //     Vector3 inputDirection = new Vector3(_input.move.x, 0.0f, _input.move.y).normalized;

        //     // note: Vector2's != operator uses approximation so is not floating point error prone, and is cheaper than magnitude
        //     // if there is a move input rotate player when the player is moving
        //     if (_input.move != Vector2.zero)
        //     {
        //         _targetRotation = Mathf.Atan2(inputDirection.x, inputDirection.z) * Mathf.Rad2Deg +
        //                           _mainCamera.transform.eulerAngles.y;
        //         float rotation = Mathf.SmoothDampAngle(transform.eulerAngles.y, _targetRotation, ref _rotationVelocity,
        //             RotationSmoothTime);

        //         // rotate to face input direction relative to camera position
        //         //transform.rotation = Quaternion.Euler(0.0f, rotation, 0.0f);
            

        //         Vector3 targetDirection = Quaternion.Euler(0.0f, _targetRotation, 0.0f) * Vector3.forward;
                

        //         // Smoothly interpolate the character direction towards the target direction
        //         Vector3 currentDirection = characterDirection.forward;
        //         Vector3 newDirection = Vector3.RotateTowards(currentDirection, targetDirection, Time.deltaTime * RotationSmoothSpeed, 0.0f);
        //         characterDirection.rotation = Quaternion.LookRotation(newDirection);
                
        //         Vector3 movement;
                
        //         if(isGrounded)
        //         {
        //             // Calculate the desired movement vector based on input and speed
        //             movement = targetDirection * moveSpeed/4f * Time.deltaTime;
        //         }
        //         else
        //         {
        //             // Calculate the desired movement vector based on input and speed
        //             movement = targetDirection * moveSpeed * Time.deltaTime;
        //         }

        //         // Apply the force to the Rigidbody
        //         rb.AddForce(movement);
        //     }

        //     // Get the current velocity of the Rigidbody
        //     Vector3 velocity = rb.velocity;

        //     // Project the velocity onto the XZ plane (ignore the Y component)
        //     Vector3 velocityXZ = new Vector3(velocity.x, 0.0f, velocity.z);

        //     // Calculate the magnitude (speed) of the projected velocity
        //     currentSpeed = velocityXZ.magnitude;

        //     animator.SetFloat(animMoveSpeed,currentSpeed);
        // }

        void LiftCharacter()
        {
            if(_input.lift)
            {
                platformLogic.Lift();
                LiftObject.SetActive(true);
            }
            else
            {
                platformLogic.Reset();
                LiftObject.SetActive(false);
            }
        }

        void JumpCharacter()
        {
            if (!isGrounded && _input.jump && jumpCooldownTimer <= 0f)  // Check if jump is allowed
            {
                rb.AddForce(Vector3.up * JumpHeight, ForceMode.Impulse);
                jumpCooldownTimer = jumpCooldown;  // Reset the cooldown timer

                // Assuming you have an animator and animation setup
                animator.SetBool(animIsJumping, true);
            }
        }

        void AttackCharacter()
        {   
            if (!isGrounded && _input.attack)  // Check if jump is allowed
            {
                if (Time.time - lastAttackTime > comboResetTime)
                {
                    comboStep = 0;  // Reset the combo if too much time has passed
                }

                comboStep++;
                lastAttackTime = Time.time;
                isAttacking = true;

                if(currentSpeed <= 1f)
                {
                    animator.SetFloat(animAttackType, 0f);
                }
                else
                {
                    animator.SetFloat(animAttackType, 1f);
                }

                if (comboStep == 1)
                {
                    //animator.SetTrigger("Attack1");
                    
                    // Assuming you have an animator and animation setup
                    animator.SetTrigger(animIsAttacking);
                }
                else if (comboStep == 2)
                {
                    //animator.SetTrigger("Attack2");
                    
                    // Assuming you have an animator and animation setup
                    animator.SetTrigger(animIsAttacking);
                }
                else if (comboStep == 3)
                {
                    //animator.SetTrigger("Attack3");
                    
                    // Assuming you have an animator and animation setup
                    animator.SetTrigger(animIsAttacking);

                    comboStep = 0;  // Reset combo after the last attack
                }

                float currentAnimationTime = GetCurrentAnimationTime();

                if(currentAnimationTime >= 0.9f)
                {
                    EndAttack();
                }
            }

            // Reset combo if no input within the reset time
            if (Time.time - lastAttackTime > comboResetTime)
            {
                comboStep = 0;
            }

            _input.attack = false;
        }

        // Call this method from your animation events to indicate the attack is finished
        public void EndAttack()
        {
            isAttacking = false;
        }

        // Method to get the current time of the attack animation
        public float GetCurrentAnimationTime()
        {
            AnimatorStateInfo stateInfo = animator.GetCurrentAnimatorStateInfo(0);
            if (stateInfo.IsName("Attack"))
            {
                return stateInfo.normalizedTime % 1;
            }
            return 0f;
        }

        void GroundCheck()
        {
            // Perform a sphere cast downwards to check for ground
            RaycastHit hit;
            if (Physics.SphereCast(transform.position + Vector3.up * yOffset, radius, Vector3.down, out hit, radius, groundLayer))
            {
                isGrounded = false;
                Debug.Log("On Ground!");
                // You can add further actions here if the object is on the ground
                animator.SetBool(animIsFalling,false);
                animator.SetBool(animIsJumping,false);
                GroundedCursor.SetActive(false);
                
                if(!landed)
                {
                    GameObject instObject = Instantiate(LandingParticle, footTarget.position, Quaternion.identity);
                    landed = true;
                }
            }
            else
            {
                isGrounded = true;
                Debug.Log("Not On Ground!");
                // You can add further actions here if the object is not on the ground
                animator.SetBool(animIsFalling,true);
                GroundedCursor.SetActive(true);
                landed = false;
            }
        }

        void AttackCheck()
        {
            // Perform a sphere cast downwards to check for ground and detect objects
            Collider[] hitColliders = Physics.OverlapSphere(transform.position, attackRadius, attackLayer);

            if (!isGrounded && _input.attack)  // Check if jump is allowed
            {
                // Check if any colliders were detected
                if (hitColliders.Length > 0)
                {
                    foreach (var hitCollider in hitColliders)
                    {
                        // Destroy the detected object
                        SpawnOrbs(hitCollider.gameObject.transform);
                        Destroy(hitCollider.gameObject);
                    }
                }
            }
        }

        void SpawnOrbs(Transform spawnPosition)
        {
            for (int i = 0; i < numberOfOrbs; i++)
            {
                // Randomize the spawn position slightly
                Vector3 randomPosition = spawnPosition.position + new Vector3(
                    Random.Range(-positionVariance, positionVariance),
                    1f,
                    Random.Range(-positionVariance, positionVariance)
                );

                // Instantiate the orb
                GameObject orb = Instantiate(orbPrefab, randomPosition, Quaternion.identity);

                // Apply an upward force with a bit of randomness
                Rigidbody rb = orb.GetComponent<Rigidbody>();
                if (rb != null)
                {
                    Vector3 explosionDirection = (orb.transform.position - spawnPosition.position).normalized;
                    rb.AddForce((Vector3.up + explosionDirection) * explosionForce, ForceMode.Impulse);
                }
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

            Gizmos.color = Color.red;
            Gizmos.DrawWireSphere(transform.position + Vector3.up * attackyOffset, attackRadius);
        }
    }
}