using Player; // для PlayerTools

namespace Items
{
    public interface IInteractable
    {
        string ItemName { get; }
        string ItemType { get; }
        string Description { get; }
        string Lore { get; }

        void OnFocus();    
        void OnDefocus();  
        void Interact();   
        bool CanInteract(PlayerTools tools);
    }
}