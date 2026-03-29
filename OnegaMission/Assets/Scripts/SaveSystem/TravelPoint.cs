using Items;
using UnityEngine;
using Player;

// -----------------------------------------------------------------------------
// Назначение файла: TravelPoint.cs
// Путь: Assets/Scripts/SaveSystem/TravelPoint.cs
// Описание: Содержит игровую логику, связанную с данным компонентом.
// Примечание: Комментарии добавлены для ускорения поддержки и онбординга.
// -----------------------------------------------------------------------------

/// <summary>
/// Реализует компонент `TravelPoint` и инкапсулирует связанную с ним игровую логику.
/// </summary>
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

    /// <summary>
    /// Выполняет операцию `Interact` в рамках обязанностей текущего компонента.
    /// </summary>
    public void Interact()
    {
        if (TravelUI.Instance != null)
            TravelUI.Instance.Show();
        else
            Debug.LogError("TravelUI не найден");
    }

    /// <summary>
    /// Выполняет операцию `GetInteractionMessage` в рамках обязанностей текущего компонента.
    /// </summary>
    public string GetInteractionMessage()
    {
        return "Нажмите E, чтобы отправиться в путешествие";
    }
}