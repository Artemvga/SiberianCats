using Player;
using UnityEngine;

// -----------------------------------------------------------------------------
// Назначение файла: TrashItem.cs
// Путь: Assets/Scripts/Items/TrashItem.cs
// Описание: Содержит игровую логику, связанную с данным компонентом.
// Примечание: Комментарии добавлены для ускорения поддержки и онбординга.
// -----------------------------------------------------------------------------

namespace Items
{
    /// <summary>
    /// Реализует компонент `TrashItem` и инкапсулирует связанную с ним игровую логику.
    /// </summary>
    public class TrashItem : InteractableBase
    {
        private string _id;
        public string Id => _id;

        public override bool ShouldShowRequirement => false;

        /// <summary>
        /// Выполняет операцию `CanInteract` в рамках обязанностей текущего компонента.
        /// </summary>
        public override bool CanInteract(PlayerTools tools)
        {
            return ActiveTool.Instance.GetCurrentToolType() == ToolType.Tablet && TabletUI.IsOpen;
        }

        /// <summary>
        /// Выполняет операцию `Interact` в рамках обязанностей текущего компонента.
        /// </summary>
        public override void Interact()
        {
            if (!CanInteract(PlayerTools.Instance))
            {
                Debug.Log("Сначала возьми планшет в руки и открой его (I)");
                return;
            }

            if (Inventory.Instance != null && Inventory.Instance.AddItem(this))
            {
                // Сбрасываем фокус
                if (PlayerInteraction.Instance != null)
                    PlayerInteraction.Instance.ClearFocus();

                // Меняем слой, чтобы объект больше не попадал в луч
                gameObject.layer = LayerMask.NameToLayer("Default");

                // Не уничтожаем, а просто деактивируем
                gameObject.SetActive(false);
                Debug.Log($"{ItemName} добавлен в инвентарь");
                OnInteractEvent?.Invoke();
                // Destroy(gameObject); // УБИРАЕМ ЭТУ СТРОКУ
            }
            else
            {
                Debug.Log("Не удалось добавить предмет: инвентарь полон");
            }
        }

        /// <summary>
        /// Выполняет операцию `GetInteractionMessage` в рамках обязанностей текущего компонента.
        /// </summary>
        public override string GetInteractionMessage()
        {
            if (ActiveTool.Instance.GetCurrentToolType() == ToolType.Tablet && TabletUI.IsOpen)
                return "Нажмите E, чтобы собрать мусор";
            if (ActiveTool.Instance.GetCurrentToolType() == ToolType.Tablet && !TabletUI.IsOpen)
                return "Откройте планшет (I)";
            return "Возьмите планшет в руки (1) и откройте его (I)";
        }

        /// <summary>
        /// Выполняет операцию `SetData` в рамках обязанностей текущего компонента.
        /// </summary>
        public void SetData(string id, string newName, ItemTypeSO newType, string newDescription, string newLore)
        {
            _id = id;
            SetItemInfo(newName, newType, newDescription, newLore);
        }
    }
}