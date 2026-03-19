using Player;
using UnityEngine;

namespace Items
{ 
    public class ToolItem : InteractableBase
    {
        [SerializeField] private ToolType _toolType;

        public ToolType ToolType => _toolType;

        public override bool CanInteract(PlayerTools tools)
        {
            return true; 
        }

        public override void Interact()
        {
            if (PlayerTools.Instance != null && PlayerTools.Instance.AddTool(this))
            {
                gameObject.SetActive(false);
                Debug.Log($"Инструмент {ItemName} ({_toolType}) добавлен");
                Destroy(gameObject);
            }
            else
            {
                Debug.Log("Не удалось добавить инструмент");
            }
        }
    }
}