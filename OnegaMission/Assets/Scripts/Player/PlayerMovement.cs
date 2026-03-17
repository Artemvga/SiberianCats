using System;
using Unity.Cinemachine;
using UnityEngine;
using UnityEngine.InputSystem;

namespace Player
{
    [RequireComponent(typeof(CharacterController))]
    public class PlayerMovement : MonoBehaviour
    {
        [SerializeField] private float _currentSpeed;
        [SerializeField] private float _walkSpeed;
        [SerializeField] private float _sprintSpeed;
        [SerializeField] private CharacterController _characterController;
        [SerializeField] private CinemachineCamera _playerCamera;

        private Vector2 _move;

        private void Start()
        {
            _currentSpeed = _walkSpeed;
            Cursor.lockState = CursorLockMode.Locked;
            Cursor.visible = false;
        }

        public void OnMove(InputValue value)
        {
            _move = value.Get<Vector2>();
        }

        public void OnSprint(InputValue value)
        {
            if (value.Get<float>() > 0.5f)
            {
                _currentSpeed = _sprintSpeed;
            }
            else
            {
                _currentSpeed = _walkSpeed;
            }
        }
        private void Update()
        {
            _characterController.Move((GetForwad() * _move.y + GetRight() * _move.x) * Time.deltaTime * _currentSpeed);
        }

        private Vector3 GetForwad()
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

