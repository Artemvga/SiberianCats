using System;
using UnityEngine;
using UnityEngine.InputSystem;

namespace InputSystemProject
{
    public class TestInput : MonoBehaviour
    {
        private void OnEnable()
        {
            InputManager.Instance.actions.Player.Interact.performed += DoJump;
        }
        
        private void DoJump(InputAction.CallbackContext obj)
        {
            Debug.Log("DoJump");
        }
        
        private void OnDisable()
        {
            InputManager.Instance.actions.Player.Interact.performed -= DoJump;
        }
    }
}