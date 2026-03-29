using Items;
using UnityEngine;
using UnityEngine.Events;

// -----------------------------------------------------------------------------
// Назначение файла: Bin.cs
// Путь: Assets/Scripts/Sorting/Bin.cs
// Описание: Содержит игровую логику, связанную с данным компонентом.
// Примечание: Комментарии добавлены для ускорения поддержки и онбординга.
// -----------------------------------------------------------------------------

/// <summary>
/// Контейнер (корзина) для сортировки мусора.
/// Хранит тип принимаемого мусора.
/// </summary>
public class Bin : MonoBehaviour
{
    [SerializeField] private ItemTypeSO _acceptedType;
    public ItemTypeSO AcceptedType => _acceptedType;

    // Опциональные события для звуков/анимаций
    public UnityEvent OnCorrect;
    public UnityEvent OnWrong;
}