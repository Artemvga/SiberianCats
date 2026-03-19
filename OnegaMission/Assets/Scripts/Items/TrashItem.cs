using Player;
using UnityEngine;

namespace Items
{
    public class TrashItem : InteractableBase
    {
        public override bool CanInteract(PlayerTools tools)
        {
            return tools != null && tools.HasTool(ToolType.Tablet);
        }
        
        public override void Interact()
        {
            if (!TabletUI.IsOpen)
            {
                Debug.Log("Сначала открой планшет (нажми I)");
                return;
            }

            if (Inventory.Instance != null && Inventory.Instance.AddItem(this))
            {
                gameObject.SetActive(false);
                Debug.Log($"{ItemName} добавлен в инвентарь");
                Destroy(gameObject);
            }
            else
            {
                Debug.Log("Не удалось добавить предмет: инвентарь полон");
            }
        }
    }
}