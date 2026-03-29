using System.Collections.Generic;
using Items;
using Player;
using UnityEngine;

// -----------------------------------------------------------------------------
// Назначение файла: ToolSpawner.cs
// Путь: Assets/Scripts/Game/ToolSpawner.cs
// Описание: Содержит игровую логику, связанную с данным компонентом.
// Примечание: Комментарии добавлены для ускорения поддержки и онбординга.
// -----------------------------------------------------------------------------

/// <summary>
/// Реализует компонент `ToolSpawner` и инкапсулирует связанную с ним игровую логику.
/// </summary>
public class ToolSpawner : MonoBehaviour
{
    [System.Serializable]
    public struct ToolSpawnInfo
    {
        public ToolType toolType;
        public GameObject toolPrefab;
        public Transform spawnPoint;
    }

    [SerializeField] private List<ToolSpawnInfo> _toolsToSpawn;

    /// <summary>
    /// Запускает начальную настройку после инициализации сцены.
    /// </summary>
    private void Start()
    {
        foreach (var info in _toolsToSpawn)
        {
            if (!PlayerTools.Instance.HasTool(info.toolType))
            {
                Instantiate(info.toolPrefab, info.spawnPoint.position, info.spawnPoint.rotation);
                Debug.Log($"Спавн инструмента {info.toolType}");
            }
        }
    }
}