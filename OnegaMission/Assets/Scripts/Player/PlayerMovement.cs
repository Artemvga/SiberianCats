using UnityEngine;
using UnityEngine.InputSystem;
using InputSystemProject;
using Unity.Cinemachine;

// -----------------------------------------------------------------------------
// Назначение файла: PlayerMovement.cs
// Путь: Assets/Scripts/Player/PlayerMovement.cs
// Описание: Содержит игровую логику, связанную с данным компонентом.
// Примечание: Комментарии добавлены для ускорения поддержки и онбординга.
// -----------------------------------------------------------------------------

namespace Player
{
    /// <summary>
    /// Управление передвижением игрока с использованием новой Input System.
    /// Подписывается на действия Move и Sprint из InputManager.
    /// </summary>
    public class PlayerMovement : MonoBehaviour
    {
        [Header("Movement Settings")]
        [SerializeField] private float _walkSpeed = 5f;
        [SerializeField] private float _sprintSpeed = 10f;
        [SerializeField] private CharacterController _characterController;
        [SerializeField] private CinemachineCamera _playerCamera;
        [SerializeField] private float _gravity = 9.8f;

        private float _currentSpeed;
        private Vector2 _moveInput;
        private float _verticalVelocity;

        /// <summary>
        /// Срабатывает при активации компонента.
        /// </summary>
        private void OnEnable()
        {
            var playerMap = InputManager.Instance.actions.Player;
            playerMap.Move.performed += OnMovePerformed;
            playerMap.Move.canceled += OnMoveCanceled;
            playerMap.Sprint.performed += OnSprintPerformed;
            playerMap.Sprint.canceled += OnSprintCanceled;
        }

        /// <summary>
        /// Срабатывает при деактивации компонента.
        /// </summary>
        private void OnDisable()
        {
            if (InputManager.Instance == null) return;
            var playerMap = InputManager.Instance.actions.Player;
            playerMap.Move.performed -= OnMovePerformed;
            playerMap.Move.canceled -= OnMoveCanceled;
            playerMap.Sprint.performed -= OnSprintPerformed;
            playerMap.Sprint.canceled -= OnSprintCanceled;
        }

        private void OnMovePerformed(InputAction.CallbackContext context) => _moveInput = context.ReadValue<Vector2>();
        private void OnMoveCanceled(InputAction.CallbackContext context) => _moveInput = Vector2.zero;
        private void OnSprintPerformed(InputAction.CallbackContext context) => _currentSpeed = _sprintSpeed;
        private void OnSprintCanceled(InputAction.CallbackContext context) => _currentSpeed = _walkSpeed;

        /// <summary>
        /// Запускает начальную настройку после инициализации сцены.
        /// </summary>
        private void Start()
        {
            _currentSpeed = _walkSpeed;
            Cursor.lockState = CursorLockMode.Locked;
        }

        /// <summary>
        /// Выполняет логику, которая должна обновляться каждый кадр.
        /// </summary>
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

        /// <summary>
        /// Выполняет операцию `GetForward` в рамках обязанностей текущего компонента.
        /// </summary>
        private Vector3 GetForward()
        {
            Vector3 forward = _playerCamera.transform.forward;
            forward.y = 0;
            return forward.normalized;
        }

        /// <summary>
        /// Выполняет операцию `GetRight` в рамках обязанностей текущего компонента.
        /// </summary>
        private Vector3 GetRight()
        {
            Vector3 right = _playerCamera.transform.right;
            right.y = 0;
            return right.normalized;
        }
    }
}