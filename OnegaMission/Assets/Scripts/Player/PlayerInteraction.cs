using InputSystemProject;
using Items;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.Events;

namespace Player
{
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

        private void OnDestroy()
        {
            if (Instance == this) Instance = null;
        }

        private void Update()
        {
            // Не проверяем интерактивные объекты, если открыт фоторежим или записка
            if (PhotoCameraMode.IsActive || NoteUI.IsActive) return;

            CheckInteractable();
        }

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

        private void OnInteractPerformed(InputAction.CallbackContext context)
        {
            if (_currentInteractable != null)
            {
                _currentInteractable.Interact();
                OnInteract?.Invoke();
                ClearFocus();
            }
        }

        private void OnEnable()
        {
            InputManager.Instance.actions.Player.Interact.performed += OnInteractPerformed;
        }

        private void OnDisable()
        {
            InputManager.Instance.actions.Player.Interact.performed -= OnInteractPerformed;
        }
    }
}