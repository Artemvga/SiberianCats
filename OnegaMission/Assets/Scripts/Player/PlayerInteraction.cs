using InputSystemProject;
using Items;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.Events;

// -----------------------------------------------------------------------------
// Назначение файла: PlayerInteraction.cs
// Путь: Assets/Scripts/Player/PlayerInteraction.cs
// Описание: Содержит игровую логику, связанную с данным компонентом.
// Примечание: Комментарии добавлены для ускорения поддержки и онбординга.
// -----------------------------------------------------------------------------

namespace Player
{
    /// <summary>
    /// Реализует компонент `PlayerInteraction` и инкапсулирует связанную с ним игровую логику.
    /// </summary>
    public class PlayerInteraction : MonoBehaviour
    {
        public static PlayerInteraction Instance { get; private set; }

        [Header("References")]
        [SerializeField] private Camera _camera;
        [SerializeField] private float _interactionDistance = 10f;
        [SerializeField] private InteractionUI _interactionUI;
        [SerializeField] private LayerMask _interactableLayer;
        [SerializeField] private float _gracePeriod = 0.2f; // задержка перед сбросом фокуса

        [Header("Events")]
        public UnityEvent<IInteractable> OnFocusChanged;
        public UnityEvent OnInteract;

        private IInteractable _currentInteractable;
        private float _lastHitTime;

        /// <summary>
        /// Инициализирует объект при создании компонента Unity.
        /// </summary>
        private void Awake()
        {
            // Синглтон
            if (Instance != null && Instance != this)
            {
                Destroy(gameObject);
                return;
            }
            Instance = this;
        }

        /// <summary>
        /// Освобождает ресурсы перед уничтожением объекта.
        /// </summary>
        private void OnDestroy()
        {
            if (Instance == this) Instance = null;
        }

        /// <summary>
        /// Выполняет логику, которая должна обновляться каждый кадр.
        /// </summary>
        private void Update()
        {
            // Не проверяем интерактивные объекты, если открыт фоторежим или записка
            if (PhotoCameraMode.IsActive || NoteUI.IsActive) return;

            CheckInteractable();
        }

        /// <summary>
        /// Выполняет операцию `CheckInteractable` в рамках обязанностей текущего компонента.
        /// </summary>
        private void CheckInteractable()
        {
            Ray ray = _camera.ScreenPointToRay(new Vector3(Screen.width / 2f, Screen.height / 2f, 0));
            bool hitSomething = false;

            if (Physics.Raycast(ray, out RaycastHit hit, _interactionDistance, _interactableLayer))
            {
                IInteractable interactable = hit.collider.GetComponent<IInteractable>();
                if (interactable != null)
                {
                    hitSomething = true;
                    _lastHitTime = Time.time;

                    // Устанавливаем фокус (включаем подсветку)
                    SetCurrentInteractable(interactable);

                    // Получаем подсказку
                    string prompt = interactable.GetInteractionMessage();
                    if (!interactable.CanInteract(PlayerTools.Instance))
                        prompt = "🔒 " + prompt;

                    // Показываем UI с информацией и подсказкой
                    _interactionUI.Show(interactable, prompt);
                    return;
                }
            }

            // Если луч не попал в объект и прошло время gracePeriod – сбрасываем фокус
            if (!hitSomething && Time.time - _lastHitTime > _gracePeriod)
            {
                SetCurrentInteractable(null);
                _interactionUI.ClearContent(); // скрываем UI
            }
        }

        /// <summary>
        /// Выполняет операцию `SetCurrentInteractable` в рамках обязанностей текущего компонента.
        /// </summary>
        private void SetCurrentInteractable(IInteractable newInteractable)
        {
            if (_currentInteractable == newInteractable) return;

            // Снимаем подсветку со старого объекта
            if (_currentInteractable != null)
                _currentInteractable.OnDefocus();

            _currentInteractable = newInteractable;
            OnFocusChanged?.Invoke(newInteractable);

            // Включаем подсветку на новом объекте
            if (_currentInteractable != null)
                _currentInteractable.OnFocus();
        }

        /// <summary>
        /// Принудительно сбрасывает фокус и скрывает UI.
        /// </summary>
        public void ClearFocus()
        {
            SetCurrentInteractable(null);
            if (_interactionUI != null)
                _interactionUI.ClearContent();
        }

        /// <summary>
        /// Выполняет операцию `OnInteractPerformed` в рамках обязанностей текущего компонента.
        /// </summary>
        private void OnInteractPerformed(InputAction.CallbackContext context)
        {
            if (_currentInteractable != null)
            {
                _currentInteractable.Interact();
                OnInteract?.Invoke();
                ClearFocus();
            }
        }

        /// <summary>
        /// Срабатывает при активации компонента.
        /// </summary>
        private void OnEnable()
        {
            InputManager.Instance.actions.Player.Interact.performed += OnInteractPerformed;
        }

        /// <summary>
        /// Срабатывает при деактивации компонента.
        /// </summary>
        private void OnDisable()
        {
            InputManager.Instance.actions.Player.Interact.performed -= OnInteractPerformed;
        }
    }
}