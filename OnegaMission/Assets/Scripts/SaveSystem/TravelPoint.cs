using Items;
using UnityEngine;
using Player;

public class TravelPoint : MonoBehaviour, IInteractable
{
    [Header("Travel")]
    [SerializeField] private string _itemName = "Лодка";
    [SerializeField] private string _itemType = "Транспорт";

    public string ItemName => _itemName;
    public string ItemType => _itemType;
    public string Description => "На этой лодке можно отправиться в другие места";
    public string Lore => "";
    public bool ShouldShowRequirement => true;

    public void OnFocus() { }
    public void OnDefocus() { }

    public bool CanInteract(PlayerTools tools) => true;

    public void Interact()
    {
        if (TravelUI.Instance != null)
            TravelUI.Instance.Show();
        else
            Debug.LogError("TravelUI не найден");
    }

    public string GetInteractionMessage()
    {
        return "Нажмите E, чтобы отправиться в путешествие";
    }
}