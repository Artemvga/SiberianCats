using Items;
using UnityEngine;
using Player;

public class SavePoint : MonoBehaviour, IInteractable
{
    [Header("Save Point")]
    [SerializeField] private string _savePointName = "Спальник";
    [SerializeField] private GameObject _interactionUI;

    public string ItemName => _savePointName;
    public string ItemType => "Спальное место";
    public string Description => "Место, где можно сохраниться";
    public string Lore => "";
    public bool ShouldShowRequirement => true;

    public void OnFocus() { }
    public void OnDefocus() { }

    public bool CanInteract(PlayerTools tools) => true;

    public void Interact()
    {
        SaveManager.Instance.SaveGame(_savePointName);
        Debug.Log("Игра сохранена!");
    }

    public string GetInteractionMessage()
    {
        return "Нажмите E, чтобы сохраниться";
    }
}