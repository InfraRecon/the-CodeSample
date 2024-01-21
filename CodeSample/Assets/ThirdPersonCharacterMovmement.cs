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


        void Start()
        {
            rb = GetComponent<Rigidbody>();
            
            // Make sure the Rigidbody has constraints on rotation to avoid unexpected behavior
            rb.freezeRotation = true;

            _input = GetComponent<PlayerAssetsInput>();
            _playerInput = GetComponent<PlayerInput>();

        }

        void Update()
        {
            // Call the function to handle player input
            HandleInput();
        }

        void HandleInput()
        {
            // Calculate the movement direction
            Vector3 movement = new Vector3(_input.move.x, 0f, _input.move.y);

            // Move the character using forces
            MoveCharacter(movement);
        }

        void MoveCharacter(Vector3 direction)
        {
            // Calculate the desired movement vector based on input and speed
            Vector3 movement = direction * moveSpeed * Time.deltaTime;

            // Apply the force to the Rigidbody
            rb.AddForce(movement);
        }
    }
}