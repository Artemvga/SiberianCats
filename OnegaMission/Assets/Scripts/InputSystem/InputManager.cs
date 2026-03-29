using UnityEngine;

// -----------------------------------------------------------------------------
// Назначение файла: InputManager.cs
// Путь: Assets/Scripts/InputSystem/InputManager.cs
// Описание: Содержит игровую логику, связанную с данным компонентом.
// Примечание: Комментарии добавлены для ускорения поддержки и онбординга.
// -----------------------------------------------------------------------------

namespace InputSystemProject
{
    /// <summary>
    /// Менеджер ввода (синглтон). Создаётся автоматически до загрузки сцены.
    /// Содержит экземпляр Input Actions и позволяет переключать карты ввода (Player / UI).
    /// </summary>
    public class InputManager : MonoBehaviour
    {
        public static InputManager Instance { get; private set; }
        public InputSystem_Actions actions;

        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.BeforeSceneLoad)]
        /// <summary>
        /// Выполняет операцию `Init` в рамках обязанностей текущего компонента.
        /// </summary>
        public static void Init()
        {
            if (Instance != null) return;
            GameObject go = new GameObject("InputManager");
            Instance = go.AddComponent<InputManager>();
            DontDestroyOnLoad(go);
        }

        /// <summary>
        /// Инициализирует объект при создании компонента Unity.
        /// </summary>
        private void Awake()
        {
            if (Instance != null && Instance != this)
            {
                Destroy(gameObject);
                return;
            }
            actions = new InputSystem_Actions();
            actions.Enable();
        }

        /// <summary>
        /// Выполняет операцию `ChangeInputMap` в рамках обязанностей текущего компонента.
        /// </summary>
        public void ChangeInputMap(InputType inputType)
        {
            switch (inputType)
            {
                case InputType.Player:
                    actions.Player.Enable();
                    actions.UI.Enable();
                    break;
                case InputType.UI:
                    actions.UI.Enable();
                    actions.Player.Disable();
                    break;
            }
        }

        /// <summary>
        /// Освобождает ресурсы перед уничтожением объекта.
        /// </summary>
        private void OnDestroy()
        {
            if (Instance == this)
            {
                actions.Disable();
                actions = null;
                Instance = null;
            }
        }
    }
}