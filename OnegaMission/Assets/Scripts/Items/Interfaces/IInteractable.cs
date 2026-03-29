using Player;

// -----------------------------------------------------------------------------
// Назначение файла: IInteractable.cs
// Путь: Assets/Scripts/Items/Interfaces/IInteractable.cs
// Описание: Содержит игровую логику, связанную с данным компонентом.
// Примечание: Комментарии добавлены для ускорения поддержки и онбординга.
// -----------------------------------------------------------------------------

namespace Items
{
    /// <summary>
    /// Определяет контракт `IInteractable` для взаимодействия между компонентами.
    /// </summary>
    public interface IInteractable
    {
        string ItemName { get; }
        string ItemType { get; }
        string Description { get; }
        string Lore { get; }
        bool ShouldShowRequirement { get; }

        void OnFocus();
        void OnDefocus();
        void Interact();
        bool CanInteract(PlayerTools tools);

        /// <summary>Возвращает динамическую подсказку для UI (что нужно сделать).</summary>
        string GetInteractionMessage();
    }
}