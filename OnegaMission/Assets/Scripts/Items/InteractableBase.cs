using EPOOutline;
using Player;
using UnityEngine;
using UnityEngine.Events;

namespace Items
{
    /// <summary>
    /// Абстрактный базовый класс для всех интерактивных объектов.
    /// Содержит общую логику подсветки (Outlinable), информацию о предмете,
    /// а также виртуальные методы для переопределения в наследниках.
    /// </summary>
    public abstract class InteractableBase : MonoBehaviour, IInteractable
    {
        [Header("Item Info")]
        [SerializeField] private string _itemName;
        [SerializeField] private ItemTypeSO _itemType;          // ссылка на SO с типом
        [SerializeField] [TextArea(3, 5)] private string _description;
        [SerializeField] [TextArea(2, 4)] private string _lore;

        [Header("Events")]
        public UnityEvent OnFocusEvent;      // вызывается при наведении
        public UnityEvent OnDefocusEvent;    // вызывается при потере фокуса
        public UnityEvent OnInteractEvent;   // вызывается при успешном взаимодействии

        // Реализация свойств интерфейса
        public string ItemName => _itemName;
        public string ItemType => _itemType != null ? _itemType.DisplayName : "Неизвестно";
        public string Description => _description;
        public string Lore => _lore;
        public ItemTypeSO ItemTypeSO => _itemType;  // доступ к SO для сравнения

        // По умолчанию не показываем требование (переопределяется при необходимости)
        public virtual bool ShouldShowRequirement => false;

        private Outlinable _outlinable;

        protected virtual void Start()
        {
            // Настройка подсветки через EPOOutline
            _outlinable = GetComponent<Outlinable>();
            if (_outlinable == null)
                _outlinable = gameObject.AddComponent<Outlinable>();

            _outlinable.RenderStyle = RenderStyle.FrontBack;
            _outlinable.FrontParameters.Color = Color.green;
            _outlinable.BackParameters.Color = Color.red;
            _outlinable.OutlineParameters.DilateShift = 2f;
            _outlinable.enabled = false;
        }

        public virtual void OnFocus()
        {
            _outlinable.enabled = true;
            OnFocusEvent?.Invoke();
        }

        public virtual void OnDefocus()
        {
            _outlinable.enabled = false;
            OnDefocusEvent?.Invoke();
        }

        /// <summary>
        /// Базовая проверка: требует планшет.
        /// Переопределите в наследниках для других условий.
        /// </summary>
        public virtual bool CanInteract(PlayerTools tools)
        {
            return tools != null && tools.HasTool(ToolType.Tablet);
        }

        /// <summary>
        /// Абстрактный метод – каждый конкретный тип реализует своё взаимодействие.
        /// </summary>
        public abstract void Interact();
    }
}