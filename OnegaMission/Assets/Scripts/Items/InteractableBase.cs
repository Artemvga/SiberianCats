using EPOOutline;
using Player;
using UnityEngine;
using UnityEngine.Events;

// -----------------------------------------------------------------------------
// Назначение файла: InteractableBase.cs
// Путь: Assets/Scripts/Items/InteractableBase.cs
// Описание: Содержит игровую логику, связанную с данным компонентом.
// Примечание: Комментарии добавлены для ускорения поддержки и онбординга.
// -----------------------------------------------------------------------------

namespace Items
{
    /// <summary>
    /// Реализует компонент `InteractableBase` и инкапсулирует связанную с ним игровую логику.
    /// </summary>
    public abstract class InteractableBase : MonoBehaviour, IInteractable
    {
        [Header("Item Info")]
        [SerializeField] private string _itemName;
        [SerializeField] private ItemTypeSO _itemType;
        [SerializeField] [TextArea(3, 5)] private string _description;
        [SerializeField] [TextArea(2, 4)] private string _lore;

        [Header("Events")]
        public UnityEvent OnFocusEvent;
        public UnityEvent OnDefocusEvent;
        public UnityEvent OnInteractEvent;

        public string ItemName => _itemName;
        public string ItemType => _itemType != null ? _itemType.DisplayName : "Неизвестно";
        public string Description => _description;
        public string Lore => _lore;
        public ItemTypeSO ItemTypeSO => _itemType;

        public virtual bool ShouldShowRequirement => false;

        private Outlinable _outlinable;

        /// <summary>
        /// Запускает начальную настройку после инициализации сцены.
        /// </summary>
        protected virtual void Start()
        {
            _outlinable = GetComponent<Outlinable>();
            if (_outlinable == null)
                _outlinable = gameObject.AddComponent<Outlinable>();

            _outlinable.RenderStyle = RenderStyle.FrontBack;
            _outlinable.FrontParameters.Color = Color.green;
            _outlinable.BackParameters.Color = Color.red;
            _outlinable.OutlineParameters.DilateShift = 2f;
            _outlinable.enabled = false;
        }

        /// <summary>
        /// Выполняет операцию `OnFocus` в рамках обязанностей текущего компонента.
        /// </summary>
        public virtual void OnFocus()
        {
            if (_outlinable != null && _outlinable.gameObject != null)
                _outlinable.enabled = true;
            OnFocusEvent?.Invoke();
        }

        /// <summary>
        /// Выполняет операцию `OnDefocus` в рамках обязанностей текущего компонента.
        /// </summary>
        public virtual void OnDefocus()
        {
            // Проверка, что объект не уничтожен
            if (this == null) return;
            if (_outlinable != null && _outlinable.gameObject != null)
                _outlinable.enabled = false;
            OnDefocusEvent?.Invoke();
        }

        /// <summary>
        /// Выполняет операцию `CanInteract` в рамках обязанностей текущего компонента.
        /// </summary>
        public virtual bool CanInteract(PlayerTools tools)
        {
            return tools != null && tools.HasTool(ToolType.Tablet);
        }

        public abstract void Interact();
        public abstract string GetInteractionMessage();

        /// <summary>
        /// Выполняет операцию `SetItemInfo` в рамках обязанностей текущего компонента.
        /// </summary>
        public void SetItemInfo(string name, ItemTypeSO type, string description, string lore)
        {
            _itemName = name;
            _itemType = type;
            _description = description;
            _lore = lore;
        }
    }
}