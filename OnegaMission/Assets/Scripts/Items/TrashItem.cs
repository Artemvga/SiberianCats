using Player;
using UnityEngine;

namespace Items
{
    /// <summary>
    /// Компонент мусора.
    /// Взаимодействие возможно только при открытом планшете (TabletUI.IsOpen == true).
    /// При подборе предмет деактивируется (не уничтожается), чтобы данные сохранились для планшета.
    /// </summary>
    public class TrashItem : InteractableBase
    {
        public override bool ShouldShowRequirement => false;

        public override bool CanInteract(PlayerTools tools)
        {
            return TabletUI.IsOpen;
        }

        public override void Interact()
        {
            if (!TabletUI.IsOpen)
            {
                Debug.Log("Сначала открой планшет");
                return;
            }

            if (Inventory.Instance != null && Inventory.Instance.AddItem(this))
            {
                gameObject.SetActive(false); // деактивируем, но не уничтожаем
                Debug.Log($"{ItemName} добавлен в инвентарь");
                OnInteractEvent?.Invoke();
            }
            else
            {
                Debug.Log("Не удалось добавить предмет: инвентарь полон");
            }
        }
    }
}