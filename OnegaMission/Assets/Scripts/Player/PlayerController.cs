using UnityEngine;

// -----------------------------------------------------------------------------
// Назначение файла: PlayerController.cs
// Путь: Assets/Scripts/Player/PlayerController.cs
// Описание: Содержит игровую логику, связанную с данным компонентом.
// Примечание: Комментарии добавлены для ускорения поддержки и онбординга.
// -----------------------------------------------------------------------------

/// <summary>
/// Реализует компонент `PlayerController` и инкапсулирует связанную с ним игровую логику.
/// </summary>
public class PlayerController : MonoBehaviour
{
    public static PlayerController Instance { get; private set; }

    /// <summary>
    /// Инициализирует объект при создании компонента Unity.
    /// </summary>
    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;
        DontDestroyOnLoad(gameObject);
    }
}