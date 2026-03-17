namespace Items
{
    public interface IInteractable
    {
        void Change();
        void Interact();
        string GetDescription();
    }
}