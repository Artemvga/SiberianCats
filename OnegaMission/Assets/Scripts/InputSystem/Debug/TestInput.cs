using System;
using UnityEngine;
using UnityEngine.InputSystem;

namespace InputSystemProject
{
    public class TestInput : MonoBehaviour
    {
        private void OnEnable()
        {
            InputManager.Instance.actions.Player.Jump.performed += DoJump;
        }

        private void DoJump(InputAction.CallbackContext obj)
        {
            Debug.Log("DoJump");
        }

        private void Update()
        {
            Debug.Log(InputManager.Instance.actions.Player.Move.ReadValue<Vector2>());
        }

        private void OnDisable()
        {
            InputManager.Instance.actions.Player.Jump.performed -= DoJump;
        }
    }
}