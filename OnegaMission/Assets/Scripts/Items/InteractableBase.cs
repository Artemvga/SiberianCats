using EPOOutline;
using Player;
using UnityEngine;
using UnityEngine.Events;

namespace Items
{
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

        public virtual void OnFocus()
        {
            if (_outlinable != null && _outlinable.gameObject != null)
                _outlinable.enabled = true;
            OnFocusEvent?.Invoke();
        }

        public virtual void OnDefocus()
        {
            // Проверка, что объект не уничтожен
            if (this == null) return;
            if (_outlinable != null && _outlinable.gameObject != null)
                _outlinable.enabled = false;
            OnDefocusEvent?.Invoke();
        }

        public virtual bool CanInteract(PlayerTools tools)
        {
            return tools != null && tools.HasTool(ToolType.Tablet);
        }

        public abstract void Interact();
        public abstract string GetInteractionMessage();

        public void SetItemInfo(string name, ItemTypeSO type, string description, string lore)
        {
            _itemName = name;
            _itemType = type;
            _description = description;
            _lore = lore;
        }
    }
}