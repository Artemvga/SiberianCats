using System;
using UnityEngine;
using UnityEngine.InputSystem;

// -----------------------------------------------------------------------------
// Назначение файла: TestInput.cs
// Путь: Assets/Scripts/InputSystem/Debug/TestInput.cs
// Описание: Содержит игровую логику, связанную с данным компонентом.
// Примечание: Комментарии добавлены для ускорения поддержки и онбординга.
// -----------------------------------------------------------------------------

namespace InputSystemProject
{
    /// <summary>
    /// Реализует компонент `TestInput` и инкапсулирует связанную с ним игровую логику.
    /// </summary>
    public class TestInput : MonoBehaviour
    {
        /// <summary>
        /// Срабатывает при активации компонента.
        /// </summary>
        private void OnEnable()
        {
            InputManager.Instance.actions.Player.Interact.performed += DoJump;
        }
        
        /// <summary>
        /// Выполняет операцию `DoJump` в рамках обязанностей текущего компонента.
        /// </summary>
        private void DoJump(InputAction.CallbackContext obj)
        {
            Debug.Log("DoJump");
        }
        
        /// <summary>
        /// Срабатывает при деактивации компонента.
        /// </summary>
        private void OnDisable()
        {
            InputManager.Instance.actions.Player.Interact.performed -= DoJump;
        }
    }
}