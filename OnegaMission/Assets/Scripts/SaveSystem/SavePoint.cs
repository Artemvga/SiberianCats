using Items;
using UnityEngine;
using Player;

// -----------------------------------------------------------------------------
// Назначение файла: SavePoint.cs
// Путь: Assets/Scripts/SaveSystem/SavePoint.cs
// Описание: Содержит игровую логику, связанную с данным компонентом.
// Примечание: Комментарии добавлены для ускорения поддержки и онбординга.
// -----------------------------------------------------------------------------

/// <summary>
/// Реализует компонент `SavePoint` и инкапсулирует связанную с ним игровую логику.
/// </summary>
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

    /// <summary>
    /// Выполняет операцию `Interact` в рамках обязанностей текущего компонента.
    /// </summary>
    public void Interact()
    {
        SaveManager.Instance.SaveGame(_savePointName);
        Debug.Log("Игра сохранена!");
    }

    /// <summary>
    /// Выполняет операцию `GetInteractionMessage` в рамках обязанностей текущего компонента.
    /// </summary>
    public string GetInteractionMessage()
    {
        return "Нажмите E, чтобы сохраниться";
    }
}