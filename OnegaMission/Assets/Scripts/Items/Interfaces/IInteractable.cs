using Player;

namespace Items
{
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