using EPOOutline;
using Player;
using UnityEngine;

namespace Items
{
    public abstract class InteractableBase : MonoBehaviour, IInteractable
    {
        [Header("Item Info")]
        [SerializeField] private string _itemName;
        [SerializeField] private ItemTypeSO _itemType;
        [SerializeField] [TextArea(3, 5)] private string _description;
        [SerializeField] [TextArea(2, 4)] private string _lore;

        public string ItemName => _itemName;
        public string ItemType => _itemType != null ? _itemType.DisplayName : "Неизвестно";
        public string Description => _description;
        public string Lore => _lore;

        private Outlinable _outlinable;

        protected virtual void Start()
        {
            _outlinable = GetComponent<Outlinable>();
            if (_outlinable == null)
                _outlinable = gameObject.AddComponent<Outlinable>();

            _outlinable.RenderStyle = RenderStyle.FrontBack;
            _outlinable.FrontParameters.Color = Color.green;
            _outlinable.BackParameters.Color = Color.red;
            _outlinable.OutlineParameters.DilateShift = 3f;
            _outlinable.enabled = false;
        }

        public virtual void OnFocus()
        {
            _outlinable.enabled = true;
        }

        public virtual void OnDefocus()
        {
            _outlinable.enabled = false;
        }
        
        public virtual bool CanInteract(PlayerTools tools)
        {
            return tools != null && tools.HasTool(ToolType.Tablet);
        }

        public abstract void Interact();
    }
}