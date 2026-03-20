using Player;
using UnityEngine;

namespace Items
{
    /// <summary>
    /// Компонент инструмента (планшет, отвёртка и т.д.).
    /// Инструменты всегда можно подобрать, независимо от состояния.
    /// При подборе добавляются в PlayerTools.
    /// </summary>
    public class ToolItem : InteractableBase
    {
        [SerializeField] private ToolType _toolType;
        public ToolType ToolType => _toolType;

        // Инструменты не показывают требование
        public override bool ShouldShowRequirement => false;

        public override bool CanInteract(PlayerTools tools) => true;

        public override void Interact()
        {
            if (PlayerTools.Instance != null && PlayerTools.Instance.AddTool(this))
            {
                gameObject.SetActive(false);
                Debug.Log($"Инструмент {ItemName} ({_toolType}) добавлен");
                OnInteractEvent?.Invoke();
                Destroy(gameObject);
            }
            else
            {
                Debug.Log("Не удалось добавить инструмент");
            }
        }
    }
}