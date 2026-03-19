using UnityEngine;
using UnityEngine.InputSystem;
using InputSystemProject;
using Unity.Cinemachine;

namespace Player
{
    public class PlayerMovement : MonoBehaviour
    {
        [SerializeField] private float _walkSpeed = 5f;
        [SerializeField] private float _sprintSpeed = 10f;
        [SerializeField] private CharacterController _characterController;
        [SerializeField] private CinemachineCamera _playerCamera;
        [SerializeField] private float _gravity = 9.8f;

        private float _currentSpeed;
        private Vector2 _moveInput;
        private float _verticalVelocity;

        private void OnEnable()
        {
            var playerMap = InputManager.Instance.actions.Player;
            playerMap.Move.performed += OnMovePerformed;
            playerMap.Move.canceled += OnMoveCanceled;
            playerMap.Sprint.performed += OnSprintPerformed;
            playerMap.Sprint.canceled += OnSprintCanceled;
        }

        private void OnDisable()
        {
            if (InputManager.Instance == null) return;
            var playerMap = InputManager.Instance.actions.Player;
            playerMap.Move.performed -= OnMovePerformed;
            playerMap.Move.canceled -= OnMoveCanceled;
            playerMap.Sprint.performed -= OnSprintPerformed;
            playerMap.Sprint.canceled -= OnSprintCanceled;
        }

        private void OnMovePerformed(InputAction.CallbackContext context)
        {
            _moveInput = context.ReadValue<Vector2>();
        }

        private void OnMoveCanceled(InputAction.CallbackContext context)
        {
            _moveInput = Vector2.zero;
        }

        private void OnSprintPerformed(InputAction.CallbackContext context)
        {
            _currentSpeed = _sprintSpeed;
        }

        private void OnSprintCanceled(InputAction.CallbackContext context)
        {
            _currentSpeed = _walkSpeed;
        }

        private void Start()
        {
            _currentSpeed = _walkSpeed;
            Cursor.lockState = CursorLockMode.Locked;
        }

        private void Update()
        {
            Vector3 move = (GetForward() * _moveInput.y + GetRight() * _moveInput.x) * _currentSpeed;

            if (_characterController.isGrounded && _verticalVelocity < 0)
                _verticalVelocity = -2f;
            else
                _verticalVelocity -= _gravity * Time.deltaTime;

            Vector3 velocity = move + new Vector3(0, _verticalVelocity, 0);
            _characterController.Move(velocity * Time.deltaTime);
        }

        private Vector3 GetForward()
        {
            Vector3 forward = _playerCamera.transform.forward;
            forward.y = 0;
            return forward.normalized;
        }

        private Vector3 GetRight()
        {
            Vector3 right = _playerCamera.transform.right;
            right.y = 0;
            return right.normalized;
        }
    }
}